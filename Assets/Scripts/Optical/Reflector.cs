using System.Collections.Generic;
using UnityEngine;

namespace Phos.Optical {
    public class Reflector : MonoBehaviour, ILightAcceptable {
        [Range(0, 1)] 
        public float IntensityLoss = 0.1f;

        public bool OnLightHitted(LightData income, RaycastHit hit, out List<LightData> outgo) {
            Vector3 reflectDirection = Vector3.Reflect(income.Direction, hit.normal);
            Vector3 start = hit.point + reflectDirection * 0.001f;
            LightData data = new LightData(
                start,
                reflectDirection,
                income.Intensity * (1 - IntensityLoss)
            );
            outgo = new List<LightData> { data };
            return true;
        }
    }
}