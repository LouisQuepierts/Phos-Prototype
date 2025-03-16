using UnityEngine;

namespace Phos.Navigate {
    public class RouterNode : BaseNode {

        private BaseNode _parent;
        
        public void Init(BaseNode parent) {
            _parent = parent;
        }
        
        public override BaseNode GetConnectedNode(Direction direction) {
            return _parent.GetConnectedNode(direction);
        }

        public override Vector3 GetNodePosition() {
            return transform.position;
        }
    }
}