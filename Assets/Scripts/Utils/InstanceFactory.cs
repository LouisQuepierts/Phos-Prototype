using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Phos.Utils {
    public class InstanceFactory<TKey, TValue> : Factory<TKey, TValue> {
        private readonly Func<TValue> _default;
        private IDictionary<TKey, Func<TValue>> _dictionary;

        public InstanceFactory(Func<TValue> @default, int initialCapacity = 16) {
            _default = @default;
            _dictionary = new Dictionary<TKey, Func<TValue>>(initialCapacity);
        }

        protected override void FreezeImpl() {
            _dictionary = new ReadOnlyDictionary<TKey, Func<TValue>>(_dictionary);
        }

        protected override TValue Get(TKey key) {
            if (_dictionary.TryGetValue(key, out Func<TValue> constructor)) {
                return constructor();
            }
            return _default();
        }

        public void Register(TKey key, Func<TValue> constructor) {
            _dictionary[key] = constructor;
        }
    }
}