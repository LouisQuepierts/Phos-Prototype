using UnityEngine;

namespace Phos.Layer {
    [RequireComponent(typeof(Collider))]
    public class LayerRegion : MonoBehaviour {
		public LayerMask enterMask;
		public LayerMask exitMask;

        private void OnCollisionEnter(Collision collision) {
            if (collision.gameObject.CompareTag("Player")) {
                collision.gameObject.layer = enterMask;
            }
        }

        private void OnCollisionExit(Collision collision) {
            if (collision.gameObject.CompareTag("Player")) {
                collision.gameObject.layer = exitMask;
            }
        }
    }
}