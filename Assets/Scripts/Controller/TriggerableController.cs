using Phos.Callback;
using Phos.Trigger;
using UnityEngine;

namespace Phos.Controller {
    public class TriggerableController : BaseController, ICallbackListener<bool> {
        public BaseTrigger[] triggers;

        protected override void PostInitialization() {
            foreach (var trigger in triggers) {
                trigger.Register(this);
            }
        }

        private void OnDisable() {
            foreach (var trigger in triggers) {
                trigger.Unregister(this);
            }
        }

        public void OnCallback(bool t) {
            Execute(t);
        }
    }
}