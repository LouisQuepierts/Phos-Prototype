using UnityEngine;

namespace Phos.Optical {
	public class ConcaveMirror : MonoBehaviour {
        public Light globalLight;

        public float curvatureRadius; 
        
        private Vector3 FocusPoint {
            get {
                float focalLength = curvatureRadius / 2;
                return transform.position + transform.forward * focalLength;
            }
        }

        private void OnEnable() {
            if (globalLight == null) {
                enabled = false;
            }
        }


        private void OnDrawGizmos() {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, transform.forward);

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(FocusPoint, 0.1f);

            if (globalLight != null) {
                bool isLightFacingMirror = Vector3.Dot(transform.forward, -globalLight.transform.forward) > 0.9f;

                Gizmos.color = isLightFacingMirror ? Color.red : Color.gray;

                Vector3 lightDir = -globalLight.transform.forward;
                Vector3 startPoint = FocusPoint - lightDir * 5;
                Gizmos.DrawLine(startPoint, FocusPoint);
                Gizmos.DrawRay(FocusPoint, transform.forward * 2);
            }
        }
    }
}