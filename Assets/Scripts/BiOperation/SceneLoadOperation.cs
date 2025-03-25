using Phos.Operation;
using Phos.Perform;

namespace BiOperation {
    public class SceneLoadOperation : BaseBiOperation {
        public override void Execute(bool trigger) {
            SceneController.Instance.LoadArchive();
        }
    }
}