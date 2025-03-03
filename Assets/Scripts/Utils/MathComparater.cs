using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Phos.Utils {
    public enum MathComparater {
        Higher,
        Lower,
        Equal,
        HigherOrEqual,
        LowerOrEqual,
        NotEqual
    }

    public static class MathComparaterExtension {
        public static bool Compare(this MathComparater comparater, float x, float y) {
            return (comparater) switch {
                MathComparater.Higher => x > y,
                MathComparater.Lower => x < y,
                MathComparater.Equal => Mathf.Abs(x - y) < Mathf.Epsilon,
                MathComparater.HigherOrEqual => x >= y,
                MathComparater.LowerOrEqual => x <= y,
                MathComparater.NotEqual => Mathf.Abs(x - y) > Mathf.Epsilon,
                _ => false
            };
        }
    }
}
