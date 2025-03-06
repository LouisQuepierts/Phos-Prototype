using System.Collections.Generic;
using UnityEngine;

namespace Phos.Navigate {
	public interface INodeBehaviour {
        Vector3 GetLocalConnectPoint(Direction direction, float offset = 0.0f);

        Vector3 GetLocalOffset(float offset = 0.0f);

        Vector3 GetNodePoint(Transform transform, float offset = 0.0f);

        Vector3 GetRelativeConnectPoint(Transform transform, Direction direction, float offset = 0.0f);

        void PerformPassing(PlayerController controller, NavigateOperation operation, BaseNode last) {
            Transform transform = controller.transform;
            Vector3 target = operation.Target;
            Vector3 delta = target - transform.position;

            float magnitude = delta.magnitude;

            Vector3 up = operation.Node.transform.up;
            if (up != transform.up) {
                Vector3 forward = Vector3.ProjectOnPlane(delta, up);
                transform.rotation = Quaternion.LookRotation(forward, up);
            }

            if (magnitude < 1e-6) {
                transform.position = target;
            } else {
                float length = Mathf.Min(magnitude, operation.Speed);
                transform.position += delta * (length / magnitude);
            }
        }

        IReadOnlyList<Direction> GetAvailableDirections();
	}
}