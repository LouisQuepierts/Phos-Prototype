using UnityEngine;

namespace Phos.Interact {
    public class TranslationControl : BaseInteractionControl {
        [Header("Translation Properties")]
        public float segmentLength = 1;

        // Initialize in Awake
        private Vector3 m_origin;
        private Vector3 m_direction;
        private Plane m_plane;

        private Vector3 m_lastPosition;
        private Quaternion m_lastRotation;

        public float m_nearestPoint;
        public float m_displacement;

        public float m_overshootPoint;
        public float m_overshootThreshold;

        private float m_alignVelocity;

        public float m_minDisplacement;
        public float m_maxDisplacement;

        protected override bool PerformAlign() {
            float displace = Mathf.SmoothDamp(
                m_displacement,
                m_nearestPoint,
                ref m_alignVelocity,
                alignTime
            );

            bool flag = false;
            if (Mathf.Abs(m_nearestPoint - displace) < 1e-2) {
                m_displacement = m_nearestPoint;
                flag = true;
            } else {
                m_displacement = displace;
            }

            transform.localPosition = m_origin + m_direction * m_displacement;
            return flag;
        }

        protected override void PerformDrag(Vector3 start, Vector3 current) {
            Vector3 move = current - start;
            float displace = Vector3.Dot(m_direction, move) + m_displacement;
            m_displacement = Mathf.Clamp(displace, m_minDisplacement, m_maxDisplacement);
            transform.localPosition = m_origin + m_direction * m_displacement;
        }

        protected override bool PerformOvershoot() {
            float displace = Mathf.SmoothDamp(
                m_displacement,
                m_overshootPoint,
                ref m_alignVelocity,
                alignTime
            );

            bool flag = false;
            if (Mathf.Abs(m_overshootPoint - displace) < 1e-2) {
                flag = true;
                m_displacement = m_overshootPoint;
            } else {
                if (hardEdge) {
                    if (displace > m_maxDisplacement) {
                        float delta = (m_overshootPoint - m_maxDisplacement) * reboundCeofficient;
                        m_alignVelocity = -m_alignVelocity;
                        m_overshootPoint = m_maxDisplacement - delta;
                        displace = m_maxDisplacement;
                    } else if (displace < m_minDisplacement) {
                        float delta = (m_overshootPoint - m_minDisplacement) * reboundCeofficient;
                        m_alignVelocity = -m_alignVelocity;
                        m_overshootPoint = m_minDisplacement - delta;
                        displace = m_minDisplacement;
                    }
                }
                m_displacement = displace;
            }

            transform.localPosition = m_origin + m_direction * m_displacement;
            return flag;
        }

        protected override bool Raycast(Ray ray, out float enter, bool update) {
            if (update) {
                if (m_lastRotation != transform.rotation) {
                    m_plane.normal = GetPlaneNormal();
                    m_lastRotation = transform.rotation;
                }

                if (m_lastPosition != transform.position) {
                    m_plane.distance = 0f - Vector3.Dot(m_plane.normal, transform.position);
                    m_lastPosition = transform.position;
                }
            }
            return m_plane.Raycast(ray, out enter);
        }

        protected override bool ShouldOvershoot() {
            m_nearestPoint = Mathf.Round(m_displacement / segmentLength) * segmentLength;

            float delta = Mathf.Abs(m_nearestPoint - m_displacement) - m_overshootThreshold;
            if (delta > 0f) {
                if (m_displacement > m_nearestPoint) {
                    delta = -delta;
                }

                m_overshootPoint = m_nearestPoint + delta * overshootCeoficient;
                return true;
            }

            return false;
        }

        protected override float UpdateSegment() {
            return m_displacement / segmentLength;
        }

        private void Awake() {
            m_direction = axis.Direction();
            m_origin = transform.localPosition;

            Vector3 normal = GetPlaneNormal();
            m_plane = new Plane(normal, transform.position);

            m_lastPosition = transform.position;
            m_lastRotation = transform.rotation;

            m_minDisplacement = minSegment * segmentLength;
            m_maxDisplacement = maxSegment * segmentLength;

            m_overshootThreshold = segmentLength * overshootThreshold;
        }

        private Vector3 GetPlaneNormal() {
            float dot = Vector3.Dot(m_direction, transform.up);

            if (dot < .99f) {
                return Vector3.Cross(m_direction, transform.up);
            } else {
                return Vector3.Cross(m_direction, transform.right);
            }
        }

#if UNITY_EDITOR
        public bool debug = false;

        private void OnDrawGizmos() {
            
        }
#endif
    }
}