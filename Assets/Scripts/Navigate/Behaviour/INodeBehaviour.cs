using System.Collections.Generic;
using UnityEngine;

namespace Phos.Navigate {
	public interface INodeBehaviour {
        Vector3 GetLocalConnectPoint(Direction direction, float offset = 0.0f);

        Vector3 GetLocalOffset(float offset = 0.0f);

        Vector3 GetNodePoint(Transform transform, float offset = 0.0f);
        
        Vector3 GetNodeNormal(Transform transform) {
            return transform.up;
        }

        Vector3 GetRelativeConnectPoint(Transform transform, Direction direction, float offset = 0.0f);

        void PerformPassing(PlayerController controller, NavigateOperation operation, BaseNode last) {
            Transform transform = controller.transform;
            Vector3 target = operation.Target;
            Vector3 delta = target - transform.position;

            float magnitude = delta.magnitude;

            if (magnitude < 1e-6) {
                transform.position = target;
            } else {
                float length = Mathf.Min(magnitude, controller.speed);
                transform.position += delta * (length / magnitude);
            }

            Vector3 up = operation.Node.transform.up;
            if (Mathf.Approximately(Vector3.Dot(transform.up, up), 1.0f)) return;
            
            if (Mathf.Approximately(Vector3.Dot(last.transform.up, up), 1.0f)) {
                Vector3 forward = Vector3.ProjectOnPlane(delta, up);
                transform.rotation = Quaternion.LookRotation(forward, up);
                return;
            }
            
            Vector3 dest = operation.NodePosition;
            float distance = Vector3.Distance(last.GetNodePosition(), dest);
            float remain = Vector3.Distance(transform.position, dest);

            var progress = Mathf.Clamp(1 - remain / distance, 0f, 1f);

            Vector3 lup = Vector3.Slerp(last.transform.up, operation.Node.transform.up, progress);
            Vector3 lforward = Vector3.ProjectOnPlane(delta, lup).normalized;
            Quaternion rotation = Quaternion.LookRotation(lforward, lup);
            transform.rotation = rotation;
        }

        IReadOnlyList<Direction> GetAvailableDirections();
    }
}