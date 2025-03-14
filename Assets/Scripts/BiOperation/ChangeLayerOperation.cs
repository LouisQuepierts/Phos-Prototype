using Phos.Navigate;
using Phos.Operation;
using UnityEngine;

namespace Phos.BiOperation {
    public class ChangeLayerOperation : BaseBiOperation {
        public GameObject[] targets;

        [Header("Layer")]
        public string off;
        public string on;

        private int m_off;
        private int m_on;

        private void Start() {
            m_off = LayerMask.NameToLayer(off);
            m_on = LayerMask.NameToLayer(on);
        }

        public override void Execute(bool trigger) {
            if (!enabled) {
                return;
            }

            int layer = trigger ? m_on : m_off;
            foreach (var obj in targets) {
                obj.layer = layer;
            }
        }
    }
}