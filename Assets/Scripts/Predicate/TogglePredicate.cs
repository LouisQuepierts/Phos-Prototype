using System;
using UnityEngine;

namespace Phos.Predicate {
    [Serializable]
    public class TogglePredicate : IPredicate {
        public bool value;
        public bool Evaluate() {
            return value;
        }
    }
}