using System;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

namespace Phos.Navigate.Behaviour {
    public class CurveNodeBehaviour : INodeBehaviour {
        public static readonly CurveNodeBehaviour Instance = new();
        private static readonly ReadOnlyArray<Direction> AvailableDirections = new ReadOnlyArray<Direction>(new Direction[] {
            Direction.Forward, Direction.Backward
        });

        private CurveNodeBehaviour() { }

        public ReadOnlyArray<Direction> GetAvailableDirections() {
            return AvailableDirections;
        }

        public Vector3 GetLocalConnectPoint(Direction direction, float offset = 0) {
            float mul = (BaseNode.BLOCK_SCALE + offset);
            return direction switch {
                Direction.Forward => (Vector3.forward) * mul,
                Direction.Backward => (Vector3.forward) * -mul,
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
                Direction.Forward => (transform.forward) * mul,
                Direction.Backward => (transform.forward) * -mul,
                _ => throw new NotImplementedException()
            };
        }
    }
}