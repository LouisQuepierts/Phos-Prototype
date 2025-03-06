using Phos.Callback;
using Phos.Navigate;
using Phos.Structure;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Phos.Interact {
    public abstract partial class BaseInteractionControl : CallbackProvider<StructureControl.CallbackContext> {
        [Header("Alignment Properties")]
        [Range(0.1f, 1f)]
        public float alignTime = 0.1f;
        [Range(0.1f, 1f)]
        public float overshootThreshold = .25f;
        [Range(0f, 1f)]
        public float overshootCeoficient = .6f;
        [Range(0f, 1f)]
        public float reboundCeofficient = .5f;

        public bool hardEdge = false;

        [Header("Segment Proeprties")]
        [Range(-32, 1)]
        public int minSegment = 0;
        [Range(-1, 32)]
        public int maxSegment = 0;

        [Header("Structure Properties")]
        public List<StructureControl> structures = new();

        public List<DragHandle> handles = new();

        public Axis axis;

        public bool active = true;

        private Vector3 m_start;

        private bool m_aligning;
        private bool m_overshooting;

        private List<DragHandle> m_handles = new();
        private bool m_pressed = false;
        private bool m_hovered = false;

        public SharedProperty<float> m_segment = new(0f);
        public SharedProperty<float> m_highlight = new(0f);

        private int m_lastSegment = 0;

        public float Segment {
            get {
                return m_segment.Value;
            }
        }

        protected abstract bool Raycast(Ray ray, out float enter, bool update);

        protected abstract bool ShouldOvershoot();

        protected abstract void PerformDrag(Vector3 start, Vector3 current);

        protected abstract bool PerformAlign();

        protected abstract bool PerformOvershoot();

        protected abstract float UpdateSegment();

        public void MousePressed() {
            if (!active) {
                return;
            }

            m_pressed = true;
            m_aligning = false;
            m_overshooting = false;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Raycast(ray, out float enter, true)) {
                m_start = ray.GetPoint(enter);
            }

            Post(new StructureControl.CallbackContext(
                m_segment.Value, StructureControl.CallbackType.InteractBegin
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

                PerformDrag(m_start, current);
                m_segment.Value = UpdateSegment();

                m_start = current;
            }

        }

        /// <summary>
        /// Handle mouse released action
        /// </summary>
        public void MouseReleased() {
            m_overshooting = ShouldOvershoot();
            m_pressed = false;
            m_aligning = true;
            InteractFinished();
        }

        private void InteractFinished() {
            foreach (var structure in structures) {
                structure.InteractFinished();
            }

            Post(new StructureControl.CallbackContext(
                m_segment.Value, StructureControl.CallbackType.InteractFinished
            ));
        }

        private void AlignFinished() {
            foreach (var structure in structures) {
                structure.AlginFinished();
            }

            Post(new StructureControl.CallbackContext(
                m_segment.Value, StructureControl.CallbackType.AlignFinished
            ));

            PathManager controller = PathManager.GetInstance();
            PlayerController player = PlayerController.GetInstance();


            controller.UpdateAccessable(player.current);
        }

        private void OnChangeSegment() {
            foreach (var structure in structures) {
                structure.SegmentChanged();
            }

            Post(new StructureControl.CallbackContext(
                m_segment.Value, StructureControl.CallbackType.ChangeSegment
            ));

            PathManager controller = PathManager.GetInstance();
            PlayerController player = PlayerController.GetInstance();

            Debug.Log("Update Accessable");
            controller.UpdateAccessable(player.current);
        }

        public void AddHandle(DragHandle handle) {
            m_handles.Add(handle);
            handle.Bind(this, m_highlight);
        }

        public void SetHovered(bool hovered) {
            m_hovered = hovered;
        }

        private void FixedUpdate() {
            bool aligning = m_aligning;
            if (m_aligning) {
                if (m_overshooting) {
                    if (overshootCeoficient > 0 && PerformOvershoot()) {
                        m_overshooting = false;
                    }
                } else {
                    if (PerformAlign()) {
                        m_aligning = false;
                    }
                }

                m_segment.Value = Mathf.Round(UpdateSegment() * 1000f) / 1000f;
            }

            if (m_aligning != aligning) {
                AlignFinished();
            }

            int rounded = Mathf.RoundToInt(m_segment.Value);
            if (rounded != m_lastSegment) {
                OnChangeSegment();
            }
            m_lastSegment = rounded;
        }

        private void Update() {
            float highlightValue = (m_hovered || m_pressed) ? 1f : -1f;
            float value = m_highlight.Value;
            m_highlight.Value = Mathf.Clamp(value + highlightValue * Time.deltaTime * 5f, 0f, 1f);
        }

        private void OnEnable() {
            foreach (var structure in structures) {
                structure.BindProperty(m_segment);
            }

            foreach (var handle in handles) {
                AddHandle(handle);
            }
        }

        public float Segement => m_segment.Value;
    }
}