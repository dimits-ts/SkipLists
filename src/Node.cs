namespace SkipLists {

    internal class Node<K,V> {
        private protected Node<K,V> next;
        private protected Node<K,V> below;
        internal protected K key;
        internal protected V value;

        internal virtual Node<K,V> Next {
            get;
            set;
        }

        internal virtual Node<K,V> Below {
            get;
            set;
        }

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
