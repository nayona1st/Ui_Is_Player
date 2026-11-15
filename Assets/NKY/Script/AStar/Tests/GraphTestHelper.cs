using System;
using System.Collections.Generic;
using System.Linq;
using NKY.Script.AStar.Core;
using NKY.Script.AStar.Graph;
using NKY.Script.AStar.Grid;
using NKY.Script.AStar.Nodes;
using NUnit.Framework;
using UnityEngine;

namespace NKY.Script.AStar.Tests
{
  internal static class GraphTestHelper
    {
        public static PathGrid CreateGrid(
            Func<Vector2Int, bool> isWalkable,
            Func<Vector2Int, int> getCost,
            params (Vector2Int origin, int width, int height)[] chunks)
        {
            PathGrid grid = new PathGrid(1f, Vector2.zero);
 
            foreach (var c in chunks)
            {
                bool added = grid.TryAddChunk(c.width, c.height, c.origin,
                    pos => new TileInfo(getCost != null ? getCost(pos) : 1, isWalkable(pos)));
                Assert.IsTrue(added);
            }
 
            return grid;
        }
 
        public static PathGrid CreateSingleChunkGrid(
            int width, int height,
            Func<Vector2Int, bool> isWalkable,
            Func<Vector2Int, int> getCost = null)
        {
            return CreateGrid(isWalkable, getCost, (Vector2Int.zero, width, height));
        }
 
        public static List<IPathNode> FindPath(
            GridGraphProvider provider, Vector2Int start, Vector2Int goal)
        {
            if (!provider.TryGetCellNode(start, out CellPathNode startNode) ||
                !provider.TryGetCellNode(goal, out CellPathNode goalNode))
                return new List<IPathNode>();
 
            return new GraphAStar(provider).FindPath(startNode, goalNode, 1);
        }
 
        public static Vector2Int GridPosOf(IPathNode node)
        {
            return ((CellPathNode)node).SourceNode.NodeGridPos;
        }
 
        public static void AssertValidPath(
            PathGrid grid, List<IPathNode> path, Vector2Int start, Vector2Int goal)
        {
            Assert.IsNotNull(path);
            Assert.IsTrue(path.Count > 0);
            Assert.AreEqual(start, GridPosOf(path[0]));
            Assert.AreEqual(goal, GridPosOf(path[path.Count - 1]));
 
            for (int i = 0; i < path.Count; i++)
            {
                Assert.IsInstanceOf<CellPathNode>(path[i]);
                Assert.IsTrue(((CellPathNode)path[i]).SourceNode.CanMove);
 
                if (i == 0) continue;
 
                Vector2Int prev = GridPosOf(path[i - 1]);
                Vector2Int cur = GridPosOf(path[i]);
                int dx = Mathf.Abs(cur.x - prev.x);
                int dy = Mathf.Abs(cur.y - prev.y);
 
                Assert.IsTrue(dx <= 1 && dy <= 1 && (dx + dy) > 0);
 
                if (dx == 1 && dy == 1)
                {
                    INode sideA = grid.GetNodeByGrid(new Vector2Int(cur.x, prev.y));
                    INode sideB = grid.GetNodeByGrid(new Vector2Int(prev.x, cur.y));
                    Assert.IsTrue(sideA != null && sideA.CanMove && sideB != null && sideB.CanMove);
                }
            }
        }
 
        public static float PathCost(GridGraphProvider provider, List<IPathNode> path)
        {
            float cost = 0f;
            for (int i = 1; i < path.Count; i++)
                cost += provider.GetCost(path[i - 1], path[i]);
            return cost;
        }
    }
 
    public class GridGraphProviderTests
    {
        [Test]
        public void TryGetCellNode_SameCell_ReturnsSameInstance()
        {
            PathGrid grid = GraphTestHelper.CreateSingleChunkGrid(3, 3, _ => true);
            GridGraphProvider provider = new GridGraphProvider(grid);
 
            Assert.IsTrue(provider.TryGetCellNode(new Vector2Int(1, 1), out CellPathNode a));
            Assert.IsTrue(provider.TryGetCellNode(new Vector2Int(1, 1), out CellPathNode b));
 
            Assert.AreSame(a, b);
        }
 
