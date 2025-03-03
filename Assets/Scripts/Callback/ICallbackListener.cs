namespace Phos.Callback {
	public interface ICallbackListener<T> {
		public abstract void OnCallback(T t);
	}
}