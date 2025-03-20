using Phos.Interact;
using Phos.Operation;

namespace Phos.BiOperation {
    public class ToggleInteractionControlOperation : BaseBiOperation {
        public BaseInteractionControl control;

        public bool invert;

        public override void Execute(bool trigger) {
            if (!enabled) {
                return;
            }

            control.SetControlActive(trigger != invert);
        }
    }
}