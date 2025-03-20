using System;
using Phos.Navigate;
using UnityEngine;

namespace Phos.Trigger {
    [RequireComponent(typeof(TriggerController))]
    public class ArriveNodeTriggerBehaviour : MonoBehaviour {
        public BaseNode target;
        private TriggerController _controller;

        private static Action<BaseNode, Transform> _triggers;
        
        // ReSharper disable Unity.PerformanceAnalysis
        public static void Trigger(BaseNode node, Transform transform) {
            _triggers?.Invoke(node, transform);
        }
        
        public void Awake() {
            _controller = GetComponent<TriggerController>();
        }

        private void OnEnable() {
            _triggers += Invoke;
        }
        
        private void OnDisable() {
            _triggers -= Invoke;
        }

        private void Invoke(BaseNode node, Transform @object) {
            if (node != target) return;
            _controller.Context.NewValue = true;
            _controller.Context.Collider = @object.gameObject;
            _controller.Trigger();
        }
    }
}