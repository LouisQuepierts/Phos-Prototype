using UnityEngine;

namespace Phos.Structure {
    public class RotationStructureControl : StructureControl {
        public Vector3 anglePreSegment;

        private Vector3 m_origin;
        public override void InteractFinished() {

        }

        protected override void UpdateTransform() {
            transform.localRotation = Quaternion.Euler(m_origin + anglePreSegment * m_segment.Value);
        }

        private void Awake() {
            m_origin = transform.localEulerAngles;
        }
    }
}