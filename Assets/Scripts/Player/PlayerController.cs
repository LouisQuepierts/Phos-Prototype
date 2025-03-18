using System;
using Phos.Navigate;
using Phos.Trigger;
using UnityEngine;

namespace Phos {
    public class PlayerController : MonoBehaviour {
        private const float MaxRaycastDistance = 64f;
        private static PlayerController instance;

        public NavigateNode current;
        public NavigateNode clicked;

        //[Header("Runtime Node")]
        //public BaseNode pathCurrent;
        //public BaseNode pathLast;
        //public float distance;
        //public float pathProgress;

        private NavigatePath _path = NavigatePath.Empty;
        private int _layerMask;

        public static PlayerController GetInstance() {
            return instance;
        }

        private void Awake() {
            instance = this;
            _layerMask = 0 | 1 << LayerMask.NameToLayer("Node");
            Debug.Log(Convert.ToString(_layerMask, 2).PadLeft(32, '0'));

            Ray ray = new Ray(transform.position + transform.up, -transform.up);

            if (!Physics.Raycast(ray, out RaycastHit hit, 4f, _layerMask)) return;
            NavigateNode node = hit.collider.GetComponent<NavigateNode>();

            if (node == null) return;
            current = node;
            PathManager.TryGetInstance().UpdateAccessable(node);
        }

        void Update() {
            HandleMouseInput();
        }

        private void FixedUpdate() {
            Ray ray = new Ray(transform.position + transform.up, -transform.up);

            if (Physics.Raycast(ray, out RaycastHit hit, 4f, _layerMask)) {
                NavigateNode node = hit.collider.GetComponent<NavigateNode>();

                if (node) {
                    current = node;

                    if (transform.parent != node.transform.parent) {
                        Transform last = transform.parent;
                        transform.parent = node.transform.parent;
                        ParentTriggerBehaviour.Trigger(node.transform, last, gameObject);
                    }
                }
            }

            if (_path == NavigatePath.Empty) return;
            
            if (_path.Arrive(transform)) {
                transform.position = _path.Destination().GetNodePosition();
                _path.Free();
                _path = NavigatePath.Empty;
                return;
            }

            _path.Move(this);

            //pathCurrent = _path.CurrentNode();
            //pathLast = _path.LastNode();

            //if (pathCurrent != null && pathLast != null) {
            //    distance = Vector3.Distance(pathCurrent.GetNodePoint(), pathLast.GetNodePoint());
            //    float pDistance = Vector3.Distance(transform.position, pathLast.GetNodePoint());
            //    pathProgress = Mathf.Clamp(1 - pDistance / distance, 0f, 1f);
            //}
        }

        private void OnDrawGizmos() {
            if (_path != NavigatePath.Empty) {
                var move = _path.Next(transform);
                bool show = false;
                foreach (var path in _path) {
                    if (path == move) {
                        show = true;
                    } else if (!show) {
                        continue;
                    }

                    Gizmos.color = path.IsTeleport ? Color.yellow : Color.cyan;
                    Gizmos.DrawCube(path.Target, Vector3.one * 0.2f);
                }
            }

            //if (pathCurrent != null) {
            //    Gizmos.color = Color.red;
            //    Gizmos.DrawCube(pathCurrent.transform.position, Vector3.one);
            //}

            //if (pathLast != null) {
            //    Gizmos.color = Color.blue;
            //    Gizmos.DrawCube(pathLast.transform.position, Vector3.one);
            //}

            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.forward);
        }

        private void HandleMouseInput() {
            if (Input.GetMouseButtonDown(0)) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (!Physics.Raycast(ray, out RaycastHit hit, MaxRaycastDistance, _layerMask)) return;
                NavigateNode node = hit.collider.GetComponent<NavigateNode>();

                if (!node || node == current || node == clicked || !node.Accessable) return;
                clicked = node;
                
                PathManager controller = PathManager.TryGetInstance();
                
                _path.Free();
                if (!controller.FindPath(current, clicked, out _path)) return;
                
                if (!(Vector3.Distance(current.transform.position, transform.position) > 0.5f)) return;
                Debug.Log("Is Moving");
                Direction direction = current.GetSimilarDirection(transform.position);
                BaseNode other = current.GetConnectedNode(direction);
                if (other) {
                    _path.Setup(other);
                }
            } else if (Input.GetMouseButtonDown(1)) {
                PathManager.TryGetInstance().UpdateAccessable(current);
            }
        }
    }
}