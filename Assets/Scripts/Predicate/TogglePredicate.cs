using System;
using Phos.Utils;
using UnityEngine;

namespace Phos.Predicate {
    public class TogglePredicate : BasePredicate, IToggleable {
        public bool value;
        public override bool Evaluate(bool source = false) {
            return value;
        }

        public void Toggle(bool enable) {
            value = enable;
        }
    }
}