using System;
using Phos.Operation;
using Phos.Perform;
using Phos.Trigger;

namespace BiOperation {
    public class SceneArchiveOperation : BaseBiOperation {
        public int archiveID;
        public ExecutionFlag flag;
        
        private SceneController _sceneController;
        
        private void Start() {
            _sceneController = SceneController.Instance;
        }

        public override void Execute(bool trigger) {
            if (!flag.HasFlag(trigger ? ExecutionFlag.Enter : ExecutionFlag.Exit)) return;
            _sceneController.RecordArchive(archiveID);
        }
    }
}