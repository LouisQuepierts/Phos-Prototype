using Phos.Navigate;
using UnityEngine;

namespace Phos.Predicate {
    public class AutoCheckPredicate : BasePredicate {
        public NavigateNode nodeA;
        public NavigateNode nodeB;

        private void Awake() {
            if (nodeA == null || nodeB == null) {
                enabled = false;
            }
        }

        public override bool Evaluate(bool source = false) {
            foreach (var direactionA in nodeA.AvailableDirections) {
                foreach (var direactionB in nodeB.AvailableDirections) {
                    Vector3 delta = nodeA.GetConnectionPosition(direactionA) - nodeB.GetConnectionPosition(direactionB);
                    if (delta.magnitude < 1e-2) {
                        return true;
                    }
                }
            }
            
            return false;
        }
    }
}