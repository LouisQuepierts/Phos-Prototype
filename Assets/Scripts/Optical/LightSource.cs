using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace Phos.Optical {
    [ExecuteAlways]
	public class LightSource : OpticalDevice {
        public float strength = 10f;
        public float minIntensity = 0.1f;
        public int maxBounces = 10;
        
        public Material material;

        private readonly List<LightPath> _paths = new();
        private bool _updated = false;

        private LightBeamMesh _line;

        private float _strength;

        private LightBeamMesh Light {
            get {
                if (!_line) {
                    _line = CreateLightBeam();
                }
                return _line;
            }
        }
        
        public new void Awake() {
            base.Awake();
            _strength = strength;
            
            UpdateLightPaths();
        }

        private LightBeamMesh CreateLightBeam() {
            var obj = new GameObject("LightBeam");
            var mesh = obj.AddComponent<LightBeamMesh>();
            mesh.material = material;
            mesh.Init();

            var meshRenderer = obj.GetComponent<MeshRenderer>();
            meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            meshRenderer.receiveShadows = false;
            meshRenderer.lightProbeUsage = LightProbeUsage.Off;
            return mesh;
        }

        protected override bool IsChanged() {
            if (Mathf.Approximately(_strength, strength)) return base.IsChanged();
            
            _strength = strength;
            return true;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public override void OpticalUpdate() {
            UpdateLightPaths();
            
            //if (!Application.isPlaying) return;

            //_line ??= gameObject.GetComponent<LineRenderer>() ?? gameObject.AddComponent<LineRenderer>();
            Debug.Log("Update Light Paths");

            List<Vector3> points = new List<Vector3>(_paths.Count + 1);
            points.Add(transform.position);
            points.AddRange(_paths.Select(t => t.End));
            Light.UpdatePath(_paths);
        }

        private void OnDrawGizmos() {
            if (!Manager.debug) return;
            
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
                strength    
            );

            TraceLight(light, maxBounces);
            _updated = true;
        }

        private void TraceLight(LightData light, int remainingBounces) {
            Stack<LightData> lightStack = new();
            lightStack.Push(light);

            while (lightStack.Count > 0) {
                var current = lightStack.Pop();
                if (current.Intensity < minIntensity || remainingBounces <= 0) continue;

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
                        _paths.Count
                    ));

                    var acceptor = hit.collider.GetComponent<ILightAcceptable>();
                    if (acceptor == null ||
                        !acceptor.OnLightHitted(
                            new LightData(hit.point, current.Direction, current.Intensity, current.Collider),
                            hit,
                            out List<LightData> lights
                        )) continue;
                    
                    foreach (var newLight in lights) {
                        newLight.Last = current;
                        lightStack.Push(newLight);
                    }
                    remainingBounces--;
                } else {
                    _paths.Add(new LightPath(
                        current,
                        current.StartPoint + current.Direction * current.Intensity,
                        _paths.Count
                    ));
                }
            }
        }
    }
}