using System;
using System.Collections.Generic;
using System.Linq;
using NKY.Script.AStar.Core;
using NUnit.Framework;

namespace NKY.Script.AStar.Tests
{
    public class PriorityQueueTests
    {
        private static List<int> PopAll(PriorityQueue<int> queue)
        {
            List<int> result = new List<int>();
            while (queue.Count > 0)
                result.Add(queue.Pop());
            return result;
        }

        [Test]
        public void Pop_EmptyQueue_ReturnsDefault()
        {
            PriorityQueue<int> queue = new PriorityQueue<int>();

            Assert.AreEqual(0, queue.Count);
            Assert.AreEqual(0, queue.Pop());
            Assert.AreEqual(0, queue.Count);
        }

        [Test]
        public void Pop_SingleElement_ReturnsIt()
        {
            PriorityQueue<int> queue = new PriorityQueue<int>();
            queue.Push(42);

            Assert.AreEqual(42, queue.Pop());
            Assert.AreEqual(0, queue.Count);
        }

        [Test]
        public void Pop_FixedValues_ReturnsAscending()
        {
            PriorityQueue<int> queue = new PriorityQueue<int>();
            foreach (int v in new[] { 5, 3, 8, 1, 9, 2, 7 })
                queue.Push(v);

            CollectionAssert.AreEqual(new[] { 1, 2, 3, 5, 7, 8, 9 }, PopAll(queue));
        }

        [Test]
        public void Pop_Duplicates_ReturnsAscendingWithAllItems()
        {
            PriorityQueue<int> queue = new PriorityQueue<int>();
            foreach (int v in new[] { 4, 2, 4, 2, 1, 4, 1 })
                queue.Push(v);

            CollectionAssert.AreEqual(new[] { 1, 1, 2, 2, 4, 4, 4 }, PopAll(queue));
        }

        [Test]
        public void Pop_AlreadySortedAndReverseSorted_ReturnsAscending()
        {
            int[] ascending = Enumerable.Range(0, 20).ToArray();

            PriorityQueue<int> q1 = new PriorityQueue<int>();
            foreach (int v in ascending) q1.Push(v);
            CollectionAssert.AreEqual(ascending, PopAll(q1));

            PriorityQueue<int> q2 = new PriorityQueue<int>();
            foreach (int v in ascending.Reverse()) q2.Push(v);
            CollectionAssert.AreEqual(ascending, PopAll(q2));
        }

        [Test]
        public void Pop_RandomValues_MatchesSortedOrder()
        {
            Random random = new Random(1234);
            List<int> values = new List<int>();
            for (int i = 0; i < 500; i++)
                values.Add(random.Next(-100, 100));

            PriorityQueue<int> queue = new PriorityQueue<int>();
            foreach (int v in values)
                queue.Push(v);

            values.Sort();
            CollectionAssert.AreEqual(values, PopAll(queue));
        }

        [Test]
        public void PushAndPop_Interleaved_AlwaysReturnsCurrentMinimum()
        {
            PriorityQueue<int> queue = new PriorityQueue<int>();
            queue.Push(5);
            queue.Push(1);
            queue.Push(3);

            Assert.AreEqual(1, queue.Pop());

            queue.Push(0);
            queue.Push(4);

            Assert.AreEqual(0, queue.Pop());
            Assert.AreEqual(3, queue.Pop());
            Assert.AreEqual(4, queue.Pop());
            Assert.AreEqual(5, queue.Pop());
            Assert.AreEqual(0, queue.Count);
        }

        [Test]
        public void Peek_ReturnsMinimumWithoutRemoving()
        {
            PriorityQueue<int> queue = new PriorityQueue<int>();
            queue.Push(7);
            queue.Push(2);
            queue.Push(9);

            Assert.AreEqual(2, queue.Peek());
            Assert.AreEqual(3, queue.Count);
        }
    }
}