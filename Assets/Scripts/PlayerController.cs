using Phos.Navigate;
using Unity.VisualScripting;
using UnityEngine;

namespace Phos {
    public class PlayerController : MonoBehaviour {
        public NavigateNode current;
        public NavigateNode clicked;

        private NavigatePath m_path = NavigatePath.Empty;

        private void Awake() {
            Ray ray = new Ray(transform.position + transform.up, -transform.up);

            if (Physics.Raycast(ray, out RaycastHit hit, 4f)) {
                NavigateNode node = hit.collider.GetComponent<NavigateNode>();

                if (node != null) {
                    current = node;
                    PathManager.TryGetInstance().UpdateAccessable(node);
                }
            }
        }

        void Update() {
            HandleMouseInput();
        }

        private void FixedUpdate() {
            Ray ray = new Ray(transform.position + transform.up, -transform.up);

            if (Physics.Raycast(ray, out RaycastHit hit, 4f)) {
                NavigateNode node = hit.collider.GetComponent<NavigateNode>();

                if (node != null) {
                    current = node;
                    transform.parent = node.transform.parent;
                }
            }

            if (m_path != NavigatePath.Empty) {

                if (m_path.Arrive(transform)) {
                    transform.position = m_path.Last().Target;
                    m_path = NavigatePath.Empty;
                    return;
                }

                m_path.Move(transform);
            }
        }

        private void OnDrawGizmos() {
            if (m_path != NavigatePath.Empty) {
                var move = m_path.Next(transform);
                bool show = false;
                foreach (var path in m_path) {
                    if (path == move) {
                        show = true;
                    } else if (!show) {
                        continue;
                    }

                    Gizmos.color = path.IsTeleport ? Color.yellow : Color.cyan;
                    Gizmos.DrawCube(path.Target, Vector3.one * 0.2f);
                }
            }
        }

        private void HandleMouseInput() {
            if (Input.GetMouseButtonDown(0)) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit)) {
                    NavigateNode node = hit.collider.GetComponent<NavigateNode>();

                    if (node != null && node != current && node != clicked && node.accessable) {
                        clicked = node;
                        PathManager controller = PathManager.TryGetInstance();
                        controller.FindPath(current, clicked, out m_path);
                    }
                }
            } else if (Input.GetMouseButtonDown(1)) {
                PathManager.TryGetInstance().UpdateAccessable(current);
            }
        }
    }
}