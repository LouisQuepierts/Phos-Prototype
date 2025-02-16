using UnityEngine;
using UnityEngine.InputSystem.Utilities;

namespace Phos.Navigate {
	public interface INodeBehaviour {
        Vector3 GetLocalConnectPoint(Direction direction, float offset = 0.0f);

        Vector3 GetLocalOffset(float offset = 0.0f);

        Vector3 GetNodePoint(Transform transform, float offset = 0.0f);

        Vector3 GetRelativeConnectPoint(Transform transform, Direction direction, float offset = 0.0f);

        ReadOnlyArray<Direction> GetAvailableDirections();
	}
}