using Phos.Interact;
using Phos.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace Phos.Predicate {
    public class InteractionPredicate : BaseComparePredicate {
        public BaseInteractionControl control;
        
        protected override float GetValue() {
            return control.Segment;
        }
    }
}