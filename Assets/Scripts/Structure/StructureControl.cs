using UnityEngine;

namespace Phos.Structure {
    public abstract class StructureControl : MonoBehaviour {
        protected ReadonlyProperty<float> m_segment;
        private float m_lastSegment;

        public abstract void InteractFinished();

        protected abstract void UpdateTransform();

        protected virtual void OnFixedUpdate() { }

        public void BindProperty(ReadonlyProperty<float> proeprty) {
            Debug.Log($"{this} Bind {proeprty}");
            m_segment = proeprty;
        }

        private void FixedUpdate() {
            OnFixedUpdate();
            if (m_segment == null) return;

            if (m_lastSegment != m_segment.Value) {
                UpdateTransform();
            }
            m_lastSegment = m_segment.Value;
        }
    }
}