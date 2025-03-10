using UnityEngine;

namespace Phos.Optical {
	public class LightPath {
        public Vector3 Start;
        public Vector3 End;
        public float Intensity;

        public LightPath(Vector3 start, Vector3 end, float intensity) {
            Start = start;
            End = end;
            Intensity = intensity;
        }
    }
}