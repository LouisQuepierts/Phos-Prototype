using UnityEngine;

namespace Phos.Interact {
    public class RotationControl : BaseInteractionControl {
        [Header("Rotation Properties")]
        public float segmentAngle = 90;

        [Range(0f, 45f)]
        public float rotationRedundancy = 20;

        // Initialize in Awake
        private Plane m_plane;
        private float m_minAngle = float.NegativeInfinity;
        private float m_maxAngle = float.PositiveInfinity;

        // Runtime parameters
        private float m_rotated = 0f;
        private float m_nearestAngle;
        private float m_overshootAngle;
        private float m_overshootThreshold;

        private Vector3 m_lastPosition;
        private Quaternion m_lastRotation;

        private float m_alignVelocity;

        protected override bool PerformAlign() {
            float angle = Mathf.SmoothDamp(
                m_rotated,
                m_nearestAngle,
                ref m_alignVelocity,
                alignTime
            );

            bool flag = false;
            if (Mathf.Abs(m_nearestAngle - angle) < .1f) {
                m_rotated = m_nearestAngle;
                flag = true;
            } else {
                m_rotated = angle;
            }

            transform.localRotation = Quaternion.Euler(m_rotated * axis.Direction());
            return flag;
        }

        protected override void PerformDrag(Vector3 start, Vector3 current) {
            Vector3 startToCurrent = current - transform.position;
            Vector3 startToDrag = start - transform.position;

            float delta = Vector3.SignedAngle(startToDrag, startToCurrent, transform.up);
            float angle = Mathf.Clamp(m_rotated + delta, m_minAngle, m_maxAngle);

            m_rotated = angle;
            transform.localRotation = Quaternion.Euler(angle * axis.Direction());

            /*float totalAngle = m_rotated + delta;
            if (maxSegment != 0 && totalAngle > m_maxAngle) {
                delta = m_maxAngle - totalAngle;
            } else if (minSegment != 0 && totalAngle < m_minAngle) {
                delta = m_minAngle - totalAngle;
            }

            if (Mathf.Abs(delta) > .01f) {
                Rotate(delta);
            }*/
        }

        protected override bool PerformOvershoot() {
            float angle = Mathf.SmoothDamp(
                m_rotated,
                m_overshootAngle,
                ref m_alignVelocity,
                alignTime
            );

            bool flag = false;

            if (Mathf.Abs(m_overshootAngle - angle) < 1) {
                flag = true;
                m_rotated = m_overshootAngle;
            } else {
                if (hardEdge) {
                    if (angle > m_maxAngle) {
                        float delta = (m_overshootAngle - m_maxAngle) * reboundCeofficient;
                        m_alignVelocity = -m_alignVelocity;
                        m_overshootAngle = m_maxAngle - delta;
                        angle = m_maxAngle;
                    } else if (angle < m_minAngle) {
                        float delta = (m_overshootAngle - m_minAngle) * reboundCeofficient;
                        m_alignVelocity = -m_alignVelocity;
                        m_overshootAngle = m_minAngle - delta;
                        angle = m_minAngle;
                    }
                }
                m_rotated = angle;
            }
            transform.localRotation = Quaternion.Euler(m_rotated * axis.Direction());
            return flag;
        }

        protected override bool Raycast(Ray ray, out float enter, bool update) {
            if (update) {
                if (m_lastRotation != transform.rotation) {
                    m_plane.normal = axis.Direction(transform);
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
            m_nearestAngle = Mathf.Round(m_rotated / segmentAngle) * segmentAngle;

            float delta = Mathf.Abs(m_nearestAngle - m_rotated) - m_overshootThreshold;

            if (delta > 0f) {
                if (m_rotated > m_nearestAngle) {
                    delta = -delta;
                }
                m_overshootAngle = m_nearestAngle + delta * overshootCeoficient;
                return true;
            }

            return false;
        }

        protected override float UpdateSegment() {
            return m_rotated / segmentAngle;
        }

        private void Awake() {
            m_plane = new Plane(axis.Direction(transform), transform.position);

            if (minSegment != 1) {
                m_minAngle = minSegment * segmentAngle - rotationRedundancy;
            }

            if (maxSegment != -1) {
                m_maxAngle = maxSegment * segmentAngle + rotationRedundancy;
            }

            m_lastPosition = transform.position;
            m_lastRotation = transform.rotation;

            m_overshootThreshold = segmentAngle * overshootThreshold;
        }

        private void Rotate(float delta) {
            m_rotated += delta;
            transform.Rotate(transform.up, delta, Space.World);
        }
    }
}