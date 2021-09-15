using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SkipLists {

    /// <summary>
    /// A Sorted Dictionary implemented with a Skip List, a good and fast
    /// alternative to C#'s standard <see cref="SortedDictionary{TKey, TValue}"/>
    /// which uses Binary Trees.
    /// </summary>
    /// <typeparam name="K">The type of keys used</typeparam>
    /// <typeparam name="V">The type of values used.</typeparam>
    class SkipListDictionary<K, V> : IDictionary<K, V> where V : class {
        private SkipList<K, V> dict;

        /// <summary>
        /// Creates a dictionary with a default key comparer.
        /// </summary>
        public SkipListDictionary() {
            dict = new SkipList<K, V>();
        }

        /// <summary>
        /// Creates a dictionary with a custom key comparer.
        /// </summary>
        /// <param name="customComparer">An object that can compare objects of the key's type.</param>
        public SkipListDictionary(Comparer<K> customComparer) {
            dict = new SkipList<K, V>(customComparer);
        }

        // ============================= INTERFACE ATTRIBUTES =============================

        public V this[K key] {
            get { 
                return dict.Get(key);
            }

            set {
                bool entryExists = Remove(key);

                if (entryExists)
                    Add(key, value);
            }
        }

        public ICollection<K> Keys {
            get {
                return dict.GetKeys();
            }
        }

        public ICollection<V> Values {
            get {
                return dict.GetValues();
            }
        }

        public int Count {
            get {
                return dict.Size;
            }
        }

        public bool IsReadOnly{
            get {
                return false;
            }
        }

        // ============================= EXTENSION METHODS =============================

        /// <summary>
        /// Returns the first element of the dictionary, that is 
        /// the element with the smallest key according to the <see cref="Comparer{T}"/>.
        /// 
        /// If multiple elements with the same key exist, the last inserted will be returned.
        /// </summary>
        /// <returns>A <see cref="KeyValuePair{TKey, TValue}"/> with the lowest key.</returns>
        public KeyValuePair<K, V> Min() {
            return dict.FirstEntry();
        }

        /// <summary>
        /// Returns the last element of the dictionary, that is 
        /// the element with the biggest key according to the <see cref="Comparer{T}"/>.
        /// 
        /// If multiple elements with the same key exist, the last inserted will be returned.
        /// </summary>
        /// <returns>A <see cref="KeyValuePair{TKey, TValue}"/> with the highest key.</returns>
        public KeyValuePair<K,V> Max() {
            return dict.LastEntry();
        }

        /// <summary>
        /// Returns the entry with a key larger or equal to the provided key.
        /// </summary>
        /// <exception cref="ArgumentNullException">If the key is null.</exception>
        /// <returns>An <see cref="KeyValuePair{K, V}"/> with a key larger or equal to the provided key.</returns>
        public KeyValuePair<K, V> CeilingEntry(K key) {
            return dict.CeilingEntry(key);
        }

        /// <summary>
        /// Returns the entry with a key smaller or equal to the provided key.
        /// </summary>
        /// <exception cref="ArgumentNullException">If the key is null.</exception>
        /// <returns>An <see cref="KeyValuePair{K, V}"/> with a key smaller or equal to the provided key.</returns>
        public KeyValuePair<K, V> FloorEntry(K key) {
            return dict.FloorEntry(key);
        }

        /// <summary>
        /// Returns the entry with a key smaller than the provided key.
        /// </summary>
        /// <exception cref="ArgumentNullException">If the key is null.</exception>
        /// <returns>An <see cref="KeyValuePair{K, V}"/> with a key smaller than the provided key.</returns>
        public KeyValuePair<K, V> HigherEntry(K key) {
            return dict.HigherEntry(key);
        }

        /// <summary>
        /// Returns the entry with a key smaller than the provided key.
        /// </summary>
        /// <exception cref="ArgumentNullException">If the key is null.</exception>
        /// <returns>An <see cref="KeyValuePair{K, V}"/> with a key smaller than the provided key.</returns>
        public KeyValuePair<K, V> LowerEntry(K key) {
            return dict.LowerEntry(key);
        }

        /// <summary>
        /// Returns an ordered collection containing the elements with keys between
        /// the <c>start</c> and <c>end</c> keys.
        /// </summary>
        /// <param name="start">The lower bound of the list's elements.</param>
        /// <param name="end">The upper bound of the list's elements.</param>
        /// <exception cref="ArgumentException">
        /// If the <c>start</c> is smaller than the 
        /// <c>end</c> key according to the map's <see cref="Comparer{T}"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// If any of the keys are null.
        /// </exception>
        /// <returns>
        /// An <see cref="ICollection{T}"/> holding all the elements with keys 
        /// between <c>start</c> and <c>end</c>.
        /// </returns>
        public ICollection<KeyValuePair<K,V>> Sublist(K start, K end) {
            return dict.GetSublistEntries(start, end);
        }

        /// <summary>
        /// Returns a new map containing the elements with keys between
        /// the <c>start</c> and <c>end</c> keys, ordered with a new <see cref="Comparer{T}"/>.
        /// </summary>
        /// <param name="start">The lower bound of the map's elements.</param>
        /// <param name="end">The upper bound of the map's elements.</param>
        /// <exception cref="ArgumentException">
        /// If the <c>start</c> is smaller than the 
        /// <c>end</c> key according to the map's <see cref="Comparer{T}"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// If any of the keys are null.
        /// </exception>
        /// <returns>
        /// A new <see cref="SkipListDictionary{K, V}"/> holding all the elements with keys 
        /// between <c>start</c> and <c>end</c>.
        /// </returns>
        public SkipListDictionary<K,V> Submap(K start, K end, Comparer<K> comparer) {
            SkipListDictionary<K, V> map = new SkipListDictionary<K, V>(comparer);

            foreach (var entry in Sublist(start, end))
                map.Add(entry.Key, entry.Value);

            return map;
        }

        /// <summary>
        /// Returns a new map containing the elements with keys between
        /// the <c>start</c> and <c>end</c> keys, ordered with this map's <see cref="Comparer{T}"/>.
        /// </summary>
        /// <param name="start">The lower bound of the map's elements.</param>
        /// <param name="end">The upper bound of the map's elements.</param>
        /// <exception cref="ArgumentException">
        /// If the <c>start</c> is smaller than the 
        /// <c>end</c> key according to the map's <see cref="Comparer{T}"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// If any of the keys are null.
        /// </exception>
        /// <returns>
        /// A new <see cref="SkipListDictionary{K, V}"/> holding all the elements with keys 
        /// between <c>start</c> and <c>end</c>.
        /// </returns>
        public SkipListDictionary<K, V> Submap(K start, K end) {
            return Submap(start, end, dict.Comparer);
        }

        // ============================= INTERFACE METHODS =============================

        public void Add(K key, V value) {
            dict.Insert(key, value);
        }

        public void Add(KeyValuePair<K, V> item) {
            dict.Insert(item.Key, item.Value);
        }

        public void Clear() {
            dict = new SkipList<K, V>(dict.Comparer);
        }

        public bool Contains(KeyValuePair<K, V> item) {
            foreach (KeyValuePair<K, V> entry in dict.GetAll(item.Key))
                if (item.Value.Equals(entry.Value))
                    return true;

            return false;
        }

        public bool ContainsKey(K key) {
            return dict.Get(key) != null;
        }

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex) {
            if (array == null)
                throw new ArgumentNullException("array", "The provided array can't be null");

            if (arrayIndex + Count > array.Length)
                throw new ArgumentOutOfRangeException("array" , "The array's size is not sufficient to hold the dictionary's elements");

            int currIndex = arrayIndex;
            foreach(var entry in dict.GetEntries()) {
                array[currIndex] = entry;
                currIndex++;
            }
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator() {
            return dict.GetEntries().GetEnumerator();
        }

        public bool Remove(K key) {
            try {
                dict.Remove(key);
            }
            catch (ArgumentException) {
                return false;
            }
            return true;
        }

        public bool Remove(KeyValuePair<K, V> item) {
            return dict.Remove(item.Key, item.Value);
        }

        public bool TryGetValue(K key, [MaybeNullWhen(false)] out V value) {
            value = dict.Get(key);

            if (value == null)
                return false;
            else
                return true;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return dict.GetEntries().GetEnumerator();
        }
    }
}
