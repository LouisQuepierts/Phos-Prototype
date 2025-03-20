using Phos.Operation;
using UnityEngine;

namespace Phos.Controller {
	public abstract class BaseController : MonoBehaviour {
        [Header("Operation")]
        public GameObject operationGroup;

        private BaseBiOperation[] _operations;

        private void Start() {
            PreInitialization();

            _operations = operationGroup == null ? GetComponents<BaseBiOperation>() : operationGroup.GetComponents<BaseBiOperation>();

            if (_operations == null || _operations.Length == 0) {
                enabled = false;
                return;
            }

            PostInitialization();
        }

        protected virtual void PreInitialization() { }

        protected virtual void PostInitialization() { }

        protected void Execute(bool trigger = false) {
            if (_operations == null) return;
            foreach (var opr in _operations) {
                opr.Execute(trigger);
            }
        }
    }
}