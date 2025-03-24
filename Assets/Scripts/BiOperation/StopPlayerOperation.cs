using Phos;
using Phos.Operation;
using Phos.Perform;
using Phos.Trigger;

namespace BiOperation {
    public class StopPlayerOperation : BaseBiOperation {
        public ExecutionFlag flag;
        private PlayerController _player;

        private void Start() {
            _player = SceneController.Instance.Player;
        }
        
        public override void Execute(bool trigger) {
            if (!flag.HasFlag(trigger ? ExecutionFlag.Enter : ExecutionFlag.Exit)) return;
            _player.Stop();
        }
    }
}