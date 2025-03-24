using Phos.Operation;
using UnityEngine;

namespace BiOperation {
    public class AnimationOperation : BaseBiOperation {
        public new Animation animation;
        public AnimationClip clip;

        private string _name;
        private AnimationState _state;

        private void OnEnable() {
            AnimationClip ac = clip ?? animation.clip;
            if (clip == null) {
                enabled = false;
                return;
            }

            animation.clip = ac;
            animation.playAutomatically = false;
            _name = ac.name;
            _state = animation[_name];
        }

        public override void Execute(bool trigger) {
            _state.speed = trigger ? 1 : -1;

            if (animation.isPlaying) return;
            
            _state.time = trigger ? 0 : animation.clip.length;
            animation.Play();
        }
    }
}