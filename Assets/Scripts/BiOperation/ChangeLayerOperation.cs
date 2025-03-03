using Phos.Navigate;
using Phos.Operation;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Phos.BiOperation {
    public class ChangeLayerOperation : BaseBiOperation {
        public GameObject target;

        [Header("Layer")]
        public string left;
        public string right;

        private int m_left;
        private int m_right;

        private void Start() {
            Debug.Log("Awake");

            m_left = LayerMask.NameToLayer(left);
            m_right = LayerMask.NameToLayer(right);
        }

        public override void Execute(bool trigger) {
            if (!enabled) {
                return;
            }

            if (trigger) {
                target.layer = m_right;
            } else {
                target.layer = m_left;
            }
        }
    }
}