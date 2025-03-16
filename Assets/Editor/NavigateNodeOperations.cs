using Phos.Navigate;
using System;
using UnityEditor;
using UnityEngine;

namespace PhosEditor {
    public static class NavigateNodeOperations {
        public static void ConnectSelectedNodes() {
            var selected = Selection.gameObjects;

            // if selected is null or not exactly 2 nodes, return
            if (selected == null || selected.Length != 2) return;

            NavigateNode nodeA = selected[0].GetComponent<NavigateNode>();
            NavigateNode nodeB = selected[1].GetComponent<NavigateNode>();

            // if connect succeed, return
            if (nodeA.TryConnect(nodeB)) return;

            PathManager.TryGetInstance().Connect(nodeB, nodeA, Direction.Forward, Direction.Backward, false);

            Debug.Log("Connect Succeed");
        }

        public static void DisconnectSelectedNodes() {
            var selected = Selection.gameObjects;
            // if selected is null or not exactly 2 nodes, return
            if (selected == null || selected.Length != 2) return;

            NavigateNode nodeA = selected[0].GetComponent<NavigateNode>();
            NavigateNode nodeB = selected[1].GetComponent<NavigateNode>();

            PathManager.TryGetInstance().Disconnect(nodeA, nodeB);

            Debug.Log("Disconnect Succeed");
        }

        public static void ResetSelectedNodes() {
            var selected = Selection.gameObjects;

            if (selected == null) return;

            PathManager controller = PathManager.TryGetInstance();
            foreach (var item in selected) {
                NavigateNode node = item.GetComponent<NavigateNode>();
                if (node != null) {
                    PathManager.TryGetInstance().DisconnectAll(node);
                }
            }
            Debug.Log("Reset Succeed");
        }

        public static void RemoveSelectedNodes() {
            if (Selection.gameObjects == null) return;

            foreach (var item in Selection.gameObjects) {
                if (item.TryGetComponent<NavigateNode>(out var node)) {
                    node.ResetConnection();
                    Undo.DestroyObjectImmediate(item);
                }
            }
        }

        public static void CreateVirtualNode() {
            NodePath path = LevelEditor.HighlightPath;
            Debug.Log("Create Virtual Node");
            // if not selection or neighbor path, return
            if (path == null || path.neighbor) return;

            NavigateNode nodeA = path.nodeA;
            NavigateNode nodeB = path.nodeB;

            NavigateNode lower = nodeA.transform.position.y < nodeB.transform.position.y ? nodeA : nodeB;
            NavigateNode higher = lower == nodeB ? nodeA : nodeB;

            // if virtual node already exist
            VirtualNode[] nodes = lower.GetComponentsInChildren<VirtualNode>();
            Direction direction = path.GetDirection(lower);

            VirtualNode vnode = Array.Find(nodes, item => item.direction == direction);
            if (vnode == null) {
                GameObject nodeObject = new GameObject($"VirtualNode {direction}");
                vnode = nodeObject.AddComponent<VirtualNode>();
                vnode.direction = direction;
            }

            path.router = vnode;

            vnode.transform.parent = lower.transform;
            vnode.transform.position = higher.transform.position
                + higher.GetRelativeConnectionPosition(direction.Opposite(), BaseNode.BLOCK_SCALE);

            EditorUtility.SetDirty(Selection.activeGameObject);
        }

        public static void MoveHigher() {
            if (Selection.gameObjects.Length == 0) return;
            Vector3 forward = Camera.main.transform.forward;
            Vector3 delta = new(
                -Mathf.RoundToInt(forward.x / Mathf.Abs(forward.x)),
                1,
                -Mathf.RoundToInt(forward.x / Mathf.Abs(forward.z))
            );

            Transform[] transforms = new Transform[Selection.gameObjects.Length];

            for (int i = 0; i < Selection.gameObjects.Length; i++) {
                transforms[i] = Selection.gameObjects[i].transform;
            }

            Undo.RecordObjects(transforms, "Move Higher");

            foreach (var transform in transforms) {
                transform.position += delta;
            }
        }

        public static void MoveLower() {
            if (Selection.gameObjects.Length == 0) return;
            Vector3 forward = Camera.main.transform.forward;
            Vector3 delta = new(
                Mathf.RoundToInt(forward.x / Mathf.Abs(forward.x)),
                -1,
                Mathf.RoundToInt(forward.x / Mathf.Abs(forward.z))
            );

            Transform[] transforms = new Transform[Selection.gameObjects.Length];

            for (int i = 0; i < Selection.gameObjects.Length; i++) {
                transforms[i] = Selection.gameObjects[i].transform;
            }

            Undo.RecordObjects(transforms, "Move Lower");

            foreach (var transform in transforms) {
                transform.position += delta;
            }
        }
    }
}