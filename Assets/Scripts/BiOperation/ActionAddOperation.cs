using Phos.Operation;
using Phos.Perform;
using Phos.Trigger;
using UnityEngine.Events;

namespace BiOperation {
    public class ActionAddOperation : BaseBiOperation {
        public ActionTriggerBehaviour[] actions;
        
        // ReSharper disable Unity.PerformanceAnalysis
        public override void Execute(bool trigger) {
            foreach (var action in actions) {
                SceneController.Instance.AddAction(action.Invoke);
            }
        }
    }
}