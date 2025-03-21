using System;
using Phos.Callback;
using Phos.Operation;
using Phos.Trigger.Predicate;
using UnityEngine;

namespace Phos.Trigger {
    public class TriggerController : CallbackProvider<bool> {
        public bool invert;

        public GameObject operations;
        public GameObject predicates;

        public bool Value => _enabled.Value;

        private SharedProperty<bool> _enabled;
        private BaseBiOperation[] _operations;
        private BaseTriggerPredicate[] _predicates;

        private TriggerContext _context;

        public TriggerContext Context => _context;

        private void Start() {
            _enabled = new SharedProperty<bool>(!invert);
            _operations = (!operations ? gameObject : operations).GetComponents<BaseBiOperation>();
            _predicates = (!predicates ? gameObject : predicates).GetComponents<BaseTriggerPredicate>();

            _context = new TriggerContext(this);
        }

        public void Trigger() {
            bool triggered = _predicates.Length == 0 || Array.TrueForAll(_predicates, predicate => predicate.Evaluate(_context));
            _enabled.Value = _context.NewValue;

            if (!triggered) return;
            
            Post(Value);
            foreach (var operation in _operations) {
                operation.Execute(Value);
            }
        }
    }
    
    public class TriggerContext {
        public readonly TriggerController TriggerController;
        
        public bool NewValue;
        public GameObject Collider;
        
        public TriggerContext(TriggerController triggerController) {
            TriggerController = triggerController;
        }
    }
}