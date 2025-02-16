using System;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

namespace Phos.Navigate.Behaviour {
    public class GroundNodeBehaviour : INodeBehaviour {
        public static readonly GroundNodeBehaviour Instance = new();

        private GroundNodeBehaviour() { }

        public ReadOnlyArray<Direction> GetAvailableDirections() {
            return Directions.Value;
        }

        public Vector3 GetLocalConnectPoint(Direction direction, float offset = 0) {
            float mul = (BaseNode.BLOCK_SCALE + offset);
            return direction switch {
                Direction.Forward => Vector3.forward * mul,
                Direction.Backward => Vector3.forward * -mul,
                Direction.Right => Vector3.right * mul,
                Direction.Left => Vector3.right * -mul,
                _ => throw new NotImplementedException()
            };
        }

        public Vector3 GetLocalOffset(float offset) {
            return Vector3.up * (offset + 0.5f);
        }

        public Vector3 GetNodePoint(Transform transform, float offset = 0) {
            return transform.position + transform.up * (offset + 0.5f);
        }

        public Vector3 GetRelativeConnectPoint(Transform transform, Direction direction, float offset = 0) {
            float mul = (BaseNode.BLOCK_SCALE + offset);
            return direction switch {
                Direction.Forward => transform.forward * mul,
                Direction.Backward => transform.forward * -mul,
                Direction.Right => transform.right * mul,
                Direction.Left => transform.right * -mul,
                _ => throw new NotImplementedException()
            };
        }
    }
}