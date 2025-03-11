using System.Collections.Generic;
using UnityEngine;

namespace Phos.Optical {
	public class LightPath {
        public readonly int Index;
        
        public readonly LightData Light;
        public readonly Vector3 Start;
        public readonly Vector3 End;

        public Vector3 StartNormal;
        public Vector3 EndNormal;

        public float Intensity => Light.Intensity;


        public LightPath(LightData light, Vector3 end, Vector3 endNormal, int index) {
            Index = index;
            Light = light;
            Start = light.StartPoint;
            StartNormal = light.StartPointNormal;

            End = end;
            EndNormal = endNormal;

            light.Path = this;
        }
    }
}