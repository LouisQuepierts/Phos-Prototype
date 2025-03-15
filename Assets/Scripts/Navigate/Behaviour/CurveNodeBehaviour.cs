using System;
using System.Collections.Generic;
using UnityEngine;

namespace Phos.Navigate.Behaviour {
    public class CurveNodeBehaviour : INodeBehaviour {
        public static readonly CurveNodeBehaviour Instance = new();
        private static readonly IReadOnlyList<Direction> AvailableDirections = new List<Direction>(new Direction[] {
            Direction.Forward, Direction.Backward
        });

        private CurveNodeBehaviour() { }

        public IReadOnlyList<Direction> GetAvailableDirections() {
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

        // public void PerformPassing(PlayerController controller, NavigateOperation operation, BaseNode last) {
        //     Transform transform = controller.transform;
        //     Vector3 target = operation.Target;
        //
        //     Vector3 delta = target - transform.position;
        //     float magnitude = delta.magnitude;
        //
        //     if (magnitude < 1e-6) {
        //         transform.position = target;
        //     } 
        //     
        //     float length = Mathf.Min(magnitude, operation.Speed);
        //     transform.position += delta * (length / magnitude);
        // }
    }
}