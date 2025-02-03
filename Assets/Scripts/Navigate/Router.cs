using Unity.VisualScripting;
using UnityEngine;

namespace Phos.Navigate {
    [ExecuteInEditMode]
    public class Router : MonoBehaviour {
        [DoNotSerialize]
        public NavigateNode Parent { get; private set; }

        private void OnEnable() {
            NavigateNode parent = GetComponentInParent<NavigateNode>();
            if (parent == null) {
                Debug.LogError("VirtualPathRouter must be a child of VirtualNode");
            }
            Parent = parent;
        }

        private void OnDrawGizmos() {
            if (Parent == null) return;
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(transform.position, 0.1f);
        }
    }
}