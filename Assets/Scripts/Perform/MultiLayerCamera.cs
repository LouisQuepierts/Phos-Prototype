using System;
using UnityEngine;

namespace Phos.Perform {
    [RequireComponent(typeof(Camera))]
    public class MultiLayerCamera : MonoBehaviour {
        [Range(1, 5)]
        public int layers;
        
        private Camera _camera;

        private void Start() {
            _camera = GetComponent<Camera>();
            
            for (int i = 1; i < layers; i++) {
                string name = $"RenderLayer{i}";
                GameObject layer = new GameObject(name);
                layer.transform.parent = transform;
                var cam = layer.AddComponent<Camera>();
                cam.CopyFrom(_camera);
                cam.clearFlags = CameraClearFlags.Depth;
                cam.cullingMask = LayerMask.GetMask(name);
            }
        }
    }
}