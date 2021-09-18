using System.Threading;

namespace SkipLists {

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
                try {
                    nextLock.EnterWriteLock();
                    next = value;
                }
                finally {
                    nextLock.ExitWriteLock();
                }
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
                try {
                    belowLock.EnterWriteLock();
                    next = value;
                }
                finally {
                    belowLock.ExitWriteLock();
                }
            }
        }

        public ConcurrentNode() : base() {}

        public ConcurrentNode(K key, V value): base(key, value) {}
    }
}
