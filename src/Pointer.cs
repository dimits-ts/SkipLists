using System;

namespace SkipLists {
    /// <summary>
    /// A reference wrapper pointing to an object or struct, in order to 
    /// make the return value nullable in every case.
    /// </summary>
    /// <typeparam name="T">The type of the object boxed in the wrapper.</typeparam>
    internal sealed class Pointer<T> {
        private readonly T value;
        private readonly bool isNull;
        
        public T Value {
            get {
                if (isNull)
                    throw new ArgumentNullException("There is no value stored in the pointer");
                return value;
            }
        }

        public bool IsNull {
            get {
                return isNull;
            }
        }

        public Pointer(T value) {
            this.value = value;
            isNull = false;
        }

        public Pointer() {
            this.value = default(T);
            isNull = true;
        }
    }
}
