#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Phos.Navigate {
	[ExecuteInEditMode]
    public class CurveHelper : MonoBehaviour {
        public BaseNode start;
        public BaseNode end;
        public Vector3 offset;

        public int interpolates = 5;

        [SerializeField]
        private bool available;

        public void Refresh() {
            this.available = false;
            if (start == null || end == null) return;

            Transform t1 = start.transform;
            Transform t2 = end.transform;

            Vector3 point1 = t1.position;
            Vector3 point2 = t2.position;

            Ray ray1 = new Ray(t1.position, t1.up);
            Ray ray2 = new Ray(t2.position, t2.up);

            Vector3 center = Vector3.zero;

            if (!RayIntersects(ray1, ray2, out float f)) {
                return;
            }

            center = ray1.GetPoint(f);
            available = true;
            transform.position = center;
        }

        public void Generate(GameObject template) {

            if (Application.isPlaying || !available) return;

            Vector3 dir0 = -start.transform.up;
            Vector3 dir1 = -end.transform.up;

            Vector3 normal = Vector3.Cross(dir0, dir1).normalized;

            Vector3 center = transform.position + offset;

            float distance = Vector3.Distance(start.transform.position, center);
            

            float segment = interpolates + 1;
            for (int i = 1; i < segment; i++) {
                float delta = i / segment;
                Vector3 direction = Vector3.Slerp(dir0, dir1, delta);
                Vector3 point = center + direction * distance;
                Vector3 forward = Vector3.Cross(normal, direction);

                GameObject @object = GameObject.Instantiate(
                    template, point,
                    Quaternion.LookRotation(forward, -direction)
                );
                NavigateNode node = @object.GetComponent<NavigateNode>();
                node.type = NodeType.CURVE;

                @object.transform.SetParent(transform.parent);
            }
        }

        private void OnDrawGizmos() {
            if (Application.isPlaying || !available) return;

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, 0.25f);

            Vector3 dir0 = -start.transform.up;
            Vector3 dir1 = -end.transform.up;

            Vector3 center = transform.position + offset;

            float distance = Vector3.Distance(start.GetNodePoint(), center);

            float segment = interpolates + 1;
            for (int i = 1; i < segment; i++) {
                float delta = i / segment;
                Vector3 direction = Vector3.Slerp(dir0, dir1, delta);
                Vector3 point = center + direction * distance;
                Gizmos.DrawSphere(point, 0.1f);
                Gizmos.DrawRay(point, -direction);
            }
        }

        private static bool RayIntersects(Ray ray1, Ray ray2, out float t) {
            Vector3 d1 = ray1.direction;
            Vector3 d2 = ray2.direction;
            Vector3 r0 = ray1.origin;
            Vector3 r1 = ray2.origin;

            Vector3 cross = Vector3.Cross(d1, d2);
            float denom = cross.sqrMagnitude;

            if (denom > 1e-6f) {
                Vector3 r = r1 - r0;
                t = Vector3.Cross(r, d2).magnitude / denom;
                return true;
            }

            t = 0;
            return false;
        }
    }
}
#endif