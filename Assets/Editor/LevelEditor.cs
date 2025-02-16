using Phos.Navigate;
using System;
using UnityEditor;
using UnityEngine;

namespace PhosEditor {
    [InitializeOnLoad]
    public class LevelEditor {
        public static NodePath HighlightPath {
            get; set;
        }

        public static bool SelectionChanged {
            get {
                if (selectionChanged) {
                    selectionChanged = false;
                    return true;
                }
                return false;
            }
        }

        public readonly static GameObject NodeTemplate = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Editor/Prefabs/Node.prefab");

        private static bool enable = false;
        private static bool selectionChanged = false;
        
        private readonly static Color colorHighlight = new(1f, 0f, 0f, .25f);
        private readonly static Color colorSelected = new(0f, 0f, 1f, 0.25f);
        private readonly static Color colorNeighbor = new(0f, 1f, 0f, .25f);
        private readonly static Color colorTeleport = new(1f, 0f, 1f, .25f);
        private readonly static Color colorInactive = new(1f, .5f, .5f, .25f);
        private readonly static Color colorHighlightPath = new(1f, 1f, 0f, .25f);

        private static Vector3 hitPoint = Vector3.zero;

        static LevelEditor() {
            SceneView.duringSceneGui += OnSceneGUI;
            Selection.selectionChanged += OnSelectionChanged;
        }

        private static void OnSelectionChanged() {
            selectionChanged = true;
        }

        public static void Toggle() {
            enable = !enable;
        }

        private static void OnSceneGUI(SceneView view) {
            if (Application.isPlaying) return;

            PaintSelection();

            Event e = Event.current;
            if (e == null) return;

            switch (e.type) {
                case EventType.MouseUp:
                    OnMousePressed(e);
                    break;
                case EventType.KeyUp:
                    OnKeyPressed(e);
                    break;
            }
        }

        private static void PaintSelection() {
            if (!enable) return;

            if (Selection.gameObjects != null) {
                NavigateNode node;
                if (Selection.gameObjects.Length == 1
                    && (node = Selection.gameObjects[0].GetComponent<NavigateNode>()) != null) {
                    Handles.color = colorHighlight;
                    Handles.CubeHandleCap(
                        0,
                        node.transform.position,
                        node.transform.rotation,
                        1.0f,
                        EventType.Repaint
                    );

                    foreach (var path in node.Paths) {
                        if (path == null) continue;
                        UnityEngine.Transform transform = path.GetOther(node).transform;
                        Handles.color = path.active ? (path.neighbor ? colorNeighbor : colorTeleport) : colorInactive;
                        Handles.CubeHandleCap(
                            0,
                            transform.position,
                            transform.rotation,
                            1.0f,
                            EventType.Repaint
                        );
                    }
                } else if (Selection.gameObjects.Length > 1) {
                    foreach (var item in Selection.gameObjects) {
                        Handles.color = item == Selection.activeGameObject ? colorHighlight : colorSelected;
                        if (item.GetComponent<NavigateNode>() != null) {
                            Handles.CubeHandleCap(
                                0,
                                item.transform.position,
                                item.transform.rotation,
                                1.0f,
                                EventType.Repaint
                            );
                        }
                    }
                }
            }

            if (HighlightPath != null) {
                Handles.color = colorHighlightPath;
                Handles.CubeHandleCap(
                    0,
                    HighlightPath.nodeA.GetConnectPoint(HighlightPath.directionA),
                    HighlightPath.nodeA.transform.rotation,
                    0.4f,
                    EventType.Repaint
                );
                Handles.CubeHandleCap(
                    0,
                    HighlightPath.nodeB.GetConnectPoint(HighlightPath.directionB),
                    HighlightPath.nodeB.transform.rotation,
                    0.4f,
                    EventType.Repaint
                );

                if (!HighlightPath.neighbor && HighlightPath.router != null) {
                    Handles.color = colorTeleport;
                    Handles.CubeHandleCap(
                        0,
                        HighlightPath.router.transform.position,
                        HighlightPath.router.transform.rotation,
                        0.4f,
                        EventType.Repaint
                    );
                }
            }
        }

        private static void OnMousePressed(Event e) {
            if (!enable) return;

            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                GameObject target = hit.collider.gameObject;
                hitPoint = hit.point;
                if (target != null) {
                    switch (e.button) {
                        case 0:
                            OnMouseLeftClick(e, hit, target);
                            break;
                        case 1:     // RIGHT
                            if (e.shift) {
                                AddNode(hit, target);
                            }
                            break;
                    }
                }

            } else {
                hitPoint = Vector3.zero;
            }

            if (hitPoint != Vector3.zero) {
                Handles.color = Color.cyan;
                Handles.SphereHandleCap(0, hitPoint, Quaternion.identity, 0.1f, EventType.Repaint);
            }
        }

