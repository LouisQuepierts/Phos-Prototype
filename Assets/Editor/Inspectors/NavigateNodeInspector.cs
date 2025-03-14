using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using Phos.Navigate;

namespace PhosEditor {
    [CustomEditor(typeof(NavigateNode))]
    public class NavigateNodeInspector : Editor {
        private static readonly Color gray = new Color(0.28f, 0.28f, 0.28f);

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

            bool dirty = false;
            //bool clicked = Event.current.type == EventType.MouseDown && Event.current.button == 0;
            //bool hasClicked = false;

            //EditorGUILayout.LabelField("Paths");
            //EditorGUILayout.BeginVertical(GUI.skin.box);
            //foreach (var path in node.Paths) {
            //    Rect rect = EditorGUILayout.BeginVertical();
            //    EditorGUI.DrawRect(rect, gray);
            //    path.foldout = EditorGUILayout.Foldout(path.foldout, $"{path.nodeA.transform.position} - {path.nodeB.transform.position}", true);

            //    if (path.foldout && DrawPathProperties(path)) {
            //        dirty = true;
            //    }

            //    //EditorGUILayout.LabelField($"{path.nodeA.transform.position} - {path.nodeB.transform.position}");
            //    EditorGUILayout.EndVertical();

            //    if (clicked && !hasClicked) {
            //        hasClicked = rect.Contains(Event.current.mousePosition);

            //        if (hasClicked && LevelEditor.HighlightPath != path) {
            //            LevelEditor.HighlightPath = path; 
            //            SceneView.RepaintAll();
            //        }
            //    }
            //}
            //EditorGUILayout.EndVertical();

            float offset = EditorGUILayout.FloatField("Offset", node.offset);
            NodeType type = (NodeType)EditorGUILayout.EnumPopup("NodeType", node.type);

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

            if (dirty) {
                EditorUtility.SetDirty(target);
            }

            GUILayout.Label("Misc");
            NavigateNode.ShowGizmos = GUILayout.Toggle(NavigateNode.ShowGizmos, "Show Gizmos");
        }

        //private bool DrawPathProperties(NodePath path) {
        //    bool changed = false;
        //    var directionA = (Direction)EditorGUILayout.EnumPopup("Direction A", path.directionA);
        //    var directionB = (Direction)EditorGUILayout.EnumPopup("Direction B", path.directionB);
        //    var active = EditorGUILayout.Toggle("Active", path.active);
        //    var neighbor = EditorGUILayout.Toggle("Neighbor", path.neighbor); VirtualNode router = null;

        //    if (!neighbor) {
        //        router = (VirtualNode)EditorGUILayout.ObjectField("VirtualNode", path.router, typeof(VirtualNode), true);
        //    }

        //    // Draw Predicates
        //    EditorGUILayout.LabelField("Predicate");

        //    EditorGUILayout.BeginHorizontal();
        //    EditorGUILayout.Space(1);
        //    EditorGUILayout.BeginVertical();
        //    var predicate = path.predicate;
        //    var logicalOperator = (LogicalOperator)EditorGUILayout.EnumPopup("Operator", predicate.@operator);

        //    path.showPredicates = EditorGUILayout.Foldout(path.showPredicates, "Predicates");

        //    if (path.showPredicates) {
        //        Stack<int> erase = new Stack<int>(predicate.predicates.Count);
        //        for (int i = 0; i < predicate.predicates.Count; i++) {
        //            BasePredicate instance = predicate.predicates[i];

        //            if (instance == null) {
        //                erase.Push(i);
        //            } else {
        //                predicate.predicates[i] = (BasePredicate)EditorGUILayout.ObjectField("Instance", instance, typeof(BasePredicate), true);
        //            }
        //        }

        //        while (erase.Count != 0) {
        //            int i = erase.Pop();
        //            predicate.predicates.RemoveAt(i);
        //            changed = true;
        //        }

        //        EditorGUILayout.Space(5);
        //        newPredicateType = (PredicateType)EditorGUILayout.EnumPopup("Type", newPredicateType);
        //        if (GUILayout.Button("Create")) {
        //            UnityEngine.Object target = serializedObject.targetObject;
        //            if (target is MonoBehaviour @object) {
        //                Type predicateType = PredicateHolder.GetPredicateType(newPredicateType);
        //                GameObject predicateInstance = new GameObject(newPredicateType.ToString() + " Predicate");
        //                BasePredicate i = (BasePredicate)predicateInstance.AddComponent(predicateType);
        //                predicateInstance.transform.position = Vector3.zero;
        //                predicateInstance.transform.parent = @object.gameObject.transform;
        //                predicate.predicates.Add(i);
        //                changed = true;
        //            }
        //            //property.objectReferenceValue = (Object)PredicateHolder.Factory.GetInstance(selected);
        //        }
        //    }

        //    EditorGUILayout.Space(5);
        //    EditorGUILayout.EndVertical();
        //    EditorGUILayout.EndHorizontal();

        //    var callback = (CallbackProvider<StructureControl.CallbackContext>)EditorGUILayout.ObjectField("Callback", path.callback, typeof(CallbackProvider<StructureControl.CallbackContext>), true);

        //    EditorGUILayout.Space(5);

        //    if (directionA != path.directionA
        //        || directionB != path.directionB
        //        || active != path.active
        //        || neighbor != path.neighbor
        //        || router != path.router
        //        || callback != path.callback
        //        || logicalOperator != path.predicate.@operator) {
        //        path.directionA = directionA;
        //        path.directionB = directionB;
        //        path.active = active;
        //        path.neighbor = neighbor;
        //        path.router = router;
        //        path.callback = callback;
        //        path.predicate.@operator = logicalOperator;
        //        changed = true;
        //    }

        //    return changed;
        //}

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