        [Test]
        public void TryGetCellNode_DifferentCells_HaveDifferentIds()
        {
            PathGrid grid = GraphTestHelper.CreateSingleChunkGrid(3, 3, _ => true);
            GridGraphProvider provider = new GridGraphProvider(grid);
 
            provider.TryGetCellNode(new Vector2Int(0, 0), out CellPathNode a);
            provider.TryGetCellNode(new Vector2Int(1, 0), out CellPathNode b);
 
            Assert.AreNotEqual(a.NodeId, b.NodeId);
        }
 
        [Test]
        public void TryGetCellNode_WallOrOutsideMap_ReturnsFalse()
        {
            PathGrid grid = GraphTestHelper.CreateSingleChunkGrid(3, 3, pos => pos != new Vector2Int(1, 1));
            GridGraphProvider provider = new GridGraphProvider(grid);
 
            Assert.IsFalse(provider.TryGetCellNode(new Vector2Int(1, 1), out _));
            Assert.IsFalse(provider.TryGetCellNode(new Vector2Int(9, 9), out _));
        }
 
        [Test]
        public void GetNeighbors_CenterCellOfOpenMap_ReturnsEightCachedNodes()
        {
            PathGrid grid = GraphTestHelper.CreateSingleChunkGrid(3, 3, _ => true);
            GridGraphProvider provider = new GridGraphProvider(grid);
            provider.TryGetCellNode(new Vector2Int(1, 1), out CellPathNode center);
 
            List<IPathNode> neighbors = provider.GetNeighbors(center, 1).ToList();
 
            Assert.AreEqual(8, neighbors.Count);
            provider.TryGetCellNode(new Vector2Int(0, 0), out CellPathNode corner);
            Assert.IsTrue(neighbors.Contains(corner));
        }
 
        [Test]
        public void GetNeighbors_NonCellNode_ReturnsEmpty()
        {
            PathGrid grid = GraphTestHelper.CreateSingleChunkGrid(3, 3, _ => true);
            GridGraphProvider provider = new GridGraphProvider(grid);
            IChunk chunk = grid.GetChunkByGrid(Vector2Int.zero);
            ForeignPathNode foreign = new ForeignPathNode(PathNodeIdIssuer.Issue(), Vector2.zero, chunk);
 
            Assert.AreEqual(0, provider.GetNeighbors(foreign, 1).Count());
        }
 
        [Test]
        public void GetCost_UsesWorldDistanceTimesDestinationMoveCost()
        {
            PathGrid grid = GraphTestHelper.CreateSingleChunkGrid(3, 3, _ => true,
                pos => pos == new Vector2Int(1, 0) ? 4 : 1);
            GridGraphProvider provider = new GridGraphProvider(grid);
            provider.TryGetCellNode(new Vector2Int(0, 0), out CellPathNode from);
            provider.TryGetCellNode(new Vector2Int(1, 0), out CellPathNode costly);
            provider.TryGetCellNode(new Vector2Int(1, 1), out CellPathNode diagonal);
 
            Assert.AreEqual(4f, provider.GetCost(from, costly), 0.0001f);
            Assert.AreEqual(Mathf.Sqrt(2f), provider.GetCost(from, diagonal), 0.0001f);
        }
    }
 
    public class GraphAStarTests
    {
        [Test]
        public void FindPath_EmptyMap_ReturnsDiagonalShortestPath()
        {
            PathGrid grid = GraphTestHelper.CreateSingleChunkGrid(5, 5, _ => true);
            GridGraphProvider provider = new GridGraphProvider(grid);
 
            Vector2Int start = new Vector2Int(0, 0);
            Vector2Int goal = new Vector2Int(4, 4);
            List<IPathNode> path = GraphTestHelper.FindPath(provider, start, goal);
 
            GraphTestHelper.AssertValidPath(grid, path, start, goal);
            Assert.AreEqual(5, path.Count);
        }
 
        [Test]
        public void FindPath_WallWithGap_DetoursThroughGap()
        {
            PathGrid grid = GraphTestHelper.CreateSingleChunkGrid(5, 5,
                pos => !(pos.x == 2 && pos.y <= 3));
            GridGraphProvider provider = new GridGraphProvider(grid);
 
            Vector2Int start = new Vector2Int(0, 0);
            Vector2Int goal = new Vector2Int(4, 0);
            List<IPathNode> path = GraphTestHelper.FindPath(provider, start, goal);
 
            GraphTestHelper.AssertValidPath(grid, path, start, goal);
            Assert.IsTrue(path.Any(n => GraphTestHelper.GridPosOf(n) == new Vector2Int(2, 4)));
        }
 
        [Test]
        public void FindPath_FullyBlocked_ReturnsEmpty()
        {
            PathGrid grid = GraphTestHelper.CreateSingleChunkGrid(5, 5, pos => pos.x != 2);
            GridGraphProvider provider = new GridGraphProvider(grid);
 
            List<IPathNode> path = GraphTestHelper.FindPath(provider, new Vector2Int(0, 0), new Vector2Int(4, 0));
 
            Assert.IsNotNull(path);
            Assert.AreEqual(0, path.Count);
        }
 
        [Test]
        public void FindPath_DiagonalBlockedByCorner_DoesNotCutCorner()
        {
            PathGrid grid = GraphTestHelper.CreateSingleChunkGrid(2, 2, pos => pos != new Vector2Int(1, 0));
            GridGraphProvider provider = new GridGraphProvider(grid);
 
            Vector2Int start = new Vector2Int(0, 0);
            Vector2Int goal = new Vector2Int(1, 1);
            List<IPathNode> path = GraphTestHelper.FindPath(provider, start, goal);
 
            GraphTestHelper.AssertValidPath(grid, path, start, goal);
            Assert.AreEqual(3, path.Count);
            Assert.AreEqual(new Vector2Int(0, 1), GraphTestHelper.GridPosOf(path[1]));
        }
 
        [Test]
        public void FindPath_HighCostCell_IsAvoided()
        {
            Vector2Int expensive = new Vector2Int(2, 1);
            PathGrid grid = GraphTestHelper.CreateSingleChunkGrid(5, 3, _ => true,
                pos => pos == expensive ? 10 : 1);
            GridGraphProvider provider = new GridGraphProvider(grid);
 
            Vector2Int start = new Vector2Int(0, 1);
            Vector2Int goal = new Vector2Int(4, 1);
            List<IPathNode> path = GraphTestHelper.FindPath(provider, start, goal);
 
            GraphTestHelper.AssertValidPath(grid, path, start, goal);
            Assert.IsFalse(path.Any(n => GraphTestHelper.GridPosOf(n) == expensive));
            Assert.Less(GraphTestHelper.PathCost(provider, path), 10f);
        }
 
        [Test]
        public void FindPath_AcrossChunkBoundary_ReturnsPathThroughBothChunks()
        {
            PathGrid grid = GraphTestHelper.CreateGrid(_ => true, null,
                (new Vector2Int(0, 0), 3, 5),
                (new Vector2Int(3, 0), 2, 5));
            GridGraphProvider provider = new GridGraphProvider(grid);
 
            Vector2Int start = new Vector2Int(0, 2);
            Vector2Int goal = new Vector2Int(4, 2);
            List<IPathNode> path = GraphTestHelper.FindPath(provider, start, goal);
 
            GraphTestHelper.AssertValidPath(grid, path, start, goal);
            Assert.AreEqual(5, path.Count);
            Assert.AreEqual(2, path.Select(n => n.OwnerChunk.ChunkId).Distinct().Count());
        }
 
        [Test]
        public void FindPath_StartOrGoalOnWallOrOutsideMap_ReturnsEmpty()
        {
            PathGrid grid = GraphTestHelper.CreateSingleChunkGrid(5, 5, pos => pos != new Vector2Int(0, 0));
            GridGraphProvider provider = new GridGraphProvider(grid);
 
            Assert.AreEqual(0, GraphTestHelper.FindPath(provider, new Vector2Int(0, 0), new Vector2Int(4, 4)).Count);
            Assert.AreEqual(0, GraphTestHelper.FindPath(provider, new Vector2Int(4, 4), new Vector2Int(0, 0)).Count);
            Assert.AreEqual(0, GraphTestHelper.FindPath(provider, new Vector2Int(-1, 0), new Vector2Int(4, 4)).Count);
        }
 
