using System;
using UnityEngine;

namespace Phos.Optical {
    [ExecuteAlways]
    public abstract class OpticalDevice : MonoBehaviour {
        private Vector3 _position;
        private Quaternion _rotation;
        private OpticalDeviceManager _manager;

        protected OpticalDeviceManager Manager => _manager;

        public void Awake() {
            _position = transform.position;
            _rotation = transform.rotation;
        }

        private void OnEnable() {
            _manager = OpticalDeviceManager.GetInstance();
            _manager.Add(this);
        }

        private void OnDisable() {
            _manager.Remove(this);
        }

        private void OnDestroy() {
            Debug.Log("OpticalDevice Destroy");
        }

        public virtual void OpticalUpdate() {
            
        }

        protected virtual bool IsChanged() {
            return TransformChanged();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public  bool CheckChanged() {
            if (!IsChanged()) return false;
            Debug.Log(this);
            _position = transform.position;
            _rotation = transform.rotation;

            return true;
        }

        private bool TransformChanged() {
            return _position != transform.position || 
                   _rotation != transform.rotation;
        }
    }
}