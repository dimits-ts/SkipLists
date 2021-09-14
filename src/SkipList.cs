#define DEBUG
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
    internal class SkipList<K,V> where V : class {
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
            get{
                return size;
            }
        }

        public SkipList() : this(Comparer<K>.Default){}

        public SkipList(Comparer<K> customComparer) {
            size = 0;
            height = 1;
            head = new Node<K, V>(default(K), default(V));
            keyComparer = customComparer;
        }

        public void Insert(K key, V value) {
            int height = 1;
            while (CoinFlip())
                height++;

            //create more levels if needed
            if(height >= this.height) {
                for(int i=0; i <= height - this.height; i++) { //build as many sentinels as to match height
                    Node<K,V> newHead = new Node<K, V>();
                    newHead.below = head;
                    head = newHead;
                }
                this.height = height;
            }
              
            //find current level
            Node<K, V> curr = head;
            for(int i=0; i <= this.height - height - 1; i++) 
                curr = curr.below;

            //tower creation
            Node<K, V> currRow = curr;             //keep reference to first node of current tower
            Node<K, V> lastCreatedNode = null;
            do {                       
                while (curr.next != null && !isSmaller(key, curr.next.key)) //scan till you find the right position
                    curr = curr.next;

                Node<K, V> nextNode;                 //old next node, to be bypassed
                if (curr.next == null)
                    nextNode = null;
                else
                    nextNode = curr.next.next;

                Node<K, V> newNode = new Node<K, V>(key, value);

                if (lastCreatedNode != null)            //if not the first node that's created
                    lastCreatedNode.below = newNode;    //link it to tower

                //update created node's references
                lastCreatedNode = newNode;
                curr.next = newNode;
                newNode.next = nextNode;
                
             
                //reset
                currRow = currRow.below;      //drop down one level
                curr = currRow;                //reset curr to start of list
            } while (currRow != null);
        }

        public V Remove(K key) {
            Node<K, V> prev = head;
            Node<K, V> curr;
            V value;

            while(prev != null) { //while not on the bottom list
                curr = prev.next;
               
                while (curr != null && isBigger(key, curr.key)) { //scan current list
                    prev = prev.next;
                    curr = curr.next;
                }

                if(curr != null && isEqual(key, curr.key)) { //start deleting all nodes from the tower
                    value = curr.value;

                    while(prev != null) {
                        curr = prev.next;
                        prev.next = curr.next;  //remove node
                        prev = prev.below;      //and drop down
                        while (prev != null && prev.next != null && !isEqual(prev.next.key, key)) //find next node to remove in current list
                            prev = prev.next;
                    }

                    return value;
                }

                prev = prev.below;  //drop down if you couldn't find the key
            }
            //if no removal
            throw new ArgumentException("There's no object with the provided key."); 
        }

        public V Get(K key) {
            Node<K, V> curr = head;

            do { //while not in the bottom
                while (curr.next != null && !isSmaller(key, curr.next.key)) { //scan forward
                    curr = curr.next;
                    if (isEqual(key, curr.key))
                        return curr.value;
                }
                curr = curr.below; //drop down
            } while (curr.below != null);

            return null;
        }

        public List<K> GetKeys() {
            List<K> list = new List<K>(size);

            //go to the bottom list
            Node<K, V> curr = head;
            while (curr.below != null)
                curr = curr.below;

            //fill list with the contents of the bottom list
            while (curr != null) {
                list.Add(curr.key);
                curr = curr.next;
            }

            return list;
        }

        public List<V> GetValues() {
            List<V> list = new List<V>(size);

            //go to the bottom list
            Node<K, V> curr = head;
            while (curr.below != null)
                curr = curr.below;

            //fill list with the contents of the bottom list
            while(curr != null) {
                list.Add(curr.value);
                curr = curr.next;
            }

            return list;
        }

        public List<Entry<K,V>> GetEntries() {
            List<Entry<K, V>> list = new List<Entry<K, V>>(size);

            //go to the bottom list
            Node<K, V> curr = head;
            while (curr.below != null)
                curr = curr.below;

            //fill list with the contents of the bottom list
            while (curr != null) {
                list.Add(new Entry<K,V>(curr));
                curr = curr.next;
            }

            return list;
        }

        public List<Entry<K,V>> GetSublistEntries(K start, K end) {
            List<Entry<K, V>> ls = new List<Entry<K, V>>();

            if (isSmallerOrEqual(start, end))
                //this likely indicates a failure in the client's logic so it's not handled in the method itself
                throw new ArgumentException("The end key precedes the start key"); 

            Node<K, V> currNode = GetPosition(start, isSmaller);
            Node<K, V> endNode = GetPosition(end, isSmaller);
            while (currNode.next != endNode)
                ls.Add(new Entry<K, V>(currNode));

            return ls;
        }

        public Entry<K, V> FirstEntry() {
            //find the 2nd node of the bottom list
            Node<K, V> curr = head;
            while (curr.below != null)
                curr = curr.below;

            return new Entry<K,V>(curr.next);
        }

        public Entry<K,V> LastEntry() {
            //skip to last node of the bottom list
            Node<K, V> curr = head;
            while(curr.below != null) {

                while(curr.next != null)    //go to end of current list
                    curr = curr.next;
                
                curr = curr.below;          //drop down
            }
            return new Entry<K, V>(curr);
        }

        /// <summary>
        /// Returns the entry with a key larger or equal to the provided key.
        /// </summary>
        /// <returns>An <see cref="Entry{K, V}"/> with a key larger or equal to the provided key.</returns>
        public Entry<K,V> CeilingEntry(K key) {
            Node<K,V> node = GetPosition(key, isSmallerOrEqual);

            while (isSmaller(node.key, key))
                node = node.next;

            return new Entry<K, V>(node);
        }

        /// <summary>
        /// Returns the entry with a key smaller or equal to the provided key.
        /// </summary>
        /// <returns>An <see cref="Entry{K, V}"/> with a key smaller or equal to the provided key.</returns>
        public Entry<K, V> FloorEntry(K key) {
            return new Entry<K, V>(GetPosition(key, isSmallerOrEqual));
        }

        /// <summary>
        /// Returns the entry with a key larger than the provided key.
        /// </summary>
        /// <returns>An <see cref="Entry{K, V}"/> with a key larger or equal to the provided key.</returns>
        public Entry<K, V> LowerEntry(K key) {
            return new Entry<K, V>(GetPosition(key, isSmaller));
        }

        /// <summary>
        /// Returns the entry with a key smaller than the provided key.
        /// </summary>
        /// <returns>An <see cref="Entry{K, V}"/> with a key smaller than the provided key.</returns>
        public Entry<K, V> HigherEntry(K key) {
            Node<K,V> node = GetPosition(key, isSmaller);

            while (isSmallerOrEqual(node.key, key))
                node = node.next;

            return new Entry<K, V>(node);
        }

        internal String debugPrint() {
            String str = "";
            Node<K, V> currRow = head;
            Node<K, V> curr;
            do {
                curr = currRow;

                while (curr != null) {
                    str += curr.key + " ";
                    curr = curr.next;
                }
                    
                str += "\n";
                currRow = currRow.below;
            } while (currRow != null);

            return str;
        }

        /// <summary>
        /// Returns the right-most node whose key fullfils the provided ComparisonCondition
        /// on the bottom list.
        /// </summary>
        /// <param name="key">The key with which each node will be compared.</param>
        /// <param name="condition">An aggregate to be used to compare the key and the node.</param>
        /// <returns>The position of the node that besyt fullfils the comparison.</returns>
        private Node<K,V> GetPosition(K key, CompCond condition) {
            Node<K, V> curr = head;

            while(curr.below != null) {
                curr = curr.below;

                while (curr.next != null && condition(curr.next.key, key)) 
                    curr = curr.next;
            }
            return curr;
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