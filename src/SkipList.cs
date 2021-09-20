using System;
using System.Collections.Generic;
#if DEBUG
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")] //test access
#endif

namespace SkipLists {

    /// <summary>
    /// A class implementing the operations of a sorted dictionary. It should be made available
    /// to users only in the form of wrappers in order to clearly separate interface from implementation.
    /// </summary>
    /// <typeparam name="K">The type of keys used</typeparam>
    /// <typeparam name="V">The type of values used.</typeparam>
    internal class SkipList<K,V> {
        private static readonly Random generator = new Random(DateTime.Now.GetHashCode());

        /// <summary>
        /// Defines a comparison condition between 2 keys, for example 
        /// whether or not we want key1 to be bigger or smaller than key2.
        /// </summary>
        /// <returns>Whether or not the comparison gives the intended result.</returns>
        private delegate bool CompCond(K key1, K key2);

        private readonly Comparer<K> keyComparer;
        private Node<K, V> head;
        private int height;
        private int size;

        private static bool CoinFlip() {
            return generator.Next(0, 2) == 0;
        }

        public int Size {
            get;
        }

        internal Comparer<K> Comparer {
            get;
        }


        public SkipList() : this(Comparer<K>.Default){}

        public SkipList(Comparer<K> customComparer) {
            size = 0;
            height = 1;
            head = BuildNode(default(K), default(V));
            keyComparer = customComparer;
        }

        /// <summary>
        /// Inserts a key-value pair in the skip list. Returns False if an entry
        /// with the same key already existed and was replaced.
        /// </summary>
        public bool Insert(K key, V value) {
            ThrowIfNull(key);

            //replace value if it already exists
            Node<K, V> oldNode = GetExactPosition(key); 
            if (oldNode != null) {
                oldNode.value = value;
                return false;
            }             

            size++;
            int height = 1;
            while (CoinFlip())
                height++;

            //create more levels if needed
            if(height >= this.height) {
                for(int i=0; i <= height - this.height; i++) { //build as many sentinels as to match height
                    Node<K,V> newHead = BuildNode();
                    newHead.Below = head;
                    head = newHead;
                }
                this.height = height;
            }
              
            //find current level
            Node<K, V> curr = head;
            for(int i=0; i <= this.height - height - 1; i++) 
                curr = curr.Below;

            //tower creation
            Node<K, V> currRow = curr;             //keep reference to first node of current tower
            Node<K, V> lastCreatedNode = null;
            do {                       
                while (curr.Next != null && !isSmaller(key, curr.Next.key)) //scan till you find the right position
                    curr = curr.Next;

                Node<K, V> nextNode;                 //old Next node, to be bypassed
                if (curr.Next == null)
                    nextNode = null;
                else
                    nextNode = curr.Next.Next;

                Node<K, V> newNode = BuildNode(key, value);

                if (lastCreatedNode != null)            //if not the first node that's created
                    lastCreatedNode.Below = newNode;    //link it to tower

                //update created node's references
                lastCreatedNode = newNode;
                curr.Next = newNode;
                newNode.Next = nextNode;
                
             
                //reset
                currRow = currRow.Below;      //drop down one level
                curr = currRow;                //reset curr to start of list
            } while (currRow != null);

            return true;
        }

        /// <summary>
        /// Removes the entry with the specified key, returning its value,
        /// or null if an entry with that key didn't exist.
        /// </summary>
        ///<exception cref="InvalidOperationException">If the list is empty.</exception>
        ///<exception cref="ArgumentNullException">If the key is null.</exception>
        public Pointer<V> Remove(K key) {
            ThrowIfNull(key);
            ThrowIfEmpty();

            Node<K, V> prev = head;
            Node<K, V> curr;
            V value;

            while(prev != null) { //while not on the bottom list
                curr = prev.Next;
               
                while (curr != null && isBigger(key, curr.key)) { //scan current list
                    prev = prev.Next;
                    curr = curr.Next;
                }

                if(curr != null && isEqual(key, curr.key)) { //start deleting all nodes from the tower
                    value = curr.value;

                    while(prev != null) {
                        curr = prev.Next;
                        prev.Next = curr.Next;  //remove node
                        prev = prev.Below;      //and drop down
                        while (prev != null && prev.Next != null && !isEqual(prev.Next.key, key)) //find Next node to remove in current list
                            prev = prev.Next;
                    }

                    size--;
                    return new Pointer<V>(value);
                }

                prev = prev.Below;  //drop down if you couldn't find the key
            }
            //if no removal
            return new Pointer<V>();
        }

