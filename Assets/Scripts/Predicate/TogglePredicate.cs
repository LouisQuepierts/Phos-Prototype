using System;
using UnityEngine;

namespace Phos.Predicate {
    public class TogglePredicate : BasePredicate {
        public bool value;
        public override bool Evaluate(bool source = false) {
            return value;
        }
    }
}