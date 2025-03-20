using Phos.Operation;
using UnityEngine;

namespace BiOperation {
    public class DebugOperation : BaseBiOperation {
        public string message;
        public override void Execute(bool trigger) {
            Debug.Log($"{message}: {trigger}");
        }
    }
}