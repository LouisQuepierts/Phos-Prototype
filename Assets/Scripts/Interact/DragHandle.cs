using System;
using System.Collections.Generic;
using UnityEngine;

namespace Phos.Interact {
	public class DragHandle : MonoBehaviour {
        private ReadonlyProperty<float> m_highlight;

		private BaseInteractionControl m_control;
        private Material m_material;

        private void Update() {
            if (m_highlight != null) {
                m_material.SetFloat("_Highlight", m_highlight.Value);
            }
        }

        private void Awake() {
            List<Material> materials = new();
            GetComponentInParent<Renderer>().GetMaterials(materials);
            m_material = materials[0];
        }

        private void OnMouseDown() {
            m_control?.MousePressed();
        }

        private void OnMouseDrag() {
            m_control?.MouseDragging();
        }

        private void OnMouseUp() {
            m_control?.MouseReleased();
        }

        private void OnMouseEnter() {
            m_control?.SetHovered(true);
        }

        private void OnMouseExit() {
            m_control?.SetHovered(false);
        }

        internal void Bind(BaseInteractionControl control, SharedProperty<float> property) {
            m_control = control;
            m_highlight = property;
        }
    }
}