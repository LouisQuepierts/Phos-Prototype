using System;
using Phos.Navigate;
using UnityEngine;

namespace Phos {
    [RequireComponent(typeof(Renderer))]
    public class ClickHighlight : MonoBehaviour {
        private static readonly int Highlight = Shader.PropertyToID("_Highlight");
        private float _value;
        private Renderer _renderer;
        private MaterialPropertyBlock _block;

        private void Awake() {
            _block = new MaterialPropertyBlock();
            _renderer = GetComponent<Renderer>();
            _renderer.GetPropertyBlock(_block);
            
            _block.SetFloat(Highlight, 0.0f);
            _renderer.SetPropertyBlock(_block);
        }

        public void Click(NavigateNode clicked) {
            transform.position = clicked.GetNodePosition() + clicked.transform.up * 0.01f;
            transform.rotation = clicked.transform.rotation;
            _value = 1.0f;
        }

        private void Update() {
            if (_value == 0.0f) return;
            _value -= Time.deltaTime * 2.0f;

            if (_value <= 0.0f) {
                _value = 0.0f;
            }
            
            _block.SetFloat(Highlight, _value);
            _renderer.SetPropertyBlock(_block);
        }
    }
}