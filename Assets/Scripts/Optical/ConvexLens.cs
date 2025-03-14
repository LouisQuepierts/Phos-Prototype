using System.Collections.Generic;
using UnityEngine;

namespace Phos.Optical {
    public class ConvexLens : BaseLens {
        private Vector3 ProcessFirstRefraction(Vector3 inDir, Vector3 normal) {
            float eta = 1.0f / RefractiveIndex;
            return CalculateRefraction(inDir, normal, eta).normalized;
        }

        private bool TraceThroughLens(Vector3 entryPoint, Vector3 dir, 
            out Vector3 exitPoint, out Vector3 exitNormal) {
        
            exitPoint = entryPoint + dir * LensThickness;
            exitNormal = -transform.forward;
        
            return true;
        }

        private Vector3 ProcessSecondRefraction(Vector3 inDir, Vector3 normal) {
            float eta = RefractiveIndex;
            return CalculateRefraction(inDir, normal, eta).normalized;
        }

        private LightData CreateExitLight(LightData original, Vector3 point, Vector3 dir) {
            return new LightData(
                point + dir * 0.001f,
                dir,
                original.Intensity * 0.9f,
                original.Width
            );
        }

        public override void OnLightHitted(LightData income, RaycastHit hit, out List<LightData> outgo) {
            outgo = new List<LightData>();
            Vector3 exitPoint;
            Vector3 exitDir;
        
            Vector3 entryDir = ProcessFirstRefraction(income.Direction, hit.normal);

            if (!TraceThroughLens(hit.point, entryDir, out exitPoint, out Vector3 exitNormal)) return;
            
            exitDir = ProcessSecondRefraction(entryDir, exitNormal);
            outgo.Add(CreateExitLight(income, exitPoint, exitDir));

        }
    }
}