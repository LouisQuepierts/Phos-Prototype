using UnityEngine;
using UnityEngine.Analytics;

namespace Phos.Optical {
	public class LightData {
        public readonly Vector3 StartPoint;
        public readonly Vector3 StartPointNormal;
        
        public readonly Vector3 Direction;
        public readonly float Intensity;
        public readonly Collider Collider;
        
        public float AbsorptionCoefficient = 0.1f;
        public float ScatteringCoefficient = 0.05f;
        public float IntensityThreshold = 0.01f;

        public readonly bool Continuous;
        
        public LightData Last;
        public LightPath Path;

        public bool IsRoot => Last == null;
        public bool IsContinuous => !IsRoot && Continuous;
        public bool Inside => Collider != null;

        public LightData(Vector3 start, Vector3 dir, float intensity, Collider collider = null, bool continuous = true) {
            StartPoint = start;
            StartPointNormal = Vector3.forward;
            Direction = dir.normalized;
            Intensity = intensity;
            Collider = collider;
            Continuous = continuous;
        }
        
        public LightData(Vector3 start, Vector3 dir, Vector3 normal, float intensity, Collider collider = null, bool continuous = true) {
            StartPoint = start;
            StartPointNormal = normal;
            Direction = dir.normalized;
            Intensity = intensity;
            Collider = collider;
            Continuous = continuous;
        }

        public bool Raycast(out RaycastHit hit) {
            float distance = CalculateMaxDistance();

            if (Inside) {
                Ray ray = new Ray(StartPoint + Direction * distance, -Direction);
                return Collider.Raycast(ray, out hit, distance);
            }
            else {
                Ray ray = new Ray(StartPoint, Direction);
                return Physics.Raycast(ray, out hit, distance);
            }
        }
        
        public float CalculateMaxDistance() {
            var totalAttenuation = AbsorptionCoefficient + ScatteringCoefficient;
        
            if (totalAttenuation <= 0 || Intensity <= 0.01f) {
                return 0f;
            }

            var distance = -Mathf.Log(0.01f / Intensity) / totalAttenuation;
            return distance;
        }
        
        public float CalculateIntensityAtDistance(float x) {
            float totalAttenuation = AbsorptionCoefficient + ScatteringCoefficient;
            float intensity = Intensity * Mathf.Exp(-totalAttenuation * x);
            return Mathf.Max(intensity, 0f); // 确保非负
        }
    }
}