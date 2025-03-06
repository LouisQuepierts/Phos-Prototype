using Phos.Operation;
using UnityEngine;

namespace Phos.Controller {
	public abstract class BaseController : MonoBehaviour {
        [Header("Operation")]
        public GameObject operationGroup;

        private BaseBiOperation[] m_operations;

        private void Start() {
            PreInitialization();

            m_operations = operationGroup == null ? GetComponents<BaseBiOperation>() : operationGroup.GetComponents<BaseBiOperation>();

            if (m_operations == null || m_operations.Length == 0) {
                enabled = false;
                return;
            }

            PostInitialization();
        }

        protected virtual void PreInitialization() { }

        protected virtual void PostInitialization() { }

        protected void Execute(bool trigger = false) {
            if (m_operations != null) {
                foreach (var opr in m_operations) {
                    opr.Execute(trigger);
                }
            }
        }
    }
}