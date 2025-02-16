
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

namespace Phos.Navigate {
    public abstract class BaseNode : MonoBehaviour {
        public const float BLOCK_SCALE = 0.5f;

        public NodeType type = NodeType.GROUND;
        public float offset = 0.0f;

        public Vector3 GetNodePoint() {
            return type.GetNodeBehaviour().GetNodePoint(transform, offset);
            //return type switch {
            //    NodeType.GROUND => transform.position + transform.up * (offset + 0.5f),
            //    NodeType.STAIR => transform.position + transform.up * offset,
            //    NodeType.CLIMB => transform.position + transform.forward * (offset + 0.5f),
            //    _ => throw new NotImplementedException(),
            //};
        }

        public Vector3 GetLocalOffset() {
            return type.GetNodeBehaviour().GetLocalOffset(offset);
            //return type switch {
            //    NodeType.GROUND => Vector3.up * (offset + 0.5f),
            //    NodeType.STAIR => Vector3.up * offset,
            //    NodeType.CLIMB => Vector3.forward * (offset + 0.5f),
            //    _ => throw new NotImplementedException(),
            //};
        }

        public Vector3 GetRelativeConnectPoint(Direction direction, float offset = 0.0f) {
            return type.GetNodeBehaviour().GetRelativeConnectPoint(transform, direction, offset);
            //float mul = (BLOCK_SCALE + offset);
            //return type switch {
            //    NodeType.GROUND => direction switch {
            //        Direction.Forward => transform.forward * mul,
            //        Direction.Backward => transform.forward * -mul,
            //        Direction.Right => transform.right * mul,
            //        Direction.Left => transform.right * -mul,
            //        _ => throw new NotImplementedException()
            //    },
            //    NodeType.STAIR => direction switch {
            //        Direction.Forward => (transform.forward + transform.up) * mul,
            //        Direction.Backward => (transform.forward + transform.up) * -mul,
            //        Direction.Right => transform.right * mul,
            //        Direction.Left => transform.right * -mul,
            //        _ => throw new NotImplementedException()
            //    },
            //    NodeType.CLIMB => direction switch {
            //        Direction.Forward => transform.up * mul,
            //        Direction.Backward => transform.up * -mul,
            //        Direction.Right => transform.right * mul,
            //        Direction.Left => transform.right * -mul,
            //        _ => throw new NotImplementedException()
            //    },
            //    _ => throw new NotImplementedException(),
            //};
        }

        public Vector3 GetLocalConnectPoint(Direction direction, float offset = 0.0f) {
            return type.GetNodeBehaviour().GetLocalConnectPoint(direction, offset) + GetLocalOffset();
            //float mul = (BLOCK_SCALE + offset);
            //return type switch {
            //    NodeType.GROUND => direction switch {
            //        Direction.Forward => Vector3.forward * mul,
            //        Direction.Backward => Vector3.forward * -mul,
            //        Direction.Right => Vector3.right * mul,
            //        Direction.Left => Vector3.right * -mul,
            //        _ => throw new NotImplementedException()
            //    },
            //    NodeType.STAIR => direction switch {
            //        Direction.Forward => (Vector3.forward + Vector3.up) * mul,
            //        Direction.Backward => (Vector3.forward + Vector3.up) * -mul,
            //        Direction.Right => Vector3.right * mul,
            //        Direction.Left => Vector3.right * -mul,
            //        _ => throw new NotImplementedException()
            //    },
            //    NodeType.CLIMB => direction switch {
            //        Direction.Forward => Vector3.up * mul,
            //        Direction.Backward => Vector3.up * -mul,
            //        Direction.Right => Vector3.right * mul,
            //        Direction.Left => Vector3.right * -mul,
            //        _ => throw new NotImplementedException()
            //    },
            //    _ => throw new NotImplementedException(),
            //} + GetLocalOffset();
        }

        public Vector3 GetConnectPoint(Direction direction, float offset = 0.0f) {
            return GetNodePoint() + GetRelativeConnectPoint(direction, offset);
        }

        public Direction GetSimilarDirection(Vector3 position) {
            Direction similar = AvailableDirections[0];
            float distance = float.PositiveInfinity;
            foreach (Direction direction in AvailableDirections) {
                Vector3 point = GetConnectPoint(direction);

                if (Vector3.SqrMagnitude(point - position) < distance) {
                    similar = direction;
                    distance = Vector3.SqrMagnitude(point - position);
                }
            }
            return similar;
        }

        public abstract BaseNode GetConnectedNode(Direction direction);

        protected ReadOnlyArray<Direction> AvailableDirections {
            get { return this.type.GetNodeBehaviour().GetAvailableDirections(); }
        }
    }
}
