using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Phos.Optical {
    [ExecuteAlways]
    public class Splitter : OpticalDevice, ILightAcceptable {
        [Range(0, 1)] 
        public float intensityLoss = 0.1f;
        
        public void OnLightHitted(LightData income, RaycastHit hit, out List<LightData> outgo) {
            Vector3 reflectDirection = Vector3.Reflect(income.Direction, hit.normal);

            float intensity = income.Intensity * (1 - intensityLoss);
            LightData reflected = new LightData(
                hit.point + reflectDirection * 0.001f,
                reflectDirection,
                -hit.normal,
                intensity,
                income.Width,
                null,
                false
            );
            LightData transmitted = new LightData(
                income.StartPoint,
                income.Direction,
                -hit.normal,
                intensity,
                income.Width
            );
            outgo = new List<LightData> { transmitted, reflected };
        }
    }
}