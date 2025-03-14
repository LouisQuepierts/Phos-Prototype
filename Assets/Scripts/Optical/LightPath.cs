using System.Collections.Generic;
using UnityEngine;

namespace Phos.Optical {
	public class LightPath {
        public readonly int Index;
        
        public readonly LightData Light;
        public readonly Vector3 Start;
        public readonly Vector3 End;
        public readonly float Distance;

        public float StartWidth;
        public float EndWidth;
        
        public Vector3 StartNormal;

        public float Intensity => Light.Intensity;

        public LightPath(LightData light, Vector3 end, int index) {
            Index = index;
            Light = light;
            Start = light.StartPoint;
            StartNormal = light.StartPointNormal;
            Distance = Vector3.Distance(Start, end);

            StartWidth = EndWidth = light.Width;

            End = end;

            light.Path = this;
        }
    }
}