using Phos.Callback;
using UnityEngine;

namespace Phos.Trigger {
    public abstract class BaseTrigger : CallbackProvider<bool> {
        public bool defualt = false;

        private SharedProperty<bool> _enabled;

        private void Awake() {
            _enabled = new SharedProperty<bool>(defualt);
        }

        protected void SetValue(bool value) {
            if (_enabled.Value != value) {
                Debug.Log($"Post Callbacks, value {value}");
                Post(value);
            }
            _enabled.Value = value;
        }
    }
}