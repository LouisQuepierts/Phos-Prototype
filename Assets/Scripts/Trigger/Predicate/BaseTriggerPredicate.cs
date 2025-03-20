using UnityEngine;

namespace Phos.Trigger.Predicate {
    public abstract class BaseTriggerPredicate : MonoBehaviour {
        public abstract bool Evaluate(TriggerContext context);
    }
}