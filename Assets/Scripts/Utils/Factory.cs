using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Phos.Utils {
	public abstract class Factory<TKey, TValue> {
		private bool frozen = false;

		public bool IsFrozen { get => frozen; }

		public TValue this[TKey key] {
			get {
				return Get(key);
			}
		}

		public TValue GetInstance(TKey key) {
			return Get(key);
		}

		public void Freeze() {
			frozen = true;
			FreezeImpl();
		}

		protected abstract TValue Get(TKey key);

		protected abstract void FreezeImpl();
	}
}