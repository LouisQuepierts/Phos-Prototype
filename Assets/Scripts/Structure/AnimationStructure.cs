using UnityEditor.Animations;
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
        private float _value;

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
            float segment = m_segment.Value;
            if (cycle) {
                segment = Mathf.Abs(segment) % cyclePeriod;
                float half = cyclePeriod / 2f;

                if (segment > half) {
                    segment = cyclePeriod - segment;
                }
            }

            _value = segment * _timePreSegment;
        }

        private void UpdateSegment() {
        }
    }
}