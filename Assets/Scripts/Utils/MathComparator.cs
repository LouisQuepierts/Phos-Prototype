using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Phos.Utils {
    public enum MathComparator {
        Higher,
        Lower,
        Equal,
        HigherOrEqual,
        LowerOrEqual,
        NotEqual
    }

    public static class MathComparatorExtension {
        public static bool Compare(this MathComparator comparator, float x, float y) {
            return (comparator) switch {
                MathComparator.Higher => x > y,
                MathComparator.Lower => x < y,
                MathComparator.Equal => Mathf.Abs(x - y) < Mathf.Epsilon,
                MathComparator.HigherOrEqual => x >= y,
                MathComparator.LowerOrEqual => x <= y,
                MathComparator.NotEqual => Mathf.Abs(x - y) > Mathf.Epsilon,
                _ => false
            };
        }
    }
}
