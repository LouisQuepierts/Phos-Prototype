using UnityEngine;

namespace Phos.Optical {
	public class LightData {
        public readonly Vector3 StartPoint;
        public readonly Vector3 StartPointNormal;
        
        public readonly Vector3 Direction;
        public readonly float Intensity;
        public readonly Collider Collider;
        
        public LightData Last;
        public LightPath Path;

        public bool IsRoot => Last == null;
        public bool Continuously => !IsRoot && Vector3.Dot(Direction, Last.Direction) > 0;
        public bool Inside => Collider != null;

        public LightData(Vector3 start, Vector3 dir, float intensity, Collider collider = null) {
            StartPoint = start;
            StartPointNormal = Vector3.forward;
            Direction = dir.normalized;
            Intensity = intensity;
            Collider = collider;
        }
        
        public LightData(Vector3 start, Vector3 dir, Vector3 normal, float intensity, Collider collider = null) {
            StartPoint = start;
            StartPointNormal = normal;
            Direction = dir.normalized;
            Intensity = intensity;
            Collider = collider;
        }
    }
}