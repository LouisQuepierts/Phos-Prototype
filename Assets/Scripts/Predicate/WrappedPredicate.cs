using UnityEngine;

namespace Phos.Predicate {
    public class WrappedPredicate : BasePredicate {
        public BasePredicate other;

        private void Awake() {
            if (other == null || other == this) {
                enabled = false;
            }
        }

        public override bool Evaluate(bool source = false) {
            return other.Evaluate(source);
        }
    }
}