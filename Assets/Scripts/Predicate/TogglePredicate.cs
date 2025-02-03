using UnityEngine;

namespace Phos.Predicate {
    [CreateAssetMenu(fileName = "TogglePredicate", menuName = "Predicate/TogglePredicate")]
    public class TogglePredicate : BasePredicate {
        public bool value;
        public override bool Evaluate() {
            return value;
        }
    }
}