using Phos.Operation;
using Phos.Perform;
using UnityEngine;

namespace BiOperation {
    public class CameraFollowOperation : BaseBiOperation {
        public CameraPoint followPoint;
        public bool invert;
        
        
        public override void Execute(bool trigger) {
            bool follow = trigger != invert;
            SceneController.Instance.CameraFollow(follow ? followPoint : null);
        }
    }
}