using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phos.Navigate {
    public class NavigatePath : IEnumerable<NavigateOperation> {
        public static readonly NavigatePath Empty = new(new(), null, null);

        private List<NodePath> m_path;
        private List<NavigateOperation> m_moves;

        private BaseNode m_last;
        private BaseNode m_current;

        private int index;

        public NavigatePath(List<NodePath> path, NavigateNode src, NavigateNode dst) {
            m_path = path;
            index = 0;

            m_moves = new();
            m_last = src;
            m_current = src;

            if (src != null && dst != null) {
                NavigateNode current = src;
                foreach (var item in path) {
                    NavigateNode other = item.GetOther(current);

                    if (item.router != null) {
                        if (item.router.Bound == current) {
                            m_moves.Add(new NavigateOperation(item.router, item, true));
                            m_moves.Add(new NavigateOperation(other, item));
                        } else {
                            m_moves.Add(new NavigateOperation(item.router, item));
                            m_moves.Add(new NavigateOperation(other, item, true));
                        }
                    } else {
                        m_moves.Add(new NavigateOperation(other, item));
                    }


                    /*if (!item.neighbor) {
                        Vector3 adjust = current.GetNodePoint();
                        if (adjust.y != to.y) {
                            adjust.y = Mathf.Max(adjust.y, to.y);
                        }

                        if (m_moves.Count > 0) {
                            m_moves[^1].Target.Set(adjust.x, adjust.y, adjust.z);
                        } else {
                            m_moves.Add(new MoveOpertaion(adjust, true, 1f));
                        }
                    }

                    m_moves.Add(new MoveOpertaion(go, false, 0.2f));

                    if ((go - to).sqrMagnitude > 1e-6) {
                        m_moves.Add(new MoveOpertaion(to, !item.neighbor, 0.2f));
                    }

                    m_moves.Add(new MoveOpertaion(other.GetNodePoint(), false, 0.2f));*/
                    current = other;
                }

            }
        }

        public IEnumerator<NavigateOperation> GetEnumerator() {
            return m_moves.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return m_moves.GetEnumerator();
        }

        public void Setup(BaseNode last) {
            m_last = last;
        }

        public bool HasNext() {
            return index < m_moves.Count;
        }

        public NavigateOperation Next(Transform transform) {
            if (index >= m_moves.Count - 1) return Last();

            if ((transform.position - m_moves[index].Target).sqrMagnitude < 0.01f) {
                index++;
                Debug.Log("Next");

                NavigateOperation current = m_moves[index];

                m_last = m_current;
                m_current = current.Node;
            }

            return m_moves[index];
        }

        public void Move(PlayerController controller) {
            Transform transform = controller.transform;
            NavigateOperation operation = Next(transform);

            Vector3 target = operation.Target;
            if (operation.IsTeleport) {
                transform.position = target;
                return;
            }

            Vector3 delta = target - transform.position;

            if (Vector3.Dot(transform.forward, delta) < 1) {
                Vector3 forward = Vector3.ProjectOnPlane(delta, transform.up);
                transform.forward = forward;
            }

            controller.current?.PerformPassing(controller, operation, m_last);

            /*Vector3 delta = target - transform.position;
            float magnitude = delta.magnitude;
            float progress = 0;

            if (magnitude < 1e-6) {
                transform.position = target;
                progress = 1;
            } else {
                float length = Mathf.Min(magnitude, operation.Speed);
                transform.position += delta * (length / magnitude);

                float distance = Vector3.Distance(m_last.GetNodePoint(), target);
                float remain = Vector3.Distance(transform.position, target);

                progress = Mathf.Clamp(1 - remain / distance, 0f, 1f);
            }

            if (transform.up != node.transform.up) {
                Vector3 up = Vector3.Slerp(m_last.transform.up, node.transform.up, progress);
                Vector3 forward = Vector3.ProjectOnPlane(delta, up);
                Quaternion rotation = Quaternion.LookRotation(forward, up);
                transform.rotation = rotation;
            }*/
        }

        public bool Arrive(Transform transform) {
            for (int i = index; i < m_moves.Count; i++) {
                if (!m_moves[i].Path.active) {
                    m_moves.RemoveRange(i, m_moves.Count - i);
                    break;
                }
            }
            return ((transform.position - Last().Target).sqrMagnitude < 0.01f);
        }

        public NavigateOperation Last() {
            return m_moves[^1];
        }

        public BaseNode LastNode() {
            return m_last;
        }

        public BaseNode CurrentNode() {
            return m_current;
        }

        public int Count => m_moves.Count;

        public NavigateOperation this[int index] => m_moves[index];
    }
}