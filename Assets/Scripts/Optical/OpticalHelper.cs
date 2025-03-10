using UnityEngine;

namespace Phos.Optical {
	public static class OpticalHelper {
		public static Vector3 Reflect(Transform reflector, Vector3 @in) {
			Vector3 normal = reflector.forward;
			return @in - 2 * Vector3.Dot(@in, normal) * normal;
		}
	}
}