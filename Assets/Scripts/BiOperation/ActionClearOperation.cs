using Phos.Operation;
using Phos.Perform;

namespace BiOperation {
    public class ActionClearOperation : BaseBiOperation {
        public override void Execute(bool trigger) {
            SceneController.Instance.ClearActions();
        }
    }
}