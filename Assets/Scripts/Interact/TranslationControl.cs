using UnityEngine;

namespace Phos.Interact {
    public class TranslationControl : BaseInteractionControl {
        [Header("Translation Properties")]
        public float segmentLength = 1;

        // Initialize in Awake
        private Vector3 _origin;
        private Vector3 _direction;
        private Plane _plane;

        private Vector3 _lastPosition;
        private Quaternion _lastRotation;

        private float _nearestPoint;
        private float _displacement;

        private float _overshootPoint;
        private float _overshootThreshold;

        private float _alignVelocity;

        private float _minDisplacement;
        private float _maxDisplacement;

        protected override bool PerformAlign() {
            float displace = Mathf.SmoothDamp(
                _displacement,
                _nearestPoint,
                ref _alignVelocity,
                alignTime
            );

            bool flag = false;
            if (Mathf.Abs(_nearestPoint - displace) < 1e-2) {
                _displacement = _nearestPoint;
                flag = true;
            } else {
                _displacement = displace;
            }

            transform.localPosition = _origin + _direction * _displacement;
            return flag;
        }

        protected override void PerformDrag(Vector3 start, Vector3 current) {
            Vector3 move = current - start;
            float displace = Vector3.Dot(_direction, move) + _displacement;
            _displacement = Mathf.Clamp(displace, _minDisplacement, _maxDisplacement);
            transform.localPosition = _origin + _direction * _displacement;
        }

        protected override bool PerformOvershoot() {
            float displace = Mathf.SmoothDamp(
                _displacement,
                _overshootPoint,
                ref _alignVelocity,
                alignTime
            );

            bool flag = false;
            if (Mathf.Abs(_overshootPoint - displace) < 1e-2) {
                flag = true;
                _displacement = _overshootPoint;
            } else {
                if (hardEdge) {
                    if (displace > _maxDisplacement) {
                        float delta = (_overshootPoint - _maxDisplacement) * reboundCeofficient;
                        _alignVelocity = -_alignVelocity;
                        _overshootPoint = _maxDisplacement - delta;
                        displace = _maxDisplacement;
                    } else if (displace < _minDisplacement) {
                        float delta = (_overshootPoint - _minDisplacement) * reboundCeofficient;
                        _alignVelocity = -_alignVelocity;
                        _overshootPoint = _minDisplacement - delta;
                        displace = _minDisplacement;
                    }
                }
                _displacement = displace;
            }

            transform.localPosition = _origin + _direction * _displacement;
            return flag;
        }

        protected override bool Raycast(Ray ray, out float enter, bool update) {
            if (!update) return _plane.Raycast(ray, out enter);
            if (_lastRotation != transform.rotation) {
                _plane.normal = GetPlaneNormal();
                _lastRotation = transform.rotation;
            }

            if (_lastPosition == transform.position) return _plane.Raycast(ray, out enter);
            _plane.distance = 0f - Vector3.Dot(_plane.normal, transform.position);
            _lastPosition = transform.position;
            return _plane.Raycast(ray, out enter);
        }

        protected override bool ShouldOvershoot() {
            _nearestPoint = Mathf.Round(_displacement / segmentLength) * segmentLength;

            float delta = Mathf.Abs(_nearestPoint - _displacement) - _overshootThreshold;
            if (delta > 0f) {
                if (_displacement > _nearestPoint) {
                    delta = -delta;
                }

                _overshootPoint = _nearestPoint + delta * overshootCeoficient;
                return true;
            }

            return false;
        }

        protected override float UpdateSegment() {
            return _displacement / segmentLength;
        }

        protected override void MoveTo(int delta) {
            _nearestPoint = delta * segmentLength;
        }

        private void Awake() {
            _direction = axis.Direction();
            _origin = transform.localPosition;

            Vector3 normal = GetPlaneNormal();
            _plane = new Plane(normal, transform.position);

            _lastPosition = transform.position;
            _lastRotation = transform.rotation;

            _minDisplacement = minSegment * segmentLength;
            _maxDisplacement = maxSegment * segmentLength;

            _overshootThreshold = segmentLength * overshootThreshold;
        }

        private Vector3 GetPlaneNormal() {
            float dot = Vector3.Dot(_direction, transform.up);

            if (dot < .99f) {
                return Vector3.Cross(_direction, transform.up);
            } else {
                return Vector3.Cross(_direction, transform.right);
            }
        }
    }
}