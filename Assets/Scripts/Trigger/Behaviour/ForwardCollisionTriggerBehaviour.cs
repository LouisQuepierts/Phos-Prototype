using UnityEngine;

namespace Phos.Trigger {
    [RequireComponent(typeof(TriggerController))]
    public class ForwardCollisionTriggerBehaviour : MonoBehaviour {
        public CollisionFlag flag;
        private TriggerController _trigger;
        
        private void Start() {
            _trigger = GetComponent<TriggerController>();
            Debug.Log("Try Register Listeners");
        }

        private void OnTriggerEnter(Collider other) {
            if (!flag.HasFlag(CollisionFlag.Enter)) return;
            Trigger(other);
        }

        private void OnTriggerExit(Collider other) {
            if (!flag.HasFlag(CollisionFlag.Exit)) return;
            Trigger(other);
        }

        private void Trigger(Collider other) {
            var context = _trigger.Context;
            context.NewValue = Vector3.Dot(transform.forward, other.transform.forward) < 0;
            context.Collider = other.gameObject;

            _trigger.Trigger();
        }
    }
}