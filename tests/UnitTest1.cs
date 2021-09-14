#define PRINT
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SkipLists;

namespace Tests {
    [TestClass]
    public class UnitTest1 {
        private static int OBJECT_COUNT = 50;

        [TestMethod]
        public void TestInsert() {
            SkipList<int, string> ls = new SkipList<int, string>();
            Console.WriteLine("Before:");
            Console.WriteLine(ls.debugPrint());
            for (int i = 0; i <= OBJECT_COUNT; i++)
                ls.Insert(i, i.ToString());
            Console.WriteLine("After insertion:");
            Console.WriteLine(ls.debugPrint());
        }

        [TestMethod]
        public void TestFind() {
            SkipList<int, string> ls = new SkipList<int, string>();
            for (int i = 0; i <= OBJECT_COUNT; i++)
                ls.Insert(i, i.ToString());
            
            Assert.IsTrue(ls.Get(23).Equals("23"));
        }

        [TestMethod]
        public void TestRemoval() {
            SkipList<int, string> ls = new SkipList<int, string>();
            for (int i = 0; i <= OBJECT_COUNT; i++)
                ls.Insert(i, i.ToString());
#if PRINT
            Console.WriteLine("Before removal:");
            Console.WriteLine(ls.debugPrint());
#endif
            ls.Remove(2);
            Assert.IsNull(ls.Get(2));
#if PRINT
            Console.WriteLine("After removal 2:");
            Console.WriteLine(ls.debugPrint());
#endif
            ls.Remove(3);
            Assert.IsNull(ls.Get(3));
#if PRINT
            Console.WriteLine("After removal 3:");
            Console.WriteLine(ls.debugPrint());
#endif
            ls.Remove(40);
            Assert.IsNull(ls.Get(40));
#if PRINT
            Console.WriteLine("After removal 40:");
            Console.WriteLine(ls.debugPrint());
#endif
            ls.Remove(5);
            Assert.IsNull(ls.Get(5));
#if PRINT
            Console.WriteLine("After removal 5:");
            Console.WriteLine(ls.debugPrint());
#endif
            ls.Remove(34);
            Assert.IsNull(ls.Get(34));
#if PRINT
            Console.WriteLine("After removal 34:");
            Console.WriteLine(ls.debugPrint());
#endif
            bool caught = false;
            try {
                ls.Remove(34);
            }
            catch (ArgumentException) {
                caught = true;
            }
            Assert.IsTrue(caught);   
        }

        [TestMethod]
        public void TestKeyLimits() {
            SkipList<int, string> ls = new SkipList<int, string>();
            for (int i = 0; i <= OBJECT_COUNT; i++)
                ls.Insert(i*2, (i*2).ToString());
#if PRINT
            Console.WriteLine(ls.debugPrint());
#endif
            Assert.AreEqual(ls.HigherEntry(43).Key, 44);
            Assert.AreEqual(ls.LowerEntry(43).Key, 42);
            Assert.AreEqual(ls.CeilingEntry(44).Key, 44);
            Assert.AreEqual(ls.FloorEntry(44).Key, 44);
        }
    }
}
