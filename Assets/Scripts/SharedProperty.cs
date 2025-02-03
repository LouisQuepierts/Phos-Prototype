using System;

namespace Phos {
    public class SharedProperty<T> : ReadonlyProperty<T> {
        public SharedProperty(T value) : base(value) { }

        public new T Value {
            get { return value; } set { base.value = value; }
        }
    }
}