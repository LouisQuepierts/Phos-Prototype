using System.Collections.Generic;
using System.Linq;
using Phos.Optical;
using UnityEngine;

namespace Phos.Trigger {
    [RequireComponent(typeof(TriggerController))]
    public class OpticalTriggerBehaviour : OpticalDevice, ILightAcceptable {
        private static readonly List<LightData> EmptyList = Enumerable.Empty<LightData>().ToList();

        private TriggerController _trigger;

        private bool _hit;
        private bool _wasHit;

        private void Start() {
            _trigger = GetComponent<TriggerController>();
        }

        public void OnLightHitted(LightData income, RaycastHit hit, out List<LightData> outgo) {
            outgo = EmptyList;

            var dot = Vector3.Dot(hit.normal, transform.forward);
            if (dot > 0.9f) return;

            _hit = true;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public override void LateOpticalUpdate() {
            if (_hit == _wasHit) return;
            
            var context = _trigger.Context;
            context.NewValue = _hit;
            _trigger.Trigger();
            _wasHit = _hit;
            _hit = false;
        }
    }
}