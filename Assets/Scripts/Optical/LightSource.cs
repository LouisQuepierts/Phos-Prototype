using System.Collections.Generic;
using UnityEngine;

namespace Phos.Optical {
    [ExecuteInEditMode]
	public class LightSource : MonoBehaviour {
        public float Strength = 10f;
        public float MinIntensity = 0.1f;
        public int MaxBounces = 10;

        private List<LightPath> m_paths = new();

        // Update in editor
        private void Update() {
            if (Application.isPlaying) return;
            UpdateLightPaths();
        }

        private void FixedUpdate() {
            // TODO: When transform changed execute
            UpdateLightPaths();
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            foreach (var path in m_paths) {
                Gizmos.DrawLine(path.Start, path.End);
            }
        }

        private void UpdateLightPaths() {
            m_paths.Clear();
            var light = new LightData(
                transform.position,
                transform.forward,
                Strength    
            );

            TraceLight(light, MaxBounces);
        }

        private void TraceLight(LightData light, int remainingBounces) {
            Stack<LightData> lightStack = new();
            lightStack.Push(light);

            while (lightStack.Count > 0) {
                var current = lightStack.Pop();
                if (current.Intensity < MinIntensity || remainingBounces <= 0) continue;

                if (current.Direction.magnitude != 1.0) continue;

                float distance = current.Intensity;
                RaycastHit hit;
                bool hitted;

                if (current.Inside) {
                    Ray ray = new(current.StartPoint + current.Direction * distance, -current.Direction);
                    hitted = current.Collider.Raycast(ray, out hit, distance);
                } else {
                    Ray ray = new(current.StartPoint, current.Direction);
                    hitted = Physics.Raycast(ray, out hit, distance);
                }

                if (hitted) {
                    m_paths.Add(new LightPath(
                        current.StartPoint,
                        hit.point,
                        current.Intensity
                    ));

                    var acceptor = hit.collider.GetComponent<ILightAcceptable>();
                    if (acceptor != null) {
                        if (acceptor.OnLightHitted(
                            new LightData(hit.point, current.Direction, current.Intensity - hit.distance),
                            hit,
                            out List<LightData> lights
                        )) {

                            foreach (var newLight in lights) {
                                lightStack.Push(newLight);
                                remainingBounces--;
                            }

                        }
                    }
                } else {
                    m_paths.Add(new LightPath(
                        current.StartPoint,
                        current.StartPoint + current.Direction * current.Intensity,
                        current.Intensity
                    ));
                }
            }
        }
    }
}