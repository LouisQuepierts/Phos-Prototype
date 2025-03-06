using Phos.Callback;
using Phos.Operation;
using Phos.Predicate;
using Phos.Structure;
using Phos.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Phos.Controller {
	public class ConditionalController : BaseController, ICallbackListener<StructureControl.CallbackContext> {
        [Header("Condition")]
        public LogicalOperator conditionOperator;
        [Tooltip("Optional")]
        public GameObject conditionGroup;

        [Header("Callback Listen")]
        public CallbackProvider<StructureControl.CallbackContext> callback;

        private PredicateHolder condition;

        protected override void PostInitialization() {
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
            if (callback == null && condition != null) {
                bool trigger = condition.Evaluate();
                Execute(trigger);
            }
        }

        public void OnCallback(StructureControl.CallbackContext t) {
            if (condition != null) {
                bool trigger = condition.Evaluate();
                Execute(trigger);
            }
        }
    }
}