using System;
using UnityEngine;

namespace Phos.Callback {
	public abstract class CallbackProvider<T> : MonoBehaviour {
		private Action<T> _callback;

        protected void Post(T t) {
            _callback?.Invoke(t);
        }

		public void Register(Action<T> callback) {
			_callback += callback;
		}

		public void Register(ICallbackListener<T> listener) {
			_callback += listener.OnCallback;
		}

		public void Unregister(Action<T> callback) {
			_callback -= callback;
		}

		public void Unregister(ICallbackListener<T> listener) {
			_callback -= listener.OnCallback;
		}
	}
}