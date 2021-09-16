using System;
using System.Collections.Generic;

namespace SkipLists {

    internal sealed class ReadOnlySet<T> : SkipListSet<T> where T : class {
        private static readonly string errorMessage = "Write access denied on read-only collection.";

        public ReadOnlySet(SkipList<T, T> set) {
            this.set = set;
        }


        public override bool IsReadOnly {
            get {
                return true;
            }
        }

        public override bool Add(T item) {
            throw new NotSupportedException(errorMessage);
        }

        public override void Clear() {
            throw new NotSupportedException(errorMessage);
        }

        public override bool Remove(T key) {
            throw new NotSupportedException(errorMessage);
        }

        public override void ExceptWith(IEnumerable<T> other) {
            throw new NotSupportedException(errorMessage);
        }

        public override void IntersectWith(IEnumerable<T> other) {
            throw new NotSupportedException(errorMessage);
        }

        public override void SymmetricExceptWith(IEnumerable<T> other) {
            throw new NotSupportedException(errorMessage);
        }

        public override void UnionWith(IEnumerable<T> other) {
            throw new NotSupportedException(errorMessage);
        }

    }

}
