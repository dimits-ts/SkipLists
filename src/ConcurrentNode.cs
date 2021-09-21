using System.Threading;

namespace SkipLists {

    /// <summary>
    /// A node implementayion restricting access to its pointers
    /// via synchronized locks.
    /// </summary>
    internal sealed class ConcurrentNode<K,V> : Node<K,V> {

        private readonly ReaderWriterLockSlim nextLock = new ReaderWriterLockSlim();
        private readonly ReaderWriterLockSlim belowLock = new ReaderWriterLockSlim();

        internal override Node<K, V> Next {
            get {
                try {
                    nextLock.EnterReadLock();
                    return next;
                }
                finally {
                    nextLock.ExitReadLock();
                }
            }

            set {
                nextLock.EnterWriteLock();
                next = value;
                nextLock.ExitWriteLock();    
            }
        }

        internal override Node<K, V> Below {
            get {
                try {
                    belowLock.EnterReadLock();
                    return next;
                }
                finally {
                    belowLock.ExitReadLock();
                }
            }

            set {
                belowLock.EnterWriteLock();
                next = value;
                belowLock.ExitWriteLock();
            }
        }

        public ConcurrentNode() : base() {}

        public ConcurrentNode(K key, V value): base(key, value) {}
    }
}
