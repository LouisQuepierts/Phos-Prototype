using UnityEngine;

namespace Phos.Optical {
	public class LightData {
        public Vector3 StartPoint;
        public Vector3 Direction;
        public float Intensity;
        public Collider Collider;

        public bool Inside => Collider != null;

        public LightData(Vector3 start, Vector3 dir, float intensity, Collider collider = null) {
            StartPoint = start;
            Direction = dir.normalized;
            Intensity = intensity;
            Collider = collider;
        }
    }
}