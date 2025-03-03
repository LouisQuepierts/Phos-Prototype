using Phos.Callback;
using UnityEngine;

namespace Phos.Layer {
    public class LayerChanger : MonoBehaviour, ICallbackListener<bool> {
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

        public void OnCallback(bool t) {
            Debug.Log(t);
            if (t) {
                target.layer = m_right;
            } else {
                target.layer = m_left;
            }
        }
    }
}