        private static void OnKeyPressed(Event e) {
            if (e.shift) {
                if (e.keyCode == KeyCode.E) {
                    Toggle();
                    e.Use();
                    return;
                }

                if (!enable) return;

                switch (e.keyCode) {
                    case KeyCode.C: // Connect
                        NavigateNodeOperations.ConnectSelectedNodes();
                        e.Use();
                        break;
                    case KeyCode.N: // VirtualNode
                        Debug.Log("Shift + N");
                        NavigateNodeOperations.CreateVirtualNode();
                        e.Use();
                        break;
                    case KeyCode.D: // Disconnect
                        NavigateNodeOperations.DisconnectSelectedNodes();
                        e.Use();
                        break;
                    case KeyCode.R: // Reset
                        NavigateNodeOperations.ResetSelectedNodes();
                        e.Use();
                        break;
                    case KeyCode.X: // Delete
                        NavigateNodeOperations.RemoveSelectedNodes();
                        e.Use();
                        break;
                    case KeyCode.Equals:        // +
                    case KeyCode.Plus: 
                    case KeyCode.KeypadPlus: 
                        NavigateNodeOperations.MoveHigher();
                        e.Use();
                        break;
                    case KeyCode.Underscore: 
                    case KeyCode.Minus:
                    case KeyCode.KeypadMinus:
                        NavigateNodeOperations.MoveLower();
                        e.Use();
                        break;
                }
            }
        }

        private static void OnMouseLeftClick(Event e, RaycastHit hit, GameObject target) {
            if (!e.shift || !target.TryGetComponent<NavigateNode>(out NavigateNode node)) return;

            NodePath path = HighlightPath;
            if (path == null || !path.IsRelated(node)) return;
            e.Use();

            Vector3 hitPoint = hit.point;

            Direction currentDirection = path.GetDirection(node);
            Direction minDirection = currentDirection;
            float minMagnitude = Mathf.Min(
                (node.GetConnectPoint(minDirection) - hitPoint).magnitude,
                (node.GetNodePoint() - hitPoint).magnitude
            );

            foreach (Direction direction in Directions.Value) {
                if (direction == currentDirection) continue;
                Vector3 point = node.GetConnectPoint(direction);
                float magnitude = (point - hitPoint).magnitude;

                if (magnitude < minMagnitude) {
                    minDirection = direction;
                    minMagnitude = magnitude;
                }
            }

            if (minDirection != currentDirection) {
                path.SetDirection(node, minDirection);

                if (target == Selection.activeGameObject) {
                    EditorUtility.SetDirty(node);
                } else {
                    EditorUtility.SetDirty(path.GetOther(node));
                }
            }
        }

        private static void RemoveNode(GameObject target) {
            if (target != Selection.activeObject || target.GetComponent<NavigateNode>() == null) return;
            target.GetComponent<NavigateNode>().ResetConnection();
            Undo.DestroyObjectImmediate(target);
            Event.current.Use();
        }

        private static void AddNode(RaycastHit hit, GameObject target) {
            if (hit.collider.gameObject.GetComponent<NavigateNode>() != null) return;
            Vector3 direction = hit.normal;
            Vector3 position = target.transform.parent.InverseTransformPoint(hit.point - direction * 0.1f);
            position.x = Mathf.Round(position.x);
            position.y = Mathf.Round(position.y);
            position.z = Mathf.Round(position.z);

            GameObject newObject = GameObject.Instantiate(NodeTemplate, target.transform.parent);
            newObject.transform.localPosition = position;
            newObject.transform.up = direction;

            // TODO Build Connections
            NavigateNode node = newObject.GetComponent<NavigateNode>();
            Transform transform = node.transform;
            Collider[] hits = Physics.OverlapSphere(node.GetNodePoint(), 1f);

            foreach (var item in hits) {
                NavigateNode other = item.GetComponent<NavigateNode>();
                node.TryConnect(other);
            }

            Undo.RegisterCreatedObjectUndo(newObject, "Create On Click");

            Debug.Log($"Placed a node at {position}; facing {direction}");
            Event.current.Use();
        }

        public static Vector3 TransformDirection(Vector3 direction, GameObject target) {
            if (target.transform.parent == null) {
                return direction;
            } else {
                return target.transform.parent.InverseTransformDirection(direction);
            }
        }

        [MenuItem("Tool/LevelEditor/Enable", true)]
        static bool ValidateEnable() {
            return !enable;
        }

        [MenuItem("Tool/LevelEditor/Disable", true)]
        static bool ValidateDisable() {
            return enable;
        }

        [MenuItem("Tool/LevelEditor/Enable")]
        static void Enable() {
            enable = true;
        }

        [MenuItem("Tool/LevelEditor/Disable")]
        static void Disable() {
            enable = false;
        }

        [MenuItem("Tool/Setup Camera")]
        static void SetupCamera() {
            Quaternion quaternion = Quaternion.Euler(35.264f, 45f, 0f);

            Camera camera = Camera.main;
            if (camera != null) {
                camera.transform.rotation = quaternion;
                camera.orthographic = true;
                camera.nearClipPlane = 0.3f;
                camera.farClipPlane = 1000.0f;
                camera.orthographicSize = 10f;
            }
        }

        [MenuItem("Tool/Setup Editor Camera")]
        static void SetupEditorCamera() {
            Quaternion quaternion = Quaternion.Euler(35.264f, 45f, 0f);

            SceneView view = SceneView.lastActiveSceneView;
            if (view != null) {
                view.rotation = quaternion;
                view.orthographic = true;
                view.size = 10;
            }
        }
    }
}