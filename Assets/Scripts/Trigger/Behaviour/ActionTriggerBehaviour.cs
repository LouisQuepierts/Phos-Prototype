using UnityEngine;

namespace Phos.Trigger {
    [RequireComponent(typeof(TriggerController))]
    public class ActionTriggerBehaviour : MonoBehaviour {
        private TriggerController _trigger;
        
        private void Start() {
            _trigger = GetComponent<TriggerController>();
        }

        public void Invoke() {
            _trigger.Context.NewValue = true;
            _trigger.Trigger();
        }
    }
}