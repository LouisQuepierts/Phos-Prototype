using Phos.Navigate.Behaviour;
using System;
using Navigate.Behaviour;

namespace Phos.Navigate {
    [System.Serializable]
    public enum NodeType {
        GROUND,
        STAIR,
        CLIMB,
        CURVE,
        DOOR
    }

    public static class NodeTypeExtensions {
        private static readonly INodeBehaviour[] behaviours = new INodeBehaviour[] {
            GroundNodeBehaviour.Instance,
            StairNodeBehaviour.Instance,
            ClimbNodeBehaviour.Instance,
            CurveNodeBehaviour.Instance,
            DoorNodeBehaviour.Instance
        };

        public static INodeBehaviour GetNodeBehaviour(this NodeType type) {
            return behaviours[(int)type];
        }
    }
}
