namespace SkipLists {
    internal class Node<K,V> {
        internal Node<K,V> next;
        internal Node<K,V> below;
        internal K key;
        internal V value;

        internal Node(K key, V value) {
            this.key = key;
            this.value = value;
            next = null;
            below = null;
        }

        internal Node(): this(default(K), default(V)) {}

        public override string ToString() {
            return key.ToString();
        }
    }
}
