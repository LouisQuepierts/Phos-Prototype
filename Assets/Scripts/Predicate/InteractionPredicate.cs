using Phos.Interact;
using Phos.Utils;
using UnityEngine;

namespace Phos.Predicate {
    public class InteractionPredicate : BasePredicate {
        public InteractionControl control;
        public MathComparater comparater;
        public float value;

        public override bool Evaluate(bool source = false) {
            return comparater.Compare(value, control.Segment);
        }
    }
}