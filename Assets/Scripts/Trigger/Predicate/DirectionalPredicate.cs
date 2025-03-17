using UnityEngine;

namespace Phos.Trigger.Predicate {
    public class DirectionalPredicate : BaseTriggerPredicate {
        public override bool Evaluate(TriggerContext context) {
            return Vector3.Dot(context.Collider.transform.forward, transform.forward) < 0f;
        }
    }
}