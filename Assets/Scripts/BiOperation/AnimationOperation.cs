using Phos.Operation;
using UnityEngine;

namespace BiOperation {
    public class AnimationOperation : BaseBiOperation {
        public new Animation animation;
        public AnimationClip clip;

        private string _name;
        private AnimationState _state;

        private void OnEnable() {
            if (animation.clip == null) {
                animation.AddClip(clip, clip.name);
            }

            if (clip == null) {
                enabled = false;
                return;
            }

            _name = clip.name;
            _state = animation[_name];
        }

        public override void Execute(bool trigger) {
            _state.speed = trigger ? 1 : -1;

            if (animation.IsPlaying(_name)) return;
            Debug.Log("Animation");
            
            _state.time = trigger ? 0 : animation.clip.length;
            animation.Play(_name);
        }
    }
}