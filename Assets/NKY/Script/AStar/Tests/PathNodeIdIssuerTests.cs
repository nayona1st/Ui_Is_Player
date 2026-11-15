using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using NKY.Script.AStar.Core;
using NKY.Script.AStar.Grid;
using NKY.Script.AStar.Nodes;
using NUnit.Framework;
using UnityEngine;

namespace NKY.Script.AStar.Tests
{
public class PathNodeIdIssuerTests
    {
        [Test]
        public void Issue_ManyCalls_AllIdsAreUnique()
        {
            HashSet<int> ids = new HashSet<int>();
 
            for (int i = 0; i < 1000; i++)
                Assert.IsTrue(ids.Add(PathNodeIdIssuer.Issue()), "중복된 ID가 발급됨");
        }
 
        [Test]
        public void Issue_ConsecutiveCalls_ReturnIncreasingValues()
        {
            int first = PathNodeIdIssuer.Issue();
            int second = PathNodeIdIssuer.Issue();
 
            Assert.Greater(second, first);
        }
 
        [Test]
        public void Issue_ParallelCalls_AllIdsAreUnique()
        {
            ConcurrentBag<int> ids = new ConcurrentBag<int>();
 
            Parallel.For(0, 2000, _ => ids.Add(PathNodeIdIssuer.Issue()));
 
            Assert.AreEqual(2000, new HashSet<int>(ids).Count);
        }
    }
 
    public class CellPathNodeTests
    {
        private static IChunk CreateChunk()
        {
            return new Chunk(0, 2, 2, Vector2Int.zero, new Vector2Int(2, 2));
        }
 
        [Test]
        public void Constructor_ExposesSourceNodeData()
        {
            INode source = new Node(1, true, new Vector2(1.5f, 2.5f), new Vector2Int(1, 2));
            IChunk chunk = CreateChunk();
 
            CellPathNode node = new CellPathNode(source, chunk, 7);
 
            Assert.AreEqual(7, node.NodeId);
            Assert.AreEqual(PathNodeType.Cell, node.NodeType);
            Assert.AreEqual(new Vector2(1.5f, 2.5f), node.WorldPosition);
            Assert.AreSame(chunk, node.OwnerChunk);
            Assert.AreSame(source, node.SourceNode);
        }
 
        [Test]
        public void Constructor_NullSourceNode_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new CellPathNode(null, CreateChunk(), 0));
        }
 
        [Test]
        public void Constructor_NullChunk_Throws()
        {
            INode source = new Node(1, true, Vector2.zero, Vector2Int.zero);
 
            Assert.Throws<ArgumentNullException>(() => new CellPathNode(source, null, 0));
        }
 
        [Test]
        public void IPathNode_CanBeUsedAsDictionaryKey_ByReference()
        {
            // 캐시된 같은 객체를 키로 쓰면 같은 항목으로 인식되어야 한다.
            INode source = new Node(1, true, Vector2.zero, Vector2Int.zero);
            IPathNode node = new CellPathNode(source, CreateChunk(), 0);
            Dictionary<IPathNode, int> dict = new Dictionary<IPathNode, int> { { node, 1 } };
 
            Assert.IsTrue(dict.ContainsKey(node));
        }
    }
}