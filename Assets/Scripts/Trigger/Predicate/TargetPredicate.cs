using UnityEngine;

namespace Phos.Trigger.Predicate {
    public class TargetPredicate : BaseTriggerPredicate {
        public GameObject target;
        public override bool Evaluate(TriggerContext context) {
            return context.Collider == target;
        }
    }
}