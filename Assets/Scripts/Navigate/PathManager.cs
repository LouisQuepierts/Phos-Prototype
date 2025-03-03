using Phos.Callback;
using Phos.Structure;
using System.Collections.Generic;
using UnityEngine;

namespace Phos.Navigate {
    [ExecuteInEditMode]
    public class PathManager : MonoBehaviour, ICallbackListener<object> {
        private static PathManager CurrentManager;

        public static PathManager TryGetInstance() {
            if (CurrentManager == null) {
                PathManager temp = FindFirstObjectByType<PathManager>();

                if (temp == null) {
                    GameObject go = new GameObject("PathManager");
                    temp = go.AddComponent<PathManager>();
                }

                CurrentManager = temp;
            }
            return CurrentManager;
        }

        public static PathManager GetInstance() {
            return CurrentManager;
        }

        [SerializeField]
        public List<NodePath> m_paths = new();
        public Dictionary<NavigateNode, PathList> m_nodePaths = new();

        private void OnEnable() {
            CurrentManager = this;

            foreach (var path in m_paths) {
                MakeReference(path.nodeA, path);
                MakeReference(path.nodeB, path);
            }
        }

        private void OnDisable() {
            if (CurrentManager == this) {
                CurrentManager = null;
            }
        }

        public void UpdateAccessable(NavigateNode src) {
            foreach (var node in m_nodePaths.Keys) {
                node.accessable = false;
            }

            Queue<NavigateNode> queue = new();
            HashSet<NavigateNode> visited = new();

            queue.Enqueue(src);
            visited.Add(src);

            while (queue.Count > 0) {
                NavigateNode current = queue.Dequeue();
                current.accessable = true;

                foreach (var path in m_nodePaths[current]) {
                    NavigateNode next = path.GetOther(current);
                    if (!path.active || visited.Contains(next)) continue;

                    queue.Enqueue(next);
                    visited.Add(next);
                }
            }

            Debug.Log("Update Accessable");
        }

        public void Register(NavigateNode node) {
            if (!m_nodePaths.ContainsKey(node)) {
                m_nodePaths.Add(node, new ());
            }

            node.Paths = m_nodePaths[node];
        }

        public bool FindPath(
            NavigateNode src,
            NavigateNode dst,
            out NavigatePath naviPath
        ) {

            naviPath = NavigatePath.Empty;
            if (src == null || dst == null || src == dst) {
                return false;
            }

            Queue<NavigateNode> queue = new();
            Dictionary<NavigateNode, NodePath> prev = new();
            HashSet<NavigateNode> visited = new();

            queue.Enqueue(src);
            visited.Add(src);

            while (queue.Count > 0) {
                NavigateNode current = queue.Dequeue();

                foreach (var path in m_nodePaths[current]) {
                    if (path.active && !visited.Contains(path.GetOther(current))) {
                        NavigateNode next = path.GetOther(current);
                        queue.Enqueue(next);
                        visited.Add(next);
                        prev.Add(next, path);

                        if (next == dst) {
                            List<NodePath> result = new();
                            current = dst;

                            while (current != src) {
                                NodePath vpath = prev[current];
                                result.Add(vpath);
                                current = vpath.GetOther(current);
                            }

                            result.Reverse();
                            naviPath = new NavigatePath(result, src, dst);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public void Connect(
            NavigateNode src,
            NavigateNode dest,
            Direction direction,
            Direction opposite,
            bool neighbor = true
        ) {
            if (!src.enabled || !dest.enabled 
                || src == dest
                || IsConnected(src, dest)) return;

            NodePath path = new(
                src, dest,
                direction, opposite,
                true, neighbor
            );

            m_paths.Add(path);
            MakeReference(src, path);
            MakeReference(dest, path);
        }

        public bool IsConnected(NavigateNode src, NavigateNode dest) {
            if (!src.enabled || !dest.enabled
                || !m_nodePaths.ContainsKey(src) || !m_nodePaths.ContainsKey(dest)) return false;

            foreach (var path in src.Paths) {
                if (path != null && path.IsRelated(dest)) return true;
            }

            return false;
        }

        public void DisconnectAll(NavigateNode src) {
            if (src == null) return;
            foreach (var path in src.Paths) {
                if (path != null) {
                    NavigateNode dest = path.GetOther(src);
                    m_paths.Remove(path);
                    EraseReference(dest, path);
                }
            }
            src.Paths.Clear();
        }

        public void Disconnect(NavigateNode src, NavigateNode dest) {
            if (src == dest) return;

            NodePath path = src.Paths[dest];
            if (path == null) return;

            m_paths.Remove(path);
            EraseReference(src, path);
            EraseReference(dest, path);
        }

        private void MakeReference(NavigateNode node, NodePath path) {
            if (!m_nodePaths.TryGetValue(node, out PathList paths)) {
                paths = new();
                m_nodePaths.Add(node, paths);
            } else {
                if (paths.Contains(path)) return;
            }
            paths.Add(path);
        }

        private void EraseReference(NavigateNode node, NodePath path) {
            if (m_nodePaths.TryGetValue(node, out PathList paths)) {
                paths.Remove(path);
            }
        }

        public Dictionary<NavigateNode, PathList>.KeyCollection GetRegisteredNodes() {
            return m_nodePaths.Keys;
        }

        public void OnCallback(object t) {

        }

        public PathList this[NavigateNode node] {
            get {
                if (node == null || !m_nodePaths.ContainsKey(node)) return null;
                return m_nodePaths[node];
            }
        }

        public sealed class PathList : List<NodePath> {
            public NodePath this[NavigateNode node] { 
                get {
                    if (node == null) return null;
                    return Find(path => path.IsRelated(node));
                } 
            }
        }
    }
}