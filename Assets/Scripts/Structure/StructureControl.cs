using UnityEngine;

namespace Phos.Structure {
    public abstract class StructureControl : MonoBehaviour {
        protected ReadonlyProperty<float> m_segment;
        private float m_lastSegment;

        public abstract void InteractFinished();

        protected abstract void UpdateTransform();

        public void BindProperty(ReadonlyProperty<float> proeprty) {
            Debug.Log($"Bind {proeprty}");
            m_segment = proeprty;
        }

        private void FixedUpdate() {
            if (m_segment == null) return;

            if (m_lastSegment != m_segment.Value) {
                UpdateTransform();
            }
            m_lastSegment = m_segment.Value;
        }
    }
}