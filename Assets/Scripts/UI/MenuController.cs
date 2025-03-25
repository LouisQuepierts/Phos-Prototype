using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Phos.UI {
    public class MenuController : MonoBehaviour {
        public AnimationClip clip;
        public bool _show;
        
        private Animation _animation;
        private bool _animating;
        
        public void Awake() {
            if (clip)
                _animation = InitAnimation();
        }
        
        public void Update() {
            if (!clip || _animation.isPlaying || !_animating) return;
            _animating = false;

            if (!_show) {
                gameObject.SetActive(false);
            }
        }

        public void Toggle(bool enable) {
            if (_show == enable) return;
            _show = enable;
            
            if (clip) {
                _animation ??= InitAnimation();

                _animating = false;
                var state = _animation[clip.name];
                state.speed = _show ? 1f : -1f;
                
                if (!_animation.isPlaying) {
                    state.time = _show ? 0f : clip.length;
                    _animation.Play(clip.name);
                }
            }

            if (_show) {
                gameObject.SetActive(true);
            }
            else {
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