using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;

namespace Phos.Utils {
    public class SingletonFactory<TKey, TValue> : Factory<TKey, TValue> {
        private readonly TValue _default;
        private IDictionary<TKey, TValue> _dictionary;

        public SingletonFactory(TValue @default, int initialCapacity = 16) {
            _default = @default;
            _dictionary = new Dictionary<TKey, TValue>(initialCapacity);
        }

        protected override void FreezeImpl() {
            _dictionary = new ReadOnlyDictionary<TKey, TValue>(_dictionary);
        }

        protected override TValue Get(TKey key) {
            if (_dictionary.TryGetValue(key, out var value)) { 
                return value;
            }
            return _default;
        }

        public void Register(TKey key, TValue instance) {
            _dictionary[key] = instance;
        }
    }
}