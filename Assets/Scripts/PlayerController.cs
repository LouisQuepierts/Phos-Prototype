using Phos.Navigate;
using UnityEngine;

namespace Phos {
    public class PlayerController : MonoBehaviour {
        public NavigateNode current;
        public NavigateNode clicked;

        //[Header("Runtime Node")]
        //public BaseNode pathCurrent;
        //public BaseNode pathLast;
        //public float distance;
        //public float pathProgress;

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

                //pathCurrent = m_path.CurrentNode();
                //pathLast = m_path.LastNode();

                //if (pathCurrent != null && pathLast != null) {
                //    distance = Vector3.Distance(pathCurrent.GetNodePoint(), pathLast.GetNodePoint());
                //    float pDistance = Vector3.Distance(transform.position, pathLast.GetNodePoint());
                //    pathProgress = Mathf.Clamp(1 - pDistance / distance, 0f, 1f);
                //}
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

            //if (pathCurrent != null) {
            //    Gizmos.color = Color.red;
            //    Gizmos.DrawCube(pathCurrent.transform.position, Vector3.one);
            //}

            //if (pathLast != null) {
            //    Gizmos.color = Color.blue;
            //    Gizmos.DrawCube(pathLast.transform.position, Vector3.one);
            //}
        }

        private void HandleMouseInput() {
            if (Input.GetMouseButtonDown(0)) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit)) {
                    NavigateNode node = hit.collider.GetComponent<NavigateNode>();

                    if (node != null && node != current && node != clicked && node.accessable) {
                        clicked = node;
                        PathManager controller = PathManager.TryGetInstance();
                        if (controller.FindPath(current, clicked, out m_path)) {
                            if (Vector3.Distance(current.transform.position, transform.position) > 0.5f) {
                                Debug.Log("Is Moving");
                                Direction direction = current.GetSimilarDirection(transform.position);
                                BaseNode other = current.GetConnectedNode(direction);
                                if (other != null) {
                                    m_path.Setup(other);
                                }
                            }
                        }
                    }
                }
            } else if (Input.GetMouseButtonDown(1)) {
                PathManager.TryGetInstance().UpdateAccessable(current);
            }
        }
    }
}