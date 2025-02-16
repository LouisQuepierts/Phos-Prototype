using UnityEngine;
using UnityEngine.InputSystem.Utilities;

namespace Phos.Navigate.Behaviour {
    public class ClimbNodeBehaviour : INodeBehaviour {
        public static readonly ClimbNodeBehaviour Instance = new();

        private ClimbNodeBehaviour() { }

        public ReadOnlyArray<Direction> GetAvailableDirections() {
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

        public Vector3 GetRelativeConnectPoint(Transform transform, Direction direction, float offset = 0) {
            throw new System.NotImplementedException();
        }
    }
}