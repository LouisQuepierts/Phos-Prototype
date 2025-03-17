namespace Phos.Trigger.Predicate {
    public class ChangePredicate : BaseTriggerPredicate {
        public override bool Evaluate(TriggerContext context) {
            return context.NewValue != context.TriggerController.Value;
        }
    }
}