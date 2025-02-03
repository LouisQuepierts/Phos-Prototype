using UnityEngine;

namespace Phos.Structure {
    public class TranslationStructureControl : StructureControl {
        public Vector3 displacePreAngle;

        public bool cycle;
        [Range(0, 32)]
        public float cyclePeriod = 1f;

        private Vector3 m_origin;
        public override void InteractFinished() {

        }

        protected override void UpdateTransform() {
            float segment = m_segment.Value;
            if (cycle) {
                segment = Mathf.Abs(segment) % cyclePeriod;
                float half = cyclePeriod / 2f;

                if (segment > half) {
                    segment = cyclePeriod - segment;
                }
            }

            transform.localPosition = m_origin + displacePreAngle * segment;
        }

        private void Awake() {
            m_origin = transform.localPosition;
        }
    }
}