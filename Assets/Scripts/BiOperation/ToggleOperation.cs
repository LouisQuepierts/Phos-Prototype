using System;
using System.Collections.Generic;
using Phos.Operation;
using Phos.Utils;
using UnityEngine;

namespace BiOperation {
    public class ToggleOperation : BaseBiOperation {
        public GameObject[] objects;
        public Component[] targets;
        public bool invert;

        private List<IToggleable> _toggles;

        private void Start() {
            _toggles = new List<IToggleable>(objects.Length + targets.Length);
            foreach (var component in targets) {
                if (component is IToggleable toggle) {
                    _toggles.Add(toggle);
                }
            }
            
            foreach (var obj in objects) {
                if (obj.TryGetComponent(out IToggleable toggle)) {
                    _toggles.Add(toggle);
                }
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public override void Execute(bool trigger) {
            bool value = trigger != invert;
            foreach (var target in _toggles) {
                target.Toggle(value);
            }
        }
    }
}