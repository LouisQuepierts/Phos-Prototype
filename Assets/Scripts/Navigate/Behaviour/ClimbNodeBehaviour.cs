using System.Collections.Generic;
using UnityEngine;

namespace Phos.Navigate.Behaviour {
    public class ClimbNodeBehaviour : INodeBehaviour {
        public static readonly ClimbNodeBehaviour Instance = new();

        private ClimbNodeBehaviour() { }

        public IReadOnlyList<Direction> GetAvailableDirections() {
            throw new System.NotImplementedException();
        }

        public Vector3 GetLocalConnectPoint(Direction direction, float offset = 0) {
            throw new System.NotImplementedException();
        }

        public Vector3 GetLocalOffset(float offset = 0) {
            throw new System.NotImplementedException();
        }

        public Vector3 GetNodePoint(Transform transform, float offset = 0) {
            throw new System.NotImplementedException();
        }

        public Vector3 GetNodeNormal(Transform transform) {
            throw new System.NotImplementedException();
        }

        public Vector3 GetRelativeConnectPoint(Transform transform, Direction direction, float offset = 0) {
            throw new System.NotImplementedException();
        }

        public void PerformPassing(PlayerController controller, NavigateOperation operation, BaseNode last) {
            throw new System.NotImplementedException();
        }
    }
}