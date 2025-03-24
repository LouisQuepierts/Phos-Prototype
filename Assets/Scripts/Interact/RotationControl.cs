using System;
using UnityEngine;

namespace Phos.Interact {
    public class RotationControl : BaseInteractionControl {
        [Header("Rotation Properties")]
        public float segmentAngle = 90;

        [Range(0f, 45f)]
        public float rotationRedundancy = 20;

        // Initialize in Awake
        private Plane _plane;
        private float _minAngle = float.NegativeInfinity;
        private float _maxAngle = float.PositiveInfinity;

        // Runtime parameters
        private float _rotated;
        private float _nearestAngle;
        private float _overshootAngle;
        private float _overshootThreshold;

        private Vector3 _lastPosition;
        private Quaternion _lastRotation;

        private float _alignVelocity;

        protected override bool PerformAlign() {
            float angle = Mathf.SmoothDamp(
                _rotated,
                _nearestAngle,
                ref _alignVelocity,
                alignTime
            );

            bool flag = false;
            if (Mathf.Abs(_nearestAngle - angle) < .1f) {
                _rotated = _nearestAngle;
                flag = true;
            } else {
                _rotated = angle;
            }

            transform.localRotation = Quaternion.Euler(_rotated * axis.Direction());
            return flag;
        }

        protected override void PerformDrag(Vector3 start, Vector3 current) {
            Vector3 startToCurrent = current - transform.position;
            Vector3 startToDrag = start - transform.position;

            float delta = Vector3.SignedAngle(startToDrag, startToCurrent, axis.Direction(transform));
            float angle = Mathf.Clamp(_rotated + delta, _minAngle, _maxAngle);

            _rotated = angle;
            transform.localRotation = Quaternion.Euler(angle * axis.Direction());

            /*float totalAngle = _rotated + delta;
            if (maxSegment != 0 && totalAngle > _maxAngle) {
                delta = _maxAngle - totalAngle;
            } else if (minSegment != 0 && totalAngle < _minAngle) {
                delta = _minAngle - totalAngle;
            }

            if (Mathf.Abs(delta) > .01f) {
                Rotate(delta);
            }*/
        }

        protected override void PerformScroll(float scrollY) {
            float angle = Mathf.Clamp(_rotated + scrollY, _minAngle, _maxAngle);
            _rotated = angle;
            transform.localRotation = Quaternion.Euler(angle * axis.Direction());
        }

        protected override bool PerformOvershoot() {
            float angle = Mathf.SmoothDamp(
                _rotated,
                _overshootAngle,
                ref _alignVelocity,
                alignTime
            );

            bool flag = false;

            if (Mathf.Abs(_overshootAngle - angle) < 1) {
                flag = true;
                _rotated = _overshootAngle;
            } else {
                if (hardEdge) {
                    if (angle > _maxAngle) {
                        float delta = (_overshootAngle - _maxAngle) * reboundCeofficient;
                        _alignVelocity = -_alignVelocity;
                        _overshootAngle = _maxAngle - delta;
                        angle = _maxAngle;
                    } else if (angle < _minAngle) {
                        float delta = (_overshootAngle - _minAngle) * reboundCeofficient;
                        _alignVelocity = -_alignVelocity;
                        _overshootAngle = _minAngle - delta;
                        angle = _minAngle;
                    }
                }
                _rotated = angle;
            }
            transform.localRotation = Quaternion.Euler(_rotated * axis.Direction());
            return flag;
        }

        protected override bool Raycast(Ray ray, out float enter, bool update) {
            if (update) {
                if (_lastRotation != transform.rotation) {
                    _plane.normal = axis.Direction(transform);
                    _lastRotation = transform.rotation;
                }

                if (_lastPosition != transform.position) {
                    _plane.distance = 0f - Vector3.Dot(_plane.normal, transform.position);
                    _lastPosition = transform.position;
                }
            }
            return _plane.Raycast(ray, out enter);
        }

        protected override bool ShouldOvershoot() {
            _nearestAngle = Mathf.Round(_rotated / segmentAngle) * segmentAngle;

            float delta = Mathf.Abs(_nearestAngle - _rotated) - _overshootThreshold;

            if (delta > 0f) {
                if (_rotated > _nearestAngle) {
                    delta = -delta;
                }
                _overshootAngle = _nearestAngle + delta * overshootCeoficient;
                return true;
            }

            return false;
        }

        protected override float UpdateSegment() {
            return _rotated / segmentAngle;
        }

        protected override void MoveTo(int delta) {
            _nearestAngle = delta * segmentAngle;
        }

        private void Awake() {
            _plane = new Plane(axis.Direction(transform), transform.position);

            if (minSegment != 1) {
                _minAngle = minSegment * segmentAngle - rotationRedundancy;
            }

            if (maxSegment != -1) {
                _maxAngle = maxSegment * segmentAngle + rotationRedundancy;
            }

            _lastPosition = transform.position;
            _lastRotation = transform.rotation;

            _overshootThreshold = segmentAngle * overshootThreshold;
        }
    }
}