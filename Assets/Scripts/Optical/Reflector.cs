using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Phos.Optical {
    [ExecuteAlways]
    public class Reflector : OpticalDevice, ILightAcceptable {
        [Range(0, 1)] 
        public float intensityLoss = 0.1f;

        public void OnLightHitted(LightData income, RaycastHit hit, out List<LightData> outgo) {
            Vector3 reflectDirection = Vector3.Reflect(income.Direction, hit.normal);
            Vector3 start = hit.point + reflectDirection * 0.001f;
            LightData data = new LightData(
                start,
                reflectDirection,
                -hit.normal,
                income.Intensity * (1 - intensityLoss),
                income.Width,
                null,
                false
            );
            outgo = new List<LightData> { data };
        }
    }
}