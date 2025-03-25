using Phos.Utils;
using UnityEngine;

namespace Phos.Predicate {
    public abstract class BasePredicate : MonoBehaviour {
        public abstract bool Evaluate(bool source = false);
    }
    
    public abstract class BaseComparePredicate : BasePredicate {
        public MathComparator comparator;
        public float value;
        public float shift;

        public float mod;
        
        protected abstract float GetValue();
        
        public override bool Evaluate(bool source = false) {
            var f = GetValue() + shift;
            if (mod > 0) {
                f %= mod;
            }
            return comparator.Compare(value, f);
        }
    }
}