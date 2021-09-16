using System;
using System.Collections;
using System.Collections.Generic;

namespace SkipLists {

    /// <summary>
    /// A sorted set implemented with a Skip List.
    /// </summary>
    /// <typeparam name="T">The values' type.</typeparam>
    public class SkipListSet<T> : ISet<T> where T: class {
        private protected SkipList<T,T> set;

        /// <summary>
        /// Creates and returns a read-only wrapper for the set. Users using this wrapper will have access to all the data within, but will 
        /// be unable to mutate them.<br></br>
        /// The set can still be modified by using the underlying set reference. To prevent this, create a copy of the set 
        /// to pass as an argument to the method.
        /// </summary>
        /// <param name="set">The set to be protected.</param>
        public static SkipListSet<T> AsReadOnly(SkipListSet<T> set) {
            return new ReadOnlySet<T>(set.set);
        }

        public int Count {
            get {
                return set.Size;
            }
        }

        public virtual bool IsReadOnly {
            get {
                return false;
            }
        }

        /// <summary>
        /// Constructs an empty set using the default <see cref="Comparer{T}"/>.
        /// </summary>
        public SkipListSet() {
            set = new SkipList<T, T>();
        }

        /// <summary>
        /// Constructs an empty set using a custom <see cref="Comparer{T}"/>.
        /// </summary>
        public SkipListSet(Comparer<T> customComparer) {
            set = new SkipList<T, T>(customComparer);
        }

        /// <summary>
        /// Constructs a setm containing the <c>values</c> with 
        /// the default <see cref="Comparer{T}"/>.
        /// </summary>
        /// <param name="values">The values inserted into the set.</param>
        public SkipListSet(IEnumerable<T> values) : this() {
            foreach (T value in values)
                Add(value);
        }

        /// <summary>
        /// Constructs a setm containing the <c>values</c> with 
        /// a custom <see cref="Comparer{T}"/>.
        /// </summary>
        /// <param name="values">The values inserted into the set.</param>
        public SkipListSet(IEnumerable<T> values, Comparer<T> customComparer) : this(customComparer) {
            foreach (T value in values)
                Add(value);
        }

        // ============================= EXTENSION METHODS =============================

        /// <summary>
        /// Returns the first element of the set according to the <see cref="Comparer{T}"/>.
        /// </summary>
        /// <returns>The smallest entry.</returns>
        public virtual T Min() {
            return set.FirstEntry().Key;
        }

        /// <summary>
        /// Returns the last element of the set according to the <see cref="Comparer{T}"/>.
        /// </summary>
        /// <returns> The largest entry.</returns>
        public virtual T Max() {
            return set.LastEntry().Key;
        }

        //are these names appropriate if the user chooses reverse ordering?

        /// <summary>
        /// Returns the smallest entry larger or equal to the provided key.
        /// </summary>
        /// <exception cref="ArgumentNullException">If the key is null.</exception>
        /// <returns>The smallest entry larger or equal to the provided key.</returns>
        public virtual T CeilingEntry(T key) {
            return set.CeilingEntry(key).Key;
        }

        /// <summary>
        /// Returns the largest entry smaller or equal to the provided key.
        /// </summary>
        /// <exception cref="ArgumentNullException">If the key is null.</exception>
        /// <returns>The largest entry smaller or equal to the provided key.</returns>
        public virtual T FloorEntry(T key) {
            return set.FloorEntry(key).Key;
        }

        /// <summary>
        /// Returns the largest entry smaller than the provided key.
        /// </summary>
        /// <exception cref="ArgumentNullException">If the key is null.</exception>
        /// <returns>The largest entry smaller than the provided key.</returns>
        public virtual T HigherEntry(T key) {
            return set.HigherEntry(key).Key;
        }

        /// <summary>
        /// Returns the largest entry strictly smaller than the provided key.
        /// </summary>
        /// <exception cref="ArgumentNullException">If the key is null.</exception>
        /// <returns>The largest entry strictly smaller than the provided key.</returns>
        public virtual T LowerEntry(T key) {
            return set.LowerEntry(key).Key;
        }

        /// <summary>
        /// Returns an ordered collection containing the elements between
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
        public virtual ICollection<T> Sublist(T start, T end) {
            List<KeyValuePair<T,T>> entries = set.GetSublistEntries(start, end);
            List<T> sublist = new List<T>(entries.Count);

            foreach (var entryPair in entries)
                sublist.Add(entryPair.Key);

            return sublist;
        }

        /// <summary>
        /// Returns a new set containing the elements between
        /// the <c>start</c> and <c>end</c> keys, ordered with a new <see cref="Comparer{T}"/>.
        /// </summary>
        /// <param name="start">The lower bound of the set's elements.</param>
        /// <param name="end">The upper bound of the set's elements.</param>
        /// <exception cref="ArgumentException">
        /// If the <c>start</c> is smaller than the 
        /// <c>end</c> key according to the map's <see cref="Comparer{T}"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// If any of the keys are null.
        /// </exception>
        /// <returns>
        /// A new <see cref="SkipListSet{T}"/> holding all the elements
        /// between <c>start</c> and <c>end</c>.
        /// </returns>
        public virtual SkipListSet<T> Subset(T start, T end, Comparer<T> comparer) {
            SkipListSet<T> set = new SkipListSet<T>(comparer);

            foreach (T value in Sublist(start, end))
                set.Add(value);

            return set;
        }

