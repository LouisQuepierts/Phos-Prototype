using System.Collections.Generic;
using Phos.Callback;
using Phos.Trigger;
using UnityEngine;

namespace Phos.Navigate {
    public abstract class BaseNode : CallbackProvider<BaseNode> {
        public const float BLOCK_SCALE = 0.5f;

        public NodeType type = NodeType.GROUND;
        public float offset;

        public virtual Vector3 GetNodePosition() {
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

        public Vector3 GetRelativeConnectionPosition(Direction direction, float offset = 0.0f) {
            return type.GetNodeBehaviour().GetRelativeConnectPoint(transform, direction, offset);
        }

        public Vector3 GetLocalConnectionPosition(Direction direction, float offset = 0.0f) {
            return type.GetNodeBehaviour().GetLocalConnectPoint(direction, offset) + GetLocalOffset();
        }

        public Vector3 GetConnectionPosition(Direction direction, float offset = 0.0f) {
            return GetNodePosition() + GetRelativeConnectionPosition(direction, offset);
        }

        public virtual BaseNode GetConnectionPoint(Direction direction) {
            return this;
        }

        public void OnArrive(Transform transform) {
            ArriveNodeTriggerBehaviour.Trigger(this, transform);
        }

        public Direction GetSimilarDirection(Vector3 position) {
            Direction similar = AvailableDirections[0];
            float distance = float.PositiveInfinity;
            foreach (Direction direction in AvailableDirections) {
                Vector3 point = GetConnectionPosition(direction);

                if (Vector3.SqrMagnitude(point - position) < distance) {
                    similar = direction;
                    distance = Vector3.SqrMagnitude(point - position);
                }
            }

            return similar;
        }

        public void PerformPassing(PlayerController controller, NavigateOperation operation, BaseNode last) {
            type.GetNodeBehaviour().PerformPassing(controller, operation, last);
        }

        public abstract BaseNode GetConnectedNode(Direction direction);

        public IReadOnlyList<Direction> AvailableDirections {
            get { return this.type.GetNodeBehaviour().GetAvailableDirections(); }
        }
    }
}