        /// <summary>
        /// Returns the value of the entry with the specified key,
        /// null if no such entry is found.
        /// </summary>
        ///<exception cref="ArgumentNullException">If the key is null.</exception>
        public Pointer<V> Get(K key) {
            ThrowIfNull(key);

            Node<K, V> node = GetExactPosition(key);
            if (node == null)
                return new Pointer<V>();
            else
                return new Pointer<V>(node.value);
        }

        /// <summary>
        /// Returtns a list with all the keys contained in the list.
        /// </summary>
        public List<K> GetKeys() {
            List<K> list = new List<K>(size);

            //go to the bottom list
            Node<K, V> curr = head;
            while (curr.Below != null)
                curr = curr.Below;

            //fill list with the contents of the bottom list
            while (curr != null) {
                list.Add(curr.key);
                curr = curr.Next;
            }

            return list;
        }

        /// <summary>
        /// Returtns a list with all the cvalues contained in the list.
        /// </summary>
        public List<V> GetValues() {
            List<V> list = new List<V>(size);

            //go to the bottom list
            Node<K, V> curr = head;
            while (curr.Below != null)
                curr = curr.Below;

            //fill list with the contents of the bottom list
            while(curr != null) {
                list.Add(curr.value);
                curr = curr.Next;
            }

            return list;
        }

        /// <summary>
        /// Returtns a list with all the <see cref="KeyValuePair{TKey, TValue}"/>s contained in the list.
        /// </summary>
        public List<KeyValuePair<K,V>> GetEntries() {
            List<KeyValuePair<K, V>> list = new List<KeyValuePair<K, V>>(size);

            //go to the bottom list
            Node<K, V> curr = head;
            while (curr.Below != null)
                curr = curr.Below;

            //fill list with the contents of the bottom list
            while (curr != null) {
                list.Add(new KeyValuePair<K,V>(curr.key, curr.value));
                curr = curr.Next;
            }

            return list;
        }

        /// <summary>
        /// Returns an ordered collection containing the elements with keys between
        /// the <c>start</c> and <c>end</c> keys.
        /// </summary>
        ///<exception cref="InvalidOperationException">If the list is empty.</exception>
        ///<exception cref="ArgumentNullException">If any of the keys is null.</exception>
        public List<KeyValuePair<K,V>> GetSublistEntries(K start, K end) {
            ThrowIfNull(start);
            ThrowIfNull(end);

            List<KeyValuePair<K, V>> ls = new List<KeyValuePair<K, V>>();

            if (isBigger(start, end))
                //this likely indicates a failure in the client's logic so it's not handled in the method itself
                throw new ArgumentException("The end key precedes the start key"); 

            Node<K, V> currNode = GetPosition(start, isSmaller);
            Node<K, V> endNode = GetPosition(end, isSmaller);

            while (currNode.Next != endNode) {
                ls.Add(new KeyValuePair<K, V>(currNode.key, currNode.value));
                currNode = currNode.Next;
            }          

            return ls;
        }

        /// <summary>
        /// Get the entry with the smallest key.
        /// </summary>
        ///<exception cref = "InvalidOperationException" > If the list is empty.</exception>
        public KeyValuePair<K, V> FirstEntry() {
            ThrowIfEmpty();

            //find the 2nd node of the bottom list
            Node<K, V> curr = head;
            while (curr.Below != null)
                curr = curr.Below;

            return new KeyValuePair<K, V>(curr.Next.key, curr.Next.value);
        }

        /// <summary>
        /// Get the entry with the biggest key.
        /// </summary>
        ///<exception cref = "InvalidOperationException" > If the list is empty.</exception>
        public KeyValuePair<K,V> LastEntry() {
            ThrowIfEmpty();

            //skip to last node of the bottom list
            Node<K, V> curr = head;
            while(curr.Below != null) {

                while(curr.Next != null)    //go to end of current list
                    curr = curr.Next;
                
                curr = curr.Below;          //drop down
            }
            return new KeyValuePair<K, V>(curr.key, curr.value);
        }

        /// <summary>
        /// Returns the entry with a key larger or equal to the provided key.
        /// </summary>
        /// <returns>An <see cref="KeyValuePair{K, V}"/> with a key larger or equal to the provided key.</returns>
        /// <exception cref="InvalidOperationException">If the list is empty.</exception>
        /// <exception cref="ArgumentNullException">If the key is null.</exception>
        public KeyValuePair<K,V> CeilingEntry(K key) {
            ThrowIfNull(key);
            ThrowIfEmpty();

            Node<K,V> node = GetPosition(key, isSmallerOrEqual);

            while (isSmaller(node.key, key))
                node = node.Next;

            return new KeyValuePair<K, V>(node.key, node.value);
        }

