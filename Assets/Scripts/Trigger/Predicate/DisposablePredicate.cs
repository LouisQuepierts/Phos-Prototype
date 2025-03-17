namespace Phos.Trigger.Predicate {
    public class DisposablePredicate : BaseTriggerPredicate{
        public override bool Evaluate(TriggerContext context) {
            return !context.TriggerController.Value;
        }
    }
}