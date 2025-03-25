using Phos.Structure;

namespace Phos.Predicate {
    public class StructurePredicate : BaseComparePredicate {
        public StructureControl control;

        protected override float GetValue() {
            return control.Segment;
        }
    }
}