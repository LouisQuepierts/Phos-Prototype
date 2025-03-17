using Phos.Navigate;
using Phos.Operation;
using UnityEngine;

namespace Phos.BiOperation {
    public class TogglePathOperation : BaseBiOperation {
        public NavigateNode nodeA;
        public NavigateNode nodeB;

        public bool invert;

        private NodePath _path;

        private void Start() {
            if (nodeA == null || nodeB == null || nodeA == nodeB) {
                Debug.LogError("BiOperation.TogglePathOperation must have two difference nodes!");
                enabled = false;
                return;
            }

            _path = nodeA.Paths[nodeB];
            if (_path == null) {
                Debug.LogError($"BiOperation.TogglePathOperation: {nodeA} does not have a path to {nodeB}");
                enabled = false;
            }
        }

        public override void Execute(bool trigger) {
            if (!enabled) {
                return;
            }

            _path.active = trigger != invert;

            Debug.Log($"Update Access {_path.active}");
        }
    }
}