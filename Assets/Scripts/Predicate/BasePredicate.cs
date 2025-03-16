using UnityEngine;

namespace Phos.Predicate {
    public abstract class BasePredicate : MonoBehaviour {
        public abstract bool Evaluate(bool source = false);
    }
}