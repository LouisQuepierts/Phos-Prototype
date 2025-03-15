using UnityEngine;

namespace Phos.Navigate {
    public class VirtualNode : BaseNode {
        private NavigateNode m_bound;

        public Direction direction;

        public NavigateNode Bound { get {
                if (m_bound && m_bound.enabled) return m_bound;
                
                NavigateNode parent = GetComponentInParent<NavigateNode>();

                if (!parent) {
                    throw new System.Exception("VirtualNode must be a child of NavigateNode");
                }

                m_bound = parent;
                type = m_bound.type;
                offset = m_bound.offset;
                return m_bound;
            }
        }

        public override BaseNode GetConnectedNode(Direction direction) {
            return m_bound.GetConnectedNode(direction);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos() {
            if (!NavigateNode.ShowGizmos || Bound == null) return;
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(GetNodePosition(), 0.1f);
        }
#endif
    }
}