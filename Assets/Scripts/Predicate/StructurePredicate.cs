using Phos.Structure;
using Phos.Utils;
using UnityEngine;

namespace Phos.Predicate {
    public class StructurePredicate : BasePredicate {
        public StructureControl control;
        public MathComparater comparater;
        public float value;

        public override bool Evaluate(bool source = false) {
            return comparater.Compare(value, control.Segment);
        }
    }
}