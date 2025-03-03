using Phos.Callback;
using Phos.Predicate;
using Phos.Structure;
using UnityEngine;

namespace Phos.Condition {
    public class GlobalCondition : CallbackProvider<bool>, ICallbackListener<object> {

        [Header("Predicate")]
        public CallbackProvider<StructureControl.CallbackContext> callback;
        public PredicateHolder predicate;

        public void OnCallback(object t) {
            bool value = predicate.Evaluate();
            Post(value);
        }
    }
}