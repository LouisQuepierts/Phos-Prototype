using Phos.Predicate;
using Phos.Utils;
using UnityEngine;

namespace Phos.Trigger.Predicate {
    public class WrappedPredicate : BaseTriggerPredicate {
        [Header("Condition")]
        public LogicalOperator conditionOperator;
        [Tooltip("Optional")]
        public GameObject conditionGroup;

        private PredicateHolder _holder;

        private void Start() {
            _holder = new();
            _holder.@operator = conditionOperator;
            _holder.Init(conditionGroup == null ? gameObject : conditionGroup);
        }

        public override bool Evaluate(TriggerContext context) {
            return _holder.Evaluate();
        }
    }
}