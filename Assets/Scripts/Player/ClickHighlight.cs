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

        private int _mask;

        private void Awake() {
            _block = new MaterialPropertyBlock();
            _renderer = GetComponent<Renderer>();
            _renderer.GetPropertyBlock(_block);
            
            _block.SetFloat(Highlight, 0.0f);
            _renderer.SetPropertyBlock(_block);

            _mask = ~LayerMask.GetMask("Node");
        }

        public void Click(NavigateNode clicked) {
            var up = clicked.transform.up;
            Vector3 position = clicked.GetNodePosition();
            if (Physics.Raycast(position + up * 0.5f, -up, out RaycastHit hit, 0.5f, _mask)) {
                position = hit.point;
            }

            Vector3 normal = clicked.GetNodeNormal();
            transform.position = position + normal * 0.05f;
            transform.up = normal;
            _value = 1.0f;
            gameObject.layer = clicked.transform.parent.gameObject.layer;
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