using Phos.Callback;
using UnityEngine;

namespace Phos.Trigger {
    public abstract class BaseCollisionTrigger : BaseTrigger {
        public PlayerController controller;
        public Component[] callbacks;

        private void Start() {
            if (controller == null) {
                enabled = false;
                return;
            }

            Debug.Log($"Try Register Listeners");
            var listeners = GetComponents<ICallbackListener<bool>>();
            foreach (var listener in listeners) {
                Register(listener);
                Debug.Log($"Register Listener {listener}");
            }

            OnStart();
        }

        protected abstract void OnPlayerEnter();

        protected abstract void OnPlayerLeave();

        protected virtual void OnStart() { 
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject == controller.gameObject) {
                OnPlayerEnter();
                //Debug.Log("Enter");
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.gameObject == controller.gameObject) {
                OnPlayerLeave();
                //Debug.Log("Leave");
            }
        }
    }
}