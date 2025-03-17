using UnityEngine.Serialization;

namespace Phos.Trigger.Predicate {
    public class EdgePredicate : BaseTriggerPredicate {
        public bool rising;
        
        public override bool Evaluate(TriggerContext context) {
            return context.NewValue == rising;
        }
    }
}