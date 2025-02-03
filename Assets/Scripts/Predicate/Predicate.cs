using UnityEngine;

namespace Phos.Predicate {
    public abstract class BasePredicate : ScriptableObject {
        public abstract bool Evaluate();
    }
}