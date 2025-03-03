using UnityEngine;

namespace Phos.Predicate {
    public abstract class BasePredicate : MonoBehaviour, IPredicate {
        public abstract bool Evaluate(bool source = false);
    }
}