        /// <summary>
        /// Returns the entry with a key smaller or equal to the provided key.
        /// </summary>
        /// <returns>An <see cref="KeyValuePair{K, V}"/> with a key smaller or equal to the provided key.</returns>
        /// <exception cref="InvalidOperationException">If the list is empty.</exception>
        /// <exception cref="ArgumentNullException">If the key is null.</exception>
        public KeyValuePair<K, V> FloorEntry(K key) {
            ThrowIfNull(key);
            ThrowIfEmpty();

            Node<K, V> node = GetPosition(key, isSmallerOrEqual);
            return new KeyValuePair<K, V>(node.key, node.value);
        }

        /// <summary>
        /// Returns the entry with a key larger than the provided key.
        /// </summary>
        /// <returns>An <see cref="KeyValuePair{K, V}"/> with a key larger or equal to the provided key.</returns>
        /// <exception cref="InvalidOperationException">If the list is empty.</exception>
        /// <exception cref="ArgumentNullException">If the key is null.</exception>
        public KeyValuePair<K, V> LowerEntry(K key) {
            ThrowIfNull(key);
            ThrowIfEmpty();

            Node<K, V> node = GetPosition(key, isSmallerOrEqual);
            return new KeyValuePair<K, V>(node.key, node.value);
        }

        /// <summary>
        /// Returns the entry with a key smaller than the provided key.
        /// </summary>
        /// <returns>An <see cref="KeyValuePair{K, V}"/> with a key smaller than the provided key.</returns>
        /// <exception cref="InvalidOperationException">If the list is empty.</exception>
        /// <exception cref="ArgumentNullException">If the key is null.</exception>
        public KeyValuePair<K, V> HigherEntry(K key) {
            ThrowIfNull(key);
            ThrowIfEmpty();

            Node<K,V> node = GetPosition(key, isSmaller);

            while (isSmallerOrEqual(node.key, key))
                node = node.Next;

            return new KeyValuePair<K, V>(node.key, node.value);
        }

        internal String debugPrint() {
            String str = "";
            Node<K, V> currRow = head;
            Node<K, V> curr;
            do {
                curr = currRow;

                while (curr != null) {
                    str += curr.key + " ";
                    curr = curr.Next;
                }
                    
                str += "\n";
                currRow = currRow.Below;
            } while (currRow != null);

            return str;
        }

        //builder methods

        private protected virtual Node<K, V> BuildNode(K key, V value) {
            return new Node<K, V>(key, value);
        }
        private protected virtual Node<K, V> BuildNode() {
            return new Node<K, V>();
        }


        /// <summary>
        /// Returns the right-most node whose key fullfils the provided ComparisonCondition
        /// on the bottom list.
        /// </summary>
        /// <param name="key">The key with which each node will be compared.</param>
        /// <param name="condition">An aggregate to be used to compare the key and the node.</param>
        /// <returns>The position of the node that best fullfils the comparison.</returns>
        private Node<K,V> GetPosition(K key, CompCond condition) {
            Node<K, V> curr = head;

            while(curr.Below != null) {
                curr = curr.Below;

                while (curr.Next != null && condition(curr.Next.key, key)) 
                    curr = curr.Next;
            }
            return curr;
        }

        private Node<K,V> GetExactPosition(K key) {
            Node<K, V> curr = head;

            do { //while not in the bottom
                while (curr.Next != null && !isSmaller(key, curr.Next.key)) { //scan forward
                    curr = curr.Next;
                    if (isEqual(key, curr.key))
                        return curr;
                }
                curr = curr.Below; //drop down
            } while (curr != null && curr.Below != null);

            return null;
        }

        private void ThrowIfNull(K key) {
            if (key == null)
                throw new ArgumentNullException("key", "The null value can't be used as a key");
        }

        private void ThrowIfEmpty() {
            if (size == 0)
                throw new InvalidOperationException("There are no entries in the collection");
        }

        //======================== DELEGATE METHODS ========================

        private bool isBigger(K key1, K key2) {
            return keyComparer.Compare(key1, key2) > 0;
        }

        private bool isBiggerOrEqual(K key1, K key2) {
            return keyComparer.Compare(key1, key2) >= 0;
        }

        private bool isSmallerOrEqual(K key1, K key2) {
            return keyComparer.Compare(key1, key2) <= 0;
        }

        private bool isSmaller(K key1, K key2) {
            return keyComparer.Compare(key1, key2) < 0;
        }

        private bool isEqual(K key1, K key2) {
            return keyComparer.Compare(key1, key2) == 0;
        }
    }
}
