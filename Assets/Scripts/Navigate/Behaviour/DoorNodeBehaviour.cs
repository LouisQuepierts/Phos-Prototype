using System;
using System.Collections.Generic;
using Phos.Navigate;
using UnityEngine;

namespace Navigate.Behaviour {
    public class DoorNodeBehaviour : INodeBehaviour {
        public static readonly DoorNodeBehaviour Instance = new();
        private static readonly IReadOnlyList<Direction> AvailableDirections = new List<Direction>(new Direction[] {
            Direction.Forward, Direction.Backward
        });
        
        public Vector3 GetLocalConnectPoint(Direction direction, float offset = 0) {
            float mul = (BaseNode.BLOCK_SCALE + offset);
            return direction switch {
                Direction.Forward => Vector3.forward * (mul * 0.5f),
                Direction.Backward => Vector3.forward * -mul,
                _ => Vector3.zero
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
                Direction.Forward => transform.forward * (mul * 0.5f),
                Direction.Backward => transform.forward * -mul,
                _ => Vector3.zero
            };
        }
        
        public IReadOnlyList<Direction> GetAvailableDirections() {
            return AvailableDirections;
        }
    }
}