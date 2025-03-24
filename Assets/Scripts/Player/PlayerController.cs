using System;
using Phos.Navigate;
using Phos.Trigger;
using UnityEngine;

namespace Phos {
    public class PlayerController : MonoBehaviour {
        public NavigateNode current;

        private NavigatePath _path = NavigatePath.Empty;
        private int _layerMask;

        public void MoveTo(NavigateNode node) {
            if (node == current || !node.Accessable) return;
                
            PathManager controller = PathManager.TryGetInstance();
                
            _path.Free();
            if (!controller.FindPath(current, node, out _path)) return;
                
            if (!(Vector3.Distance(current.transform.position, transform.position) > 0.5f)) return;
            
            Direction direction = current.GetSimilarDirection(transform.position);
            BaseNode other = current.GetConnectedNode(direction);
            if (other) {
                _path.Setup(other);
            }
        }

        public void Stop() {
            if (_path == NavigatePath.Empty) return;
            _path.Free();
            _path = NavigatePath.Empty;

            Vector3 up = current.transform.up;
            Vector3 forward = Vector3.ProjectOnPlane(transform.forward, up);
            transform.position = current.GetNodePosition();
            transform.rotation = Quaternion.LookRotation(forward, up);
        }

        private void Awake() {
            _layerMask = 0 | 1 << LayerMask.NameToLayer("Node");
            // Debug.Log(Convert.ToString(_layerMask, 2).PadLeft(32, '0'));
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
                current.OnArrive(transform);
                _path.Free();
                _path = NavigatePath.Empty;
                return;
            }

            _path.Move(this);
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

        public void Spawn(Vector3 position) {
            transform.position = position;
            
            Ray ray = new Ray(transform.position + transform.up, -transform.up);

            if (!Physics.Raycast(ray, out RaycastHit hit, 4f, _layerMask)) return;
            NavigateNode node = hit.collider.GetComponent<NavigateNode>();

            if (node == null) return;
            current = node;
            PathManager.TryGetInstance().UpdateAccessable(node);
        }
    }
}