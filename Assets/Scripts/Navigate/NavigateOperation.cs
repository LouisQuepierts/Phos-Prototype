using UnityEngine;

namespace Phos.Navigate {
    public class NavigateOperation {
        public BaseNode Node { get; private set; }
        public NodePath Path { get; private set; }
        public bool IsTeleport { get; private set; }
        public float Speed { get; private set; }

        public Vector3 NodePosition => Node.GetNodePosition();
        public Vector3 Target => (_router ?? Node).GetNodePosition();
        public bool IsRouter => _router != null;

        private BaseNode _router;
        
        public NavigateOperation Init(BaseNode node, NodePath path, bool isTeleport = false, Direction direction = Direction.None) {
            Node = node;
            Path = path;
            IsTeleport = isTeleport;
            Speed = .1f;
            _router = direction == Direction.None ? null : node.GetConnectionPoint(direction);
            return this;
        }
    }
}