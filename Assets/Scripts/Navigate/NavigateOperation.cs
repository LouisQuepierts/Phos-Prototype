using UnityEngine;

namespace Phos.Navigate {
    public class NavigateOperation {
        public BaseNode Node { get; }
        public bool IsTeleport { get; }
        public float Speed { get; }

        public Vector3 Target { get { return Node.GetNodePoint(); } }
        public Vector3 Up { get { return Node.transform.up; } }

        public NavigateOperation(BaseNode node, bool isTeleport = false, float speed = .1f) {
            Node = node;
            IsTeleport = isTeleport;
            Speed = speed;
        }
    }
}