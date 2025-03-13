using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;

namespace Phos.Optical {
    [ExecuteAlways]
	public class Refractor : OpticalDevice, ILightAcceptable {
        public float refractiveIndex = 1.5f;

        [Range(0, 1)] 
        public float intensityLoss = 0.2f;

        public bool OnLightHitted(LightData income, RaycastHit hit, out List<LightData> outgo) {
            Vector3 normal = hit.normal;
            float eta = 1.0f / refractiveIndex;
            bool entering = Vector3.Dot(income.Direction, normal) < 0;

            if (!entering) {
                eta = refractiveIndex;
                normal = -normal;
            }

            Vector3 refractDir = CalculateRefraction(income.Direction, normal, eta);

            Vector3 newStart = hit.point + refractDir * 0.001f;
            outgo = new List<LightData> {
                new LightData(
                    newStart,
                    refractDir,
                    hit.normal,
                    income.Intensity * (1 - intensityLoss),
                    !income.Inside ? GetComponent<Collider>() : null
                )
            };

            return true;
        }

        private Vector3 CalculateRefraction(Vector3 inDir, Vector3 normal, float eta) {
            float cosTheta1 = Vector3.Dot(-inDir, normal);
            float sinTheta1Sq = 1 - cosTheta1 * cosTheta1;
            float sinTheta2Sq = eta * eta * sinTheta1Sq;

            if (sinTheta2Sq > 1) return Vector3.zero;

            float cosTheta2 = Mathf.Sqrt(1 - sinTheta2Sq);
            return eta * inDir + (eta * cosTheta1 - cosTheta2) * normal;
        }
    }
}