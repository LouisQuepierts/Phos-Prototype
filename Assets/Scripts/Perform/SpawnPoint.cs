using Phos.Navigate;
using UnityEngine;

namespace Phos.Perform {
    public class SpawnPoint : MonoBehaviour {
        private BaseNode _node;

        private void Awake() {
            _node = GetComponent<BaseNode>();
        }

        public Vector3 GetPosition() {
            return _node ? _node.GetNodePosition() : transform.position;
        }
    }
}