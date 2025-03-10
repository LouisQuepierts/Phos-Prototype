using UnityEngine;

namespace Phos.Structure {
    public class AnimationStructure : StructureControl {
        public AnimationClip clip;
        [Range(0.1f, 32f)]
        public float playPreSegment = 1f;

        public bool cycle;
        [Range(0, 32)]
        public float cyclePeriod = 1f;

        private Animation _animation;
        private float _timePreSegment;
        private float _segment;
        private float _value;

        public override float Segment {
            get => _segment / playPreSegment;
        }

        private void OnEnable() {
            _timePreSegment = clip.length / playPreSegment;

            _animation = gameObject.GetComponent<Animation>();
            _animation.AddClip(clip, clip.name);
            _animation.playAutomatically = false;

            _animation[clip.name].speed = 0;
            _animation.Play(clip.name);

            cyclePeriod *= playPreSegment;
        }

        protected override void OnFixedUpdate() {
            if (!_animation.IsPlaying(clip.name)) {
                _animation.Play(clip.name);
            }
            _animation[clip.name].time = _value;
        }

        public override void InteractFinished() {

        }

        protected override void UpdateTransform() {
            _segment = m_segment.Value;
            if (cycle) {
                _segment = Mathf.Abs(_segment) % cyclePeriod;
                float half = cyclePeriod / 2f;

                if (_segment > half) {
                    _segment = cyclePeriod - _segment;
                }
            }

            _value = _segment * _timePreSegment;
        }

        private void UpdateSegment() {
        }
    }
}