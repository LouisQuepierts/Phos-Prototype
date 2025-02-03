using Unity.VisualScripting;
using UnityEngine;

namespace Phos.Navigate {
	public class VirtualNode : BaseNode {
        private NavigateNode m_bound;

        [HideInInspector]
        public NavigateNode Bound { get {
                if (m_bound == null || m_bound.IsDestroyed()) {
                    NavigateNode parent = GetComponentInParent<NavigateNode>();

                    if (parent == null) {
                        throw new System.Exception("VirtualNode must be a child of VirtualNode");
                    }

                    m_bound = parent;
                    type = m_bound.type;
                    offset = m_bound.offset;
                }
                return m_bound;
            }
        }

        public Direction direction;

#if UNITY_EDITOR
        private void OnDrawGizmos() {
            if (Bound == null) return;
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(GetNodePoint(), 0.1f);
        }
#endif
    }
}