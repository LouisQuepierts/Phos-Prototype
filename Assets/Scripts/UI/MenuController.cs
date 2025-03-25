using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Phos.UI {
    public class MenuController : MonoBehaviour {
        public AnimationClip clip;
        public bool init;
        public bool show;
        private bool _show;
        
        private Animation _animation;
        private bool _animating;
        
        public void Awake() {
            if (clip)
                _animation = InitAnimation();
        }

        private void Start() {
            if (init) {
                Toggle(show, 1, false);
            }
        }

        public void Update() {
            if (!clip || _animation.isPlaying || !_animating) return;
            _animating = false;

            if (!_show) {
                gameObject.SetActive(false);
            }
        }

        public void Toggle(bool enable, float speed = 1, bool animate = true) {
            if (_show == enable) return;
            _show = enable;
            
            if (clip) {
                _animation ??= InitAnimation();

                _animating = false;
                var state = _animation[clip.name];
                state.speed = (_show ? 1f : -1f) * speed;
                if (animate) {
                    state.time = _show ? 0f : clip.length;
                }
                else {
                    state.time = _show ? clip.length : 0f;
                }
                
                if (!_animation.IsPlaying(clip.name)) {
                    _animation.Play(clip.name);
                }
            }

            if (_show) {
                gameObject.SetActive(true);
            }
            else if (clip && animate) {
                _animating = true;
            }
        }

        private Animation InitAnimation() {
            Animation animation = GetComponent<Animation>();
            animation.AddClip(clip, clip.name);
            return animation;
        }
    }
}