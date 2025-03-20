using UnityEngine.Serialization;

namespace Phos.Trigger.Predicate {
    public class EdgePredicate : BaseTriggerPredicate {
        public ExecutionFlag edge;
        
        public override bool Evaluate(TriggerContext context) {
            return edge.HasFlag(context.NewValue ? ExecutionFlag.Enter : ExecutionFlag.Exit);
        }
    }
}