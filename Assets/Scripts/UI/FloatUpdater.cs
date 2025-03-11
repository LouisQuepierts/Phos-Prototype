using System;
using UnityEngine;

namespace Phos.UI {
    [ExecuteAlways]
    public class FloatUpdater : MonoBehaviour {
        public Material material;
        public String uniform;
        public float value;

        private float _value;

        private void Start() {
            if (material == null) {
                enabled = false;
            }
        }

        private void Update() {
            if (_value != value) {
                _value = value;
                material.SetFloat(uniform, value);
            }
        }
    }
}