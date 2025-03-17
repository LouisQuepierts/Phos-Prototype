using System;
using Phos.Trigger.Predicate;
using UnityEngine;

namespace Phos.Trigger {
    [RequireComponent(typeof(TriggerController))]
    public class CollisionTriggerBehaviour : MonoBehaviour {
        public CollisionFlag flag;
        
        private TriggerController _trigger;
        
        private void Start() {
            _trigger = GetComponent<TriggerController>();
            Debug.Log("Try Register Listeners");
        }

        private void OnTriggerEnter(Collider other) {
            if (!flag.HasFlag(CollisionFlag.Enter)) return;
            var context = _trigger.Context;
            context.NewValue = true;
            context.Collider = other.gameObject;

            _trigger.Trigger();
        }

        private void OnTriggerExit(Collider other) {
            if (!flag.HasFlag(CollisionFlag.Exit)) return;
            var context = _trigger.Context;
            context.NewValue = false;
            context.Collider = other.gameObject;
            
            _trigger.Trigger();
        }
    }

    [Flags]
    public enum CollisionFlag {
        Enter,
        Exit
    }
}