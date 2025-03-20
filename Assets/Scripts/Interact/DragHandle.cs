using System;
using System.Collections.Generic;
using UnityEngine;

namespace Phos.Interact {
    [RequireComponent(typeof(Renderer))]
	public class DragHandle : MonoBehaviour {
        private static readonly int Highlight = Shader.PropertyToID("_Highlight");
        private ReadonlyProperty<float> _highlight;

		private BaseInteractionControl _control;

        private Renderer _renderer;
        private MaterialPropertyBlock _block;

        private void Update() {
            if (_highlight == null) return;
            
            _block.SetFloat(Highlight, _highlight.Value);
            _renderer.SetPropertyBlock(_block, 0);
        }

        private void Awake() {
            List<Material> materials = new();
            _block = new MaterialPropertyBlock();
            
            _renderer = GetComponent<Renderer>();
            _renderer.GetPropertyBlock(_block, 0);
        }

        private void OnMouseDown() {
            _control?.MousePressed();
        }

        private void OnMouseDrag() {
            _control?.MouseDragging();
        }

        private void OnMouseUp() {
            _control?.MouseReleased();
        }

        private void OnMouseEnter() {
            if (!_control || !_control.active) return; 
            _control?.SetHovered(true);
        }

        private void OnMouseExit() {
            if (!_control || !_control.active) return; 
            _control?.SetHovered(false);
        }

        internal void Bind(BaseInteractionControl control, SharedProperty<float> property) {
            _control = control;
            _highlight = property;
        }
    }
}