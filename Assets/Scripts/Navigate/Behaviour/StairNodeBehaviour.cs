using System;
using System.Collections.Generic;
using UnityEngine;

namespace Phos.Navigate.Behaviour {
    public class StairNodeBehaviour : INodeBehaviour {
        public static readonly StairNodeBehaviour Instance = new();
        private static readonly IReadOnlyList<Direction> AvailableDirections = new List<Direction>(new Direction[] {
            Direction.Forward, Direction.Backward
        });

        private StairNodeBehaviour() { }

        public IReadOnlyList<Direction> GetAvailableDirections() {
            return AvailableDirections;
        }

        public Vector3 GetLocalConnectPoint(Direction direction, float offset = 0) {
            float mul = (BaseNode.BLOCK_SCALE + offset);
            return direction switch {
                Direction.Forward => (Vector3.forward + Vector3.up) * mul,
                Direction.Backward => (Vector3.forward + Vector3.up) * -mul,
                _ => throw new NotImplementedException()
            };
        }

        public Vector3 GetLocalOffset(float offset) {
            return Vector3.up * offset;
        }

        public Vector3 GetNodePoint(Transform transform, float offset = 0) {
            return transform.position + transform.up * offset;
        }

        public Vector3 GetRelativeConnectPoint(Transform transform, Direction direction, float offset = 0) {
            float mul = (BaseNode.BLOCK_SCALE + offset);
            return direction switch {
                Direction.Forward => (transform.forward + transform.up) * mul,
                Direction.Backward => (transform.forward + transform.up) * -mul,
                _ => throw new NotImplementedException()
            };
        }
    }
}