        /// <summary>
        /// Returns a new set containing the elements between the <c>start</c> and 
        /// <c>end</c> keys, ordered with this set's <see cref="Comparer{T}"/>.
        /// </summary>
        /// <param name="start">The lower bound of the set's elements.</param>
        /// <param name="end">The upper bound of the set's elements.</param>
        /// <exception cref="ArgumentException">
        /// If the <c>start</c> is smaller than the 
        /// <c>end</c> key according to this set's <see cref="Comparer{T}"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// If any of the keys are null.
        /// </exception>
        /// <returns>
        /// A new <see cref="SkipListSet{T}"/> holding all the elements
        /// between <c>start</c> and <c>end</c>.
        /// </returns>
        public virtual SkipListSet<T> Subset(T start, T end) {
            return Subset(start, end, set.Comparer);
        }

        // ============================= INTERFACE METHODS =============================

        public virtual bool Add(T item) {
            return set.Insert(item, item);
        }

        public virtual void Clear() {
            set = new SkipList<T, T>(set.Comparer);
        }

        public virtual bool Contains(T item) {
            return set.Get(item) != null;
        }

        public virtual void CopyTo(T[] array, int arrayIndex) {
            if (array == null)
                throw new ArgumentNullException("array", "The provided array can't be null");

            if (arrayIndex + Count > array.Length)
                throw new ArgumentOutOfRangeException("array", "The array's size is not sufficient to hold the set's elements");

            int currIndex = arrayIndex;
            foreach (var entry in set.GetEntries()) {
                array[currIndex] = entry.Key;
                currIndex++;
            }
        }

        public virtual void ExceptWith(IEnumerable<T> other) {
            ThrowIfNull(other);

            foreach (T element in other)
                set.Remove(element);
        }

        public virtual IEnumerator<T> GetEnumerator() {
            return set.GetKeys().GetEnumerator();
        }

        public virtual void IntersectWith(IEnumerable<T> other) {
            ThrowIfNull(other);

            SkipList<T, T> newSet = new SkipList<T, T>(set.Comparer);
            foreach (T value in other)
                if (Contains(value))
                    newSet.Insert(value, value);

            set = newSet;
        }

        public virtual bool IsProperSubsetOf(IEnumerable<T> other) {
            ThrowIfNull(other);

            HashSet<T> otherSet = new HashSet<T>(other);

            if (otherSet.Count == Count) 
                return false;

            foreach (T value in set.GetKeys())
                if (!otherSet.Contains(value))
                    return false;

            return true;
        }

        public virtual bool IsProperSupersetOf(IEnumerable<T> other) {
            ThrowIfNull(other);

            int elementCount = 0;

            foreach (T value in other) {
                elementCount++;
                if (!Contains(value))
                    return false;
            }

            if (elementCount == Count)
                return false;

            return true;
        }

        public virtual bool IsSubsetOf(IEnumerable<T> other) {
            ThrowIfNull(other);

            HashSet<T> otherSet = new HashSet<T>(other);

            foreach (T value in set.GetKeys())
                if (!otherSet.Contains(value))
                    return false;

            return true;
        }

        public virtual bool IsSupersetOf(IEnumerable<T> other) {
            ThrowIfNull(other);

            foreach (T value in other) 
                if (!Contains(value))
                    return false;

            return true;
        }

        public virtual bool Overlaps(IEnumerable<T> other) {
            ThrowIfNull(other);

            foreach (T value in other)
                if (Contains(value))
                    return true;

            return false;
        }

        public virtual bool Remove(T item) {
            return set.Remove(item) != null;
        }

        public virtual bool SetEquals(IEnumerable<T> other) {
            ThrowIfNull(other);

            //much faster than IsSubsetOf && IsSupersetOf
            int elementCount = 0;

            foreach (T value in other) {
                elementCount++;
                if (!Contains(value))
                    return false;
            }

            if (elementCount == Count)
                return true;

            return false;
        }

        public virtual void SymmetricExceptWith(IEnumerable<T> other) {
            ThrowIfNull(other);

            foreach (T value in other) {
                if (Contains(value))
                    Remove(value);
                else
                    Add(value);
            }
        }

        public virtual void UnionWith(IEnumerable<T> other) {
            ThrowIfNull(other);

            foreach (T value in other) 
                Add(value);
            //duplicate values are automatically handled by the skip list
        }

        void ICollection<T>.Add(T item) {
            Add(item);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return set.GetKeys().GetEnumerator();
        }

        private void ThrowIfNull(IEnumerable<T> collecection) {
            if (collecection == null)
                throw new ArgumentNullException("The provided collection can't be null");
        }
    }
}
