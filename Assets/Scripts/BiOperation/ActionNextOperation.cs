using Phos.Operation;
using Phos.Perform;

namespace BiOperation {
    public class ActionNextOperation : BaseBiOperation {
        public override void Execute(bool trigger) {
            SceneController.Instance.NextAction();
        }
    }
}