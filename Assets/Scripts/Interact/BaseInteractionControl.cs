using System;
using Phos.Callback;
using Phos.Navigate;
using Phos.Structure;
using System.Collections.Generic;
using Phos.Perform;
using Phos.Utils;
using UnityEngine;

namespace Phos.Interact {
    public abstract class BaseInteractionControl : 
        CallbackProvider<StructureControl.CallbackContext>, IToggleable {
        [Header("Alignment Properties")]
        [Range(0.1f, 1f)]
        public float alignTime = 0.1f;
        [Range(0.1f, 1f)]
        public float overshootThreshold = .25f;
        [Range(0f, 1f)]
        public float overshootCeoficient = .6f;
        [Range(0f, 1f)]
        public float reboundCeofficient = .5f;

        public bool hardEdge;

        [Header("Segment Properties")]
        [Range(-32, 1)]
        public int minSegment;
        [Range(-1, 32)]
        public int maxSegment;

        [Header("Structure Properties")]
        public List<StructureControl> structures = new();

        public List<DragHandle> handles = new();

        public Axis axis;

        public bool active = true;

        private Vector3 _start;

        private bool _aligning;
        private bool _overshooting;

        private List<DragHandle> _handles = new();
        private bool _pressed;
        private bool _hovered;

        public SharedProperty<float> _segment = new(0f);
        public SharedProperty<float> _highlight = new(0f);

        private int _lastSegment;

        private Animation _animation;
        private AnimationClip _clip;

        public float Segment => _segment.Value;

        protected abstract bool Raycast(Ray ray, out float enter, bool update);

        protected abstract bool ShouldOvershoot();

        protected abstract void PerformDrag(Vector3 start, Vector3 current);

        protected abstract bool PerformAlign();

        protected abstract bool PerformOvershoot();

        protected abstract float UpdateSegment();

        protected abstract void MoveTo(int delta);

        public void Toggle(bool enable) {
            if (this.active == enable) return;
            this.active = enable;
            _hovered = false;

            if (!_animation || !_animation.clip) return;
            Debug.Log($"Toggle {enable}");
            var clip = _animation.clip;
            var state = _animation[clip.name];
            state.speed = enable ? 1 : -1;
            
            if (_animation.isPlaying) return;
            state.time = enable ? 0 : clip.length;
            _animation.Play();
        }

        public void MousePressed() {
            if (!active) {
                return;
            }

            _pressed = true;
            _aligning = false;
            _overshooting = false;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Raycast(ray, out float enter, true)) {
                _start = ray.GetPoint(enter);
            }

            Post(new StructureControl.CallbackContext(
                _segment.Value, StructureControl.CallbackType.InteractBegin
            ));
        }

        /// <summary>
        /// Handle mouse dragging action
        /// </summary>
        public void MouseDragging() {
            if (!active) {
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Raycast(ray, out float enter, false)) {
                Vector3 current = ray.GetPoint(enter);

                PerformDrag(_start, current);
                _segment.Value = UpdateSegment();

                _start = current;
            }

        }

        /// <summary>
        /// Handle mouse released action
        /// </summary>
        public void MouseReleased() {
            _overshooting = ShouldOvershoot();
            _pressed = false;
            _aligning = true;
            InteractFinished();
        }

        public void MoveDelta(int delta) {
            if (delta == 0) return;
            _aligning = true;
            MoveTo(Mathf.RoundToInt(_segment.Value) + delta);
        }

        private void InteractFinished() {
            foreach (var structure in structures) {
                structure.InteractFinished();
            }

            Post(new StructureControl.CallbackContext(
                _segment.Value, StructureControl.CallbackType.InteractFinished
            ));
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void AlignFinished() {
            foreach (var structure in structures) {
                structure.AlignFinished();
            }

            Post(new StructureControl.CallbackContext(
                _segment.Value, StructureControl.CallbackType.AlignFinished
            ));

            PathManager controller = PathManager.GetInstance();
            PlayerController player = SceneController.Instance.Player;

            controller.UpdateAccessable(player.current);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void OnChangeSegment() {
            foreach (var structure in structures) {
                structure.SegmentChanged();
            }

            Post(new StructureControl.CallbackContext(
                _segment.Value, StructureControl.CallbackType.ChangeSegment
            ));

            PathManager controller = PathManager.GetInstance();
            PlayerController player = SceneController.Instance.Player;

            // Debug.Log("Update Accessible");
            controller.UpdateAccessable(player.current);
        }

        public void AddHandle(DragHandle handle) {
            _handles.Add(handle);
            handle.Bind(this, _highlight);
        }

        public void SetHovered(bool hovered) {
            _hovered = hovered;
        }

        private void FixedUpdate() {
            bool aligning = _aligning;
            if (_aligning) {
                if (_overshooting) {
                    if (overshootCeoficient > 0 && PerformOvershoot()) {
                        _overshooting = false;
                    }
                } else {
                    if (PerformAlign()) {
                        _aligning = false;
                    }
                }

                _segment.Value = Mathf.Round(UpdateSegment() * 1000f) / 1000f;
            }

            if (_aligning != aligning) {
                AlignFinished();
            }

            int rounded = Mathf.RoundToInt(_segment.Value);
            if (rounded != _lastSegment) {
                OnChangeSegment();
            }
            _lastSegment = rounded;
        }

        private void Update() {
            float highlightValue = (_hovered || _pressed) ? 1f : -1f;
            float value = _highlight.Value;
            _highlight.Value = Mathf.Clamp(value + highlightValue * Time.deltaTime * 5f, 0f, 1f);
        }

        private void OnEnable() {
            foreach (var structure in structures) {
                structure.BindProperty(_segment);
            }

            foreach (var handle in handles) {
                AddHandle(handle);
            }
        }

        private void Start() {
            _animation = GetComponent<Animation>();
            
            if (!_animation) return;
            _clip = _animation.clip;
                
            if (active) return;
            _animation[_clip.name].time = _clip.length;
        }
    }
}