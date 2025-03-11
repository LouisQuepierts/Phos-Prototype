using System;
using UnityEngine;

namespace Phos.Optical {
    public abstract class OpticalDevice : MonoBehaviour {
        public bool debug;
        
        private Vector3 _position;
        private Quaternion _rotation;
        private OpticalDeviceManager _manager;
        
        public void Awake() {
            _position = transform.position;
            _rotation = transform.rotation;
        }

        private void Start() {
            _manager = OpticalDeviceManager.GetInstance();
        }

        protected abstract void OnTransformChanged();

        private void FixedUpdate() {
            Check();
        }

#if UNITY_EDITOR
        private void Update() {
            if (!debug || Application.isPlaying) return;
            Check();
        }
#endif

        private void Check() {
            if (!TransformChanged()) return;
            
            _position = transform.position;
            _rotation = transform.rotation;
                
            OnTransformChanged();
        }

        private bool TransformChanged() {
            return _position != transform.position || 
                   _rotation != transform.rotation;
        }
    }
}