using Phos.Callback;
using Phos.Navigate;
using UnityEngine;

namespace Phos.Structure {
    public abstract class StructureControl : CallbackProvider<StructureControl.CallbackContext> {
        public readonly struct CallbackContext {
            public readonly float segment;
            public readonly CallbackType type;
            public CallbackContext(float segment, CallbackType type) {
                this.segment = segment;
                this.type = type;
            }
        }

        public enum CallbackType {
            InteractBegin,
            InteractFinished,
            ChangeSegment,
            AlignFinished
        }

        protected ReadonlyProperty<float> m_segment;
        private float m_lastSegment;
        private int m_lastRounded;

        public virtual float Segment {
            get => m_segment.Value;
        }

        public abstract void InteractFinished();

        public virtual void AlignFinished() {
            Post(new CallbackContext(m_segment.Value, CallbackType.AlignFinished));
        }

        public virtual void SegmentChanged() {
            Post(new CallbackContext(m_segment.Value, CallbackType.ChangeSegment));
        }

        protected abstract void UpdateTransform();

        protected virtual void OnFixedUpdate() { }

        public void BindProperty(ReadonlyProperty<float> property) {
            Debug.Log($"{this} Bind {property}");
            m_segment = property;
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