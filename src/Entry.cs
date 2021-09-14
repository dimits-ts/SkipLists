namespace SkipLists {

    /// <summary>
    /// An immutable struct holding a key-value pair.
    /// </summary>
    /// <typeparam name="K">The type of the key.</typeparam>
    /// <typeparam name="V">The type of the value.</typeparam>
    public readonly struct Entry<K,V> {
        private readonly K key;
        private readonly V value;

        internal Entry(Node<K,V> node) {
            key = node.key;
            value = node.value;
        }

        public K Key {
            get {
                return key;
            }
        }

        public V Value {
            get {
                return value;
            }
        }

        public override string ToString() {
            return key + "-" + value;
        }
    }
}
