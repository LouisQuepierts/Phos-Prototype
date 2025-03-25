using System;
using UnityEngine;

namespace Phos.Trigger {
    [RequireComponent(typeof(TriggerController))]
    public class CollisionTriggerBehaviour : MonoBehaviour {
        public ExecutionFlag flag = ExecutionFlag.Enter;
        
        private TriggerController _trigger;
        
        private void Start() {
            _trigger = GetComponent<TriggerController>();
        }

        private void OnTriggerEnter(Collider other) {
            if (!flag.HasFlag(ExecutionFlag.Enter)) return;
            var context = _trigger.Context;
            context.NewValue = true;
            context.Collider = other.gameObject;

            _trigger.Trigger();
        }

        private void OnTriggerExit(Collider other) {
            if (!flag.HasFlag(ExecutionFlag.Exit)) return;
            var context = _trigger.Context;
            context.NewValue = false;
            context.Collider = other.gameObject;
            
            _trigger.Trigger();
        }
    }

    [Flags]
    [Serializable]
    public enum ExecutionFlag {
        Enter,
        Exit
    }
}