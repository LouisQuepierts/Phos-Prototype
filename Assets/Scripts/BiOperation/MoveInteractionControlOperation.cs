using Phos.Interact;
using Phos.Operation;
using Phos.Trigger;
using UnityEngine;

namespace BiOperation {
    public class MoveInteractionControlOperation : BaseBiOperation {
        [Header("Interaction Control Properties")]
        public BaseInteractionControl control;
        public int segmentPreExecute;

        [Header("Movement Properties")]
        public ExecutionFlag flag = ExecutionFlag.Enter;
        public bool invertOnExit;
        
        public override void Execute(bool trigger) {
            if ((!trigger || !flag.HasFlag(ExecutionFlag.Enter)) &&
                (trigger || !flag.HasFlag(ExecutionFlag.Exit))) return;
            
            if (invertOnExit && !trigger) {
                control.MoveDelta(-segmentPreExecute);
            }
            else {
                control.MoveDelta(segmentPreExecute);
            }
        }
    }
}