        [Test]
        public void FindPath_StartEqualsGoal_ReturnsEmpty()
        {
            PathGrid grid = GraphTestHelper.CreateSingleChunkGrid(5, 5, _ => true);
            GridGraphProvider provider = new GridGraphProvider(grid);
 
            Assert.AreEqual(0, GraphTestHelper.FindPath(provider, new Vector2Int(2, 2), new Vector2Int(2, 2)).Count);
        }
 
        [Test]
        public void FindPath_NullArguments_ReturnsEmpty()
        {
            PathGrid grid = GraphTestHelper.CreateSingleChunkGrid(3, 3, _ => true);
            GridGraphProvider provider = new GridGraphProvider(grid);
            GraphAStar astar = new GraphAStar(provider);
            provider.TryGetCellNode(Vector2Int.zero, out CellPathNode node);
 
            Assert.AreEqual(0, astar.FindPath(null, node, 1).Count);
            Assert.AreEqual(0, astar.FindPath(node, null, 1).Count);
        }
 
        [Test]
        public void Constructor_NullProvider_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new GraphAStar(null));
        }
 
        [Test]
        public void FindPath_RandomMaps_MatchesDijkstraCost()
        {
            const int size = 12;
            Vector2Int start = new Vector2Int(0, 0);
            Vector2Int goal = new Vector2Int(size - 1, size - 1);
 
            for (int seed = 0; seed < 40; seed++)
            {
                System.Random random = new System.Random(seed);
                bool[,] walkable = new bool[size, size];
                int[,] costs = new int[size, size];
 
                for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                {
                    walkable[x, y] = random.NextDouble() > 0.25;
                    costs[x, y] = random.Next(1, 4);
                }
 
                walkable[start.x, start.y] = true;
                walkable[goal.x, goal.y] = true;
 
                PathGrid grid = GraphTestHelper.CreateSingleChunkGrid(size, size,
                    pos => walkable[pos.x, pos.y],
                    pos => costs[pos.x, pos.y]);
 
                GridGraphProvider provider = new GridGraphProvider(grid);
                provider.TryGetCellNode(start, out CellPathNode startNode);
                provider.TryGetCellNode(goal, out CellPathNode goalNode);
 
                List<IPathNode> path = new GraphAStar(provider).FindPath(startNode, goalNode, 1);
                float expectedCost = DijkstraCost(provider, startNode, goalNode);
 
                if (float.IsPositiveInfinity(expectedCost))
                {
                    Assert.AreEqual(0, path.Count, $"seed {seed}: 도달 불가인데 경로가 나옴");
                    continue;
                }
 
                GraphTestHelper.AssertValidPath(grid, path, start, goal);
                Assert.AreEqual(expectedCost, GraphTestHelper.PathCost(provider, path), 0.001f,
                    $"seed {seed}: 최적 비용과 다름");
            }
        }
 
        private static float DijkstraCost(GridGraphProvider provider, IPathNode start, IPathNode goal)
        {
            Dictionary<IPathNode, float> distances = new Dictionary<IPathNode, float> { [start] = 0f };
            HashSet<IPathNode> finished = new HashSet<IPathNode>();
 
            while (true)
            {
                IPathNode current = null;
                float best = float.PositiveInfinity;
 
                foreach (KeyValuePair<IPathNode, float> pair in distances)
                {
                    if (finished.Contains(pair.Key) || pair.Value >= best)
                        continue;
 
                    best = pair.Value;
                    current = pair.Key;
                }
 
                if (current == null)
                    return float.PositiveInfinity;
                if (ReferenceEquals(current, goal))
                    return best;
 
                finished.Add(current);
 
                foreach (IPathNode neighbor in provider.GetNeighbors(current, 1))
                {
                    float candidate = best + provider.GetCost(current, neighbor);
                    if (!distances.TryGetValue(neighbor, out float known) || candidate < known)
                        distances[neighbor] = candidate;
                }
            }
        }
    }
 
    internal sealed class ForeignPathNode : IPathNode
    {
        public int NodeId { get; }
        public PathNodeType NodeType => PathNodeType.Cell;
        public Vector2 WorldPosition { get; }
        public IChunk OwnerChunk { get; }
 
        public ForeignPathNode(int nodeId, Vector2 worldPosition, IChunk ownerChunk)
        {
            NodeId = nodeId;
            WorldPosition = worldPosition;
            OwnerChunk = ownerChunk;
        }
    }
}