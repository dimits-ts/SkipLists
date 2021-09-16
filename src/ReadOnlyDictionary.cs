using System;
using System.Collections.Generic;

namespace SkipLists {
    internal class ReadOnlyDictionary<K,V> : SkipListDictionary<K,V> where V : class {

        private static readonly string errorMessage = "Write access denied on read-only collection.";

        public ReadOnlyDictionary(SkipList<K,V> dict) {
            this.dict = dict;
        }

        public override bool IsReadOnly {
            get {
                return true;
            }
        }

        public override V this[K key] {
            set {
                throw new NotSupportedException(errorMessage);
            }
        }

        public override void Add(K key, V value) {
            throw new NotSupportedException(errorMessage);
        }

        public override void Add(KeyValuePair<K, V> item) {
            throw new NotSupportedException(errorMessage);
        }

        public override void Clear() {
            throw new NotSupportedException(errorMessage);
        }

        public override bool Remove(K key) {
            throw new NotSupportedException(errorMessage);
        }

        public override bool Remove(KeyValuePair<K, V> item) {
            throw new NotSupportedException(errorMessage);
        }

    }
}

