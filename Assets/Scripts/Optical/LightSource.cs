using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Phos.Optical {
    [ExecuteAlways]
	public class LightSource : OpticalDevice {
        public float Strength = 10f;
        public float MinIntensity = 0.1f;
        public int MaxBounces = 10;

        private List<LightPath> _paths = new();
        private bool _updated = false;

        public DynamicPathMesh _line;
        
        public new void Awake() {
            base.Awake();
            UpdateLightPaths();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        protected override void OnTransformChanged() {
            UpdateLightPaths();
            
            //if (!Application.isPlaying) return;

            //_line ??= gameObject.GetComponent<LineRenderer>() ?? gameObject.AddComponent<LineRenderer>();
            Debug.Log("Update Light Paths");

            List<Vector3> points = new List<Vector3>(_paths.Count + 1);
            points.Add(transform.position);
            points.AddRange(_paths.Select(t => t.End));
            _line.UpdatePath(_paths);
        }

        private void OnDrawGizmos() {
            if (!debug) return;
            
            if (!_updated) UpdateLightPaths();
            
            Gizmos.color = Color.red;
            foreach (var path in _paths) {
                Gizmos.DrawLine(path.Start, path.End);
            }
        }

        private void UpdateLightPaths() {
            Debug.Log("Update Light Paths");
            _paths.Clear();
            var light = new LightData(
                transform.position,
                transform.forward,
                transform.forward,
                Strength    
            );

            TraceLight(light, MaxBounces);
            _updated = true;
        }

        private void TraceLight(LightData light, int remainingBounces) {
            Stack<LightData> lightStack = new();
            lightStack.Push(light);

            while (lightStack.Count > 0) {
                var current = lightStack.Pop();
                if (current.Intensity < MinIntensity || remainingBounces <= 0) continue;

                if (Math.Abs(current.Direction.magnitude - 1.0) > 1e-6) continue;

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
                    _paths.Add(new LightPath(
                        current,
                        hit.point,
                        hit.normal,
                        _paths.Count
                    ));

                    var acceptor = hit.collider.GetComponent<ILightAcceptable>();
                    if (acceptor == null ||
                        !acceptor.OnLightHitted(
                            new LightData(hit.point, current.Direction, current.Intensity - hit.distance),
                            hit,
                            out List<LightData> lights
                        )) continue;
                    
                    foreach (var newLight in lights) {
                        newLight.Last = current;
                        lightStack.Push(newLight);
                        remainingBounces--;
                    }
                } else {
                    _paths.Add(new LightPath(
                        current,
                        current.StartPoint + current.Direction * current.Intensity,
                        -current.StartPointNormal,
                        _paths.Count
                    ));
                }
            }
        }
    }
}