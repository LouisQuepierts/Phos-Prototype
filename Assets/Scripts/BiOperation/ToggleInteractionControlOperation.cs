using Phos.Interact;
using Phos.Navigate;
using Phos.Operation;
using UnityEngine;

namespace Phos.BiOperation {
    public class ToggleInteractionControlOperation : BaseBiOperation {
        public BaseInteractionControl control;

        public bool invert;

        private void Start() {
        }

        public override void Execute(bool trigger) {
            if (!enabled) {
                return;
            }

            control.active = trigger != invert;
        }
    }
}