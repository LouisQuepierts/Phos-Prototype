using Phos.Navigate.Behaviour;
using System;

namespace Phos.Navigate {
    [System.Serializable]
    public enum NodeType {
        GROUND,
        STAIR,
        CLIMB,
        CURVE
    }

    public static class NodeTypeExtensions {
        private static readonly INodeBehaviour[] behaviours = new INodeBehaviour[] {
            GroundNodeBehaviour.Instance,
            StairNodeBehaviour.Instance,
            ClimbNodeBehaviour.Instance,
            CurveNodeBehaviour.Instance
        };

        public static INodeBehaviour GetNodeBehaviour(this NodeType type) {
            return behaviours[(int)type];
        }
    }
}
