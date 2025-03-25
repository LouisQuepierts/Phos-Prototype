using System;
using UnityEngine;

namespace Phos.Trigger {
    [RequireComponent(typeof(TriggerController))]
    public class EnableTriggerBehaviour : MonoBehaviour {
        public ExecutionFlag flag;
        public bool invert;
        
        private TriggerController _trigger;

        private void Start() {
            if (!flag.HasFlag(ExecutionFlag.Enter)) return;
            _trigger = GetComponent<TriggerController>();
            _trigger.Context.NewValue = !invert;
            _trigger.Trigger();
        }

        private void OnDisable() {
            if (!flag.HasFlag(ExecutionFlag.Exit)) return;
            _trigger = GetComponent<TriggerController>();
            _trigger.Context.NewValue = invert;
            _trigger.Trigger();
        }
    }
}