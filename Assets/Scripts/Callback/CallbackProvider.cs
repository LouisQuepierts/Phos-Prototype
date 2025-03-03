using System;
using UnityEngine;

namespace Phos.Callback {
	public abstract class CallbackProvider<T> : MonoBehaviour {
		private Action<T> m_Callback;

        protected void Post(T t) {
            m_Callback?.Invoke(t);
        }

		public void Register(Action<T> callback) {
			m_Callback += callback;
		}

		public void Register(ICallbackListener<T> listener) {
			m_Callback += listener.OnCallback;
		}

		public void Unregister(Action<T> callback) {
			m_Callback -= callback;
		}

		public void Unregister(ICallbackListener<T> listener) {
			m_Callback -= listener.OnCallback;
		}
	}
}