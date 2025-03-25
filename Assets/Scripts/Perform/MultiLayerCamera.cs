using System;
using UnityEngine;

namespace Phos.Perform {
    [RequireComponent(typeof(Camera))]
    public class MultiLayerCamera : MonoBehaviour {
        [Range(1, 5)]
        public int layers;

        public float size;
        
        private Camera _camera;
        private Camera[] _cameras;

        private void Awake() {
            _camera = GetComponent<Camera>();
            size = _camera.orthographicSize;
            _cameras = new Camera[layers];
            _cameras[0] = _camera;
            
            for (int i = 1; i < layers; i++) {
                string name = $"RenderLayer{i}";
                GameObject layer = new GameObject(name);
                layer.transform.parent = transform;
                var cam = layer.AddComponent<Camera>();
                cam.CopyFrom(_camera);
                cam.clearFlags = CameraClearFlags.Depth;
                cam.cullingMask = LayerMask.GetMask(name);
                _cameras[i] = cam;
                Debug.Log($"{name} created");
            }
        }

        public void SetSize(float size) {
            this.size = size;
            if (_cameras == null) return;
            foreach (var cam in _cameras) {
                if (!cam) return;
                cam.orthographicSize = size;
            }
        }
    }
}