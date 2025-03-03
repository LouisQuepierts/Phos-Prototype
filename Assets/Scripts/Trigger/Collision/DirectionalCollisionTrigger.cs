using Phos.Callback;
using UnityEngine;

namespace Phos.Trigger.Collision {
    public class DirectionalCollisionTrigger : BaseCollisionTrigger {
        private Vector3 m_direction;
        
        protected override void OnStart() {
            m_direction = transform.forward;
        }

        protected override void OnPlayerEnter() {
            float dot = Vector3.Dot(m_direction, controller.transform.forward);
            //Debug.Log($"Dot {dot}");
            SetValue(dot < 0);
        }

        protected override void OnPlayerLeave() {

        }

        private void OnDrawGizmosSelected() {
            Gizmos.DrawRay(transform.position, transform.forward);
        }
    }
}