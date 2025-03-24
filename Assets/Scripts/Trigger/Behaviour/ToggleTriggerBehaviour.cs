using Phos.Utils;
using UnityEngine;

namespace Phos.Trigger {
    [RequireComponent(typeof(TriggerController))]
    public class ToggleTriggerBehaviour : MonoBehaviour, IToggleable {
        public bool trigger;
        
        private TriggerController _trigger;
        private bool _triggered;
        
        private void Start() {
            _trigger = GetComponent<TriggerController>();
        }

        private void Update() {
            switch (_triggered) {
                case false when trigger:
                    _trigger.Context.NewValue = true;
                    _trigger.Trigger();
                    _triggered = true;
                    break;
                case true when !trigger:
                    _trigger.Context.NewValue = false;
                    _trigger.Trigger();
                    _triggered = false;
                    break;
            }
        }

        public void Toggle(bool enable) {
            trigger = enable;
        }
    }
}