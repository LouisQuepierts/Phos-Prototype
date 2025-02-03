using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using System.IO;
using Phos.Navigate;

namespace PhosEditor {
    [CustomEditor(typeof(NavigateNode))]
    public class NavigateNodeInspector : Editor {
        private const float CAST_DISTANCE = 1f;

        private NavigateNode input;
        private ReorderableList list;

        private void OnDisable() {
            LevelEditor.HighlightPath = null;
        }

        private void OnEnable() {
            list = new ReorderableList(
                ((NavigateNode)target).Paths,
                typeof(Phos.Navigate.NodePath),
                false,
                true,
                false,
                false
            ) {
                drawHeaderCallback = rect => {
                    EditorGUI.LabelField(rect, "Paths");
                },
                drawElementCallback = (rect, index, isActive, isFocused) => {
                    Phos.Navigate.NodePath path = ((NavigateNode)target).Paths[index];
                    path.foldout = EditorGUI.Foldout(new Rect(rect.x + 10, rect.y, rect.width, 20), path.foldout, $"Path {path.nodeA.transform.position} - {path.nodeB.transform.position}");

                    if (path.foldout) {
                        var directionA = (Direction)EditorGUI.EnumPopup(new Rect(rect.x, rect.y + 30, rect.width, 20), "Direction A", path.directionA);
                        var directionB = (Direction)EditorGUI.EnumPopup(new Rect(rect.x, rect.y + 50, rect.width, 20), "Direction B", path.directionB);
                        var active = EditorGUI.Toggle(new Rect(rect.x, rect.y + 70, rect.width, 20), "Active", path.active);
                        var neighbor = EditorGUI.Toggle(new Rect(rect.x, rect.y + 90, rect.width, 20), "Neighbor", path.neighbor);
                        VirtualNode router = null;

                        if (!neighbor) {
                            router = (VirtualNode)EditorGUI.ObjectField(new Rect(rect.x, rect.y + 110, rect.width, 20), "VirtualNode", path.router, typeof(VirtualNode), true);
                        }

                        if (directionA != path.directionA 
                        || directionB != path.directionB 
                        || active != path.active 
                        || neighbor != path.neighbor 
                        || router != path.router) {
                            path.directionA = directionA;
                            path.directionB = directionB;
                            path.active = active;
                            path.neighbor = neighbor;
                            path.router = router;
                            EditorUtility.SetDirty(target);
                        }
                    }

                    EditorGUILayout.Space(rect.height);
                },
                elementHeightCallback = index => {
                    Phos.Navigate.NodePath path = ((NavigateNode)target).Paths[index];
                    return path.foldout ? (path.neighbor ? 120 : 140) : 20;
                },
                onSelectCallback = list => {
                    Phos.Navigate.NodePath path = ((NavigateNode)target).Paths[list.index];

                    if (LevelEditor.HighlightPath != path) {
                        LevelEditor.HighlightPath = path;
                        SceneView.RepaintAll();
                    }
                },
                onAddCallback = list => { },
                onRemoveCallback = list => { }
            };
        }

        public override void OnInspectorGUI() {
            list.DoList(EditorGUILayout.GetControlRect());
            if (list.count == 0) {
                EditorGUILayout.Space(20);
            }
            NavigateNode node = (NavigateNode)target;

            EditorGUILayout.Space(20);
            float offset = EditorGUILayout.FloatField("Offset", node.offset);
            NavigateNode.Type type = (NavigateNode.Type)EditorGUILayout.EnumPopup("Type", node.type);

            if (offset != node.offset || type != node.type) {
                node.offset = offset;
                node.type = type;
                EditorUtility.SetDirty(target);
            }

            serializedObject.ApplyModifiedProperties();


            GUILayout.Space(20);
            GUILayout.Label("Connection Management");
            if (GUILayout.Button("Rebuild Connections")) {
                RebuildConnections();
            }

            if (GUILayout.Button("Chained Rebuild Connections")) {
                ChainedRebuildConnections(node);
            }

            if (GUILayout.Button("Reset Connections")) {
                (node).ResetConnection();
            }
        }

        private static void ChainedRebuildConnections(NavigateNode target) {
            HashSet<NavigateNode> visited = new();
            Queue<NavigateNode> queue = new();
            queue.Enqueue(target);

            while (queue.Count > 0) {
                NavigateNode node = queue.Dequeue();
                Collider[] hits = Physics.OverlapSphere(node.transform.position, CAST_DISTANCE);

                foreach (var item in hits) {
                    NavigateNode other = item.GetComponent<NavigateNode>();
                    if (visited.Contains(other)) continue;

                    if (node.TryConnect(other)) {
                        visited.Add(other);
                        queue.Enqueue(other);
                    }
                }
            }
        }

        private void RebuildConnections() {
            NavigateNode node = (NavigateNode) target;
            Transform transform = node.transform;
            Collider[] hits = Physics.OverlapSphere(node.GetNodePoint(), CAST_DISTANCE);

            foreach (var item in hits) {
                NavigateNode other = item.GetComponent<NavigateNode>();
                node.TryConnect(other);
            }
        }
    }
}
