using System;

namespace Phos {
    public class ReadonlyProperty<T> {
        protected T value;
        public ReadonlyProperty(T value) {
            this.value = value;
        }

        public T Value => value;
    }
}