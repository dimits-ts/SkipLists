namespace SkipLists {

    internal class ConcurrentSkipList<K,V> : SkipList<K, V> where V : class {

        public ConcurrentSkipList(SkipList<K,V> otherList) : base(otherList.Comparer) {
            foreach (var pair in otherList.GetEntries())
                Insert(pair.Key, pair.Value);
        }

        private protected override  Node<K, V> BuildNode(K key, V value) {
            return new ConcurrentNode<K, V>(key, value);
        }
        private protected override Node<K, V> BuildNode() {
            return new ConcurrentNode<K, V>();
        }
    }

}
