using System;
using System.Collections.Generic;
using NKY.Script.AStar.Core;
using NKY.Script.AStar.Grid;
using NKY.Script.AStar.Nodes;
using NUnit.Framework;
using UnityEngine;

namespace NKY.Script.AStar.Tests
{
    public class SearchNodeTests
    {
        private static IChunk CreateChunk()
        {
            return new Chunk(0, 4, 4, Vector2Int.zero, new Vector2Int(4, 4));
        }
 
        private static CellPathNode CreateCellNode(int id, Vector2Int gridPos, IChunk chunk)
        {
            return CreateCellNode(id, gridPos, new Vector2(gridPos.x + 0.5f, gridPos.y + 0.5f), chunk);
        }
 
        private static CellPathNode CreateCellNode(int id, Vector2Int gridPos, Vector2 worldPos, IChunk chunk)
        {
            INode source = new Node(1, true, worldPos, gridPos);
            return new CellPathNode(source, chunk, id);
        }
 
        [Test]
        public void SearchNodeData_SetPathData_UpdatesGCostAndParent()
        {
            IChunk chunk = CreateChunk();
            IPathNode parent = CreateCellNode(0, new Vector2Int(1, 1), chunk);
 
            SearchNodeData data = new SearchNodeData(float.PositiveInfinity, 2f, null);
            data.SetPathData(1.5f, parent);
 
            Assert.AreSame(parent, data.Parent);
            Assert.AreEqual(1.5f, data.GCost);
            Assert.AreEqual(3.5f, data.FCost);
        }
 
        [Test]
        public void SearchNodeData_GetHCost_IsEuclideanDistanceBetweenWorldPositions()
        {
            IChunk chunk = CreateChunk();
            IPathNode a = CreateCellNode(0, new Vector2Int(0, 0), new Vector2(0f, 0f), chunk);
            IPathNode b = CreateCellNode(1, new Vector2Int(3, 4), new Vector2(3f, 4f), chunk);
 
            Assert.AreEqual(5f, SearchNodeData.GetHCost(a, b), 0.0001f);
        }
 
        [Test]
        public void SearchNodeData_FCost_IsGPlusH()
        {
            SearchNodeData data = new SearchNodeData(2f, 3f, null);
 
            Assert.AreEqual(5f, data.FCost);
        }
 
        [Test]
        public void SearchNodeEntry_NullNode_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new SearchNodeEntry(null, 0f, 0f));
        }
 
        [Test]
        public void SearchNodeEntry_InPriorityQueue_PopsLowestFCostFirst()
        {
            IChunk chunk = CreateChunk();
            PriorityQueue<SearchNodeEntry> queue = new PriorityQueue<SearchNodeEntry>();
            queue.Push(new SearchNodeEntry(CreateCellNode(0, new Vector2Int(0, 0), chunk), 0f, 5f));
            queue.Push(new SearchNodeEntry(CreateCellNode(1, new Vector2Int(1, 1), chunk), 0f, 1f));
            queue.Push(new SearchNodeEntry(CreateCellNode(2, new Vector2Int(2, 2), chunk), 0f, 3f));
 
            Assert.AreEqual(1, queue.Pop().Node.NodeId);
            Assert.AreEqual(2, queue.Pop().Node.NodeId);
            Assert.AreEqual(0, queue.Pop().Node.NodeId);
        }
 
        [Test]
        public void SearchNodeEntry_CompareToNull_ReturnsPositive()
        {
            IChunk chunk = CreateChunk();
            SearchNodeEntry entry = new SearchNodeEntry(CreateCellNode(0, new Vector2Int(0, 0), chunk), 0f, 1f);
 
            Assert.Greater(entry.CompareTo(null), 0);
        }
 
        [Test]
        public void CellNodes_CanBeUsedAsDictionaryKeysWithParentLinks()
        {
            IChunk chunk = CreateChunk();
            IPathNode first = CreateCellNode(0, new Vector2Int(0, 0), chunk);
            IPathNode second = CreateCellNode(1, new Vector2Int(1, 1), chunk);
 
            Dictionary<IPathNode, SearchNodeData> map = new Dictionary<IPathNode, SearchNodeData>();
            map.Add(first, new SearchNodeData(0f, 0f, null));
            map.Add(second, new SearchNodeData(1f, 1f, first));
 
            Assert.AreSame(first, map[second].Parent);
            Assert.AreEqual(2, map.Count);
        }
    }
}