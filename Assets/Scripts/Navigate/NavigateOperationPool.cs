using System;
using System.Collections.Generic;
using UnityEngine;

namespace Phos.Navigate {
    public class NavigateOperationPool : IDisposable {
        private readonly Stack<NavigateOperation> _pool = new();
        private readonly object _lock = new object();
        private readonly int _maxSize;
        private int _activeCount;

        public NavigateOperationPool(int initialSize = 10, int maxSize = 100) {
            _maxSize = maxSize;
            for (int i = 0; i < initialSize; i++) {
                _pool.Push(CreateNew());
            }
        }

        public NavigateOperation Get() {
            lock (_lock) {
                if (_pool.Count == 0 && _activeCount < _maxSize) {
                    return CreateNew();
                }
                return _pool.Pop();
            }
        }

        public void Release(NavigateOperation operation) {
            lock (_lock) {
                Debug.Log("Release");
                if (_pool.Count < _maxSize) {
                    _pool.Push(operation);
                }
                _activeCount--;
            }
        }

        private NavigateOperation CreateNew() {
            _activeCount++;
            return new NavigateOperation();
        }

        public void Dispose() {
            lock (_lock) {
                _pool.Clear();
            }
        }
    }
}