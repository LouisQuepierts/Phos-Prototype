namespace Phos.Trigger.Predicate {
    public class DisposablePredicate : BaseTriggerPredicate {
        private bool _triggered;
        public override bool Evaluate(TriggerContext context) {
            if (_triggered) return false;
            _triggered = context.TriggerController.Value;
            return _triggered;
        }

        public override void Reset() {
            _triggered = false;
        }
    }
}