using Phos.Callback;
using UnityEngine;

namespace Phos.Operation {
	public abstract class BaseBiOperation : MonoBehaviour, ICallbackListener<bool> {
		public abstract void Execute(bool trigger);

        // ReSharper disable Unity.PerformanceAnalysis
        public void OnCallback(bool t) {
            Execute(t);
        }
    }
}