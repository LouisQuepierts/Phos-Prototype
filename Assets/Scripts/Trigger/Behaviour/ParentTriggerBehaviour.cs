using System;
using UnityEngine;

namespace Phos.Trigger {
    [RequireComponent(typeof(TriggerController))]
    public class ParentTriggerBehaviour : MonoBehaviour {
        private static Action<Transform, Transform, GameObject> _triggers;

        // ReSharper disable Unity.PerformanceAnalysis
        public static void Trigger(Transform current, Transform last, GameObject @object) {
            _triggers?.Invoke(current, last, @object);
        }

        public Transform target;
        public bool includeChildren;
        private TriggerController _trigger;

        private void Invoke(Transform current, Transform last, GameObject @object) {
            if (!current || !last) return;
            var context = _trigger.Context;
            
            if (target == current || includeChildren && current.IsChildOf(target)) {
                Debug.Log("Trigger");
                context.NewValue = true;
                context.Collider = @object;
                _trigger.Trigger();
            }
            else if (target == last || includeChildren && last.IsChildOf(target)) {
                Debug.Log("Trigger");
                context.NewValue = false;
                context.Collider = @object;
                _trigger.Trigger();
            }
        }
        
        private void Awake() {
            _trigger = GetComponent<TriggerController>();
            if (!target) target = transform;
        }
        
        private void OnEnable() {
            _triggers += Invoke;
        }

        private void OnDisable() {
            _triggers -= Invoke;
        }
    }
}