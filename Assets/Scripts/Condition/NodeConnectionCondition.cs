using Phos.Callback;
using Phos.Navigate;
using Phos.Predicate;
using Phos.Structure;
using Phos.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Phos.Condition {
    
    public class NodeConnectionCondition : MonoBehaviour, ICallbackListener<StructureControl.CallbackContext> {
        [Header("Node Connection")]
        public NavigateNode targetNode;

        [Header("Predicate")]
        public PredicateHolder predicate;

        [Header("Callback")]
        public CallbackProvider<StructureControl.CallbackContext> callback;

        private NodePath m_path;

        private void Start() {
            if (!transform.parent.TryGetComponent<NavigateNode>(out var node)) {
                Debug.LogError("NodeConnectionCondition must be a child of a NavigateNode");
                gameObject.SetActive(false);
                return;
            }

            m_path = node.Paths[targetNode];
            if (m_path == null) {
                Debug.LogError($"NodeConnectionCondition: {node} does not have a path to {targetNode}");
                gameObject.SetActive(false);
                return;
            }

            if (callback != null) {
                callback.Register(this);
            }
        }

        private void OnDestroy() {
            if (callback != null) {
                callback.Unregister(this);
            }
        }

        public void OnCallback(StructureControl.CallbackContext t) {
            if (predicate != null) {
                m_path.active = predicate.Evaluate(m_path.active);
            }
        }

        private void FixedUpdate() {
            if (callback == null && predicate != null) {
                m_path.active = predicate.Evaluate(m_path.active);
            }
        }
    }
}