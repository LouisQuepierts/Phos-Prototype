using Phos.Callback;
using Phos.Operation;
using Phos.Predicate;
using Phos.Structure;
using Phos.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Phos.Controller {
	public class ConditionalController : MonoBehaviour, ICallbackListener<StructureControl.CallbackContext> {
        [Header("Condition")]
        public LogicalOperator conditionOperator;
        [Tooltip("Optional")]
        public GameObject conditionGroup;

        [Header("Operation")]
        public GameObject operationGroup;

        [Header("Callback Listen")]
        public CallbackProvider<StructureControl.CallbackContext> callback;

        private BaseBiOperation[] m_operations;
        private PredicateHolder condition;

        private void Start() {
            m_operations = operationGroup == null ? GetComponents<BaseBiOperation>() : operationGroup.GetComponents<BaseBiOperation>();

            if (m_operations == null || m_operations.Length == 0) {
                enabled = false;
                return;
            }

            if (callback != null) {
                callback.Register(this);
            }

            condition = new PredicateHolder();
            condition.@operator = conditionOperator;
            condition.Init(conditionGroup == null ? gameObject : conditionGroup);
        }

        private void OnDisable() {
            if (callback != null) {
                callback.Unregister(this);
            }
        }

        private void FixedUpdate() {
            if (callback != null && condition != null && m_operations != null) {
                bool trigger = condition.Evaluate();
                foreach (var opr in m_operations) {
                    opr.Execute(trigger);
                }
            }
        }

        public void OnCallback(StructureControl.CallbackContext t) {
            if (condition != null && m_operations != null) {
                bool trigger = condition.Evaluate();
                foreach (var opr in m_operations) {
                    opr.Execute(trigger);
                }
            }
        }
    }
}