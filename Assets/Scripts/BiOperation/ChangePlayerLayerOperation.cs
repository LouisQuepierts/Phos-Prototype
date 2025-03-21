using Phos.Operation;
using Phos.Perform;
using UnityEngine;

namespace BiOperation {
    public class ChangePlayerLayerOperation : BaseBiOperation {
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
            var playerGameObject = SceneController.Instance.Player.gameObject;
            playerGameObject.transform.GetChild(0).gameObject.layer = layer;
        }
    }
}