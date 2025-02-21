using Phos.Structure;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Phos.Interact {
    public abstract partial class InteractionControl : MonoBehaviour {
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

        private Vector3 m_start;

        private bool m_aligning;
        private bool m_overshooting;

        private List<DragHandle> m_handles = new();
        private bool m_pressed = false;
        private bool m_hovered = false;

        public SharedProperty<float> m_segment = new(0f);
        public SharedProperty<float> m_highlight = new(0f);

        /// <summary>
        /// Get click point
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="enter"></param>
        /// <param name="update"></param>
        /// <returns>Has hit point</returns>
        protected abstract bool Raycast(Ray ray, out float enter, bool update);

        /// <summary>
        /// Determine whether overshoot is necessary after mouse release
        /// </summary>
        /// <returns>Should overshoot when mouse released</returns>
        protected abstract bool ShouldOvershoot();

        /// <summary>
        /// Drag control object
        /// </summary>
        /// <param name="start"></param>
        /// <param name="current"></param>
        protected abstract void PerformDrag(Vector3 start, Vector3 current);

        /// <summary>
        /// Align object to nearest _value
        /// </summary>
        /// <returns>Does align finished</returns>
        protected abstract bool PerformAlign();

        /// <summary>
        /// Overshoot when align to a neart _value with long distance
        /// </summary>
        /// <returns>Does overshoot ended</returns>
        protected abstract bool PerformOvershoot();

        /// <summary>
        /// Get _value value after moved, do not round
        /// </summary>
        /// <returns>_value count</returns>
        protected abstract float UpdateSegment();

        /// <summary>
        /// Handle mouse pressed action
        /// </summary>
        public void MousePressed() {
            m_pressed = true;
            m_aligning = false;
            m_overshooting = false;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Raycast(ray, out float enter, true)) {
                m_start = ray.GetPoint(enter);
            }
        }

        /// <summary>
        /// Handle mouse dragging action
        /// </summary>
        public void MouseDragging() {
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
        }

        public void AddHandle(DragHandle handle) {
            m_handles.Add(handle);
            handle.Bind(this, m_highlight);
        }

        public void SetHovered(bool hovered) {
            m_hovered = hovered;
        }

        private void FixedUpdate() {
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