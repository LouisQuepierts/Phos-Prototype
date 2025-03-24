using System;
using UnityEngine;

namespace Phos.Optical {
    [ExecuteAlways]
    public abstract class OpticalDevice : MonoBehaviour {
        private Vector3 _position;
        private Quaternion _rotation;
        private OpticalDeviceManager _manager;
        private bool _changed;

        protected OpticalDeviceManager Manager {
            get {
                _manager ??= OpticalDeviceManager.GetInstance();
                _manager.Add(this);
                return _manager;
            }
        }

        public void Awake() {
            _position = transform.position;
            _rotation = transform.rotation;
        }

        private void OnEnable() {
            _manager = OpticalDeviceManager.GetInstance();
            _manager.Add(this);
        }

        protected virtual void OnDisable() {
            _manager.Remove(this);
        }

        private void OnValidate() {
            _changed = true;
        }

        public virtual void OpticalUpdate() {
            
        }

        public virtual void LateOpticalUpdate() {
            
        }

        protected virtual bool IsChanged() {
            bool changed = _changed;
            _changed = false;
            return changed || TransformChanged();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public  bool CheckChanged() {
            if (!IsChanged()) return false;
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