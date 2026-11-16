using System;
using System.Collections.Generic;
using NKY.Script.AStar.Core;
using NKY.Script.AStar.Obstacle;
using UnityEngine;

namespace NKY.Script.AStar.Graph
{
    public class GraphAStar
    {
        public readonly IPathGraphProvider Graph;
 
        public GraphAStar(IPathGraphProvider graph)
        {
            this.Graph = graph ?? throw new ArgumentNullException(nameof(graph));
        }
 
        public List<IPathNode> FindPath(IPathNode start, IPathNode goal, float radius)
        {
            List<IPathNode> path = new List<IPathNode>();
 
            if (start == null || goal == null)
                return path;
            if (ReferenceEquals(start, goal))
                return path;
 
            Dictionary<IPathNode, SearchNodeData> searchData = new();
            HashSet<IPathNode> closedList = new();
            PriorityQueue<SearchNodeEntry> openList = new();
 
            float startHCost = SearchNodeData.GetHCost(start, goal);
            searchData.Add(start, new SearchNodeData(0f, startHCost, null));
            openList.Push(new SearchNodeEntry(start, 0f, startHCost));
 
            while (openList.Count > 0)
            {
                SearchNodeEntry entry = openList.Pop();
                IPathNode current = entry.Node;
                SearchNodeData currentData = searchData[current];
 
                if (entry.GCost > currentData.GCost)
                    continue;
                if (!closedList.Add(current))
                    continue;
 
                if (ReferenceEquals(current, goal))
                {
                    IPathNode step = current;
                    while (step != null)
                    {
                        path.Add(step);
                        step = searchData[step].Parent;
                    }
 
                    path.Reverse();
                    break;
                }
 
                foreach (IPathNode neighbor in Graph.GetNeighbors(current, radius))
                {
                    if (closedList.Contains(neighbor))
                        continue;
 
                    float stepCost = Graph.GetCost(current, neighbor);
                    if (float.IsPositiveInfinity(stepCost))
                        continue;
 
                    float newGCost = currentData.GCost + stepCost;
 
                    if (!searchData.TryGetValue(neighbor, out SearchNodeData neighborData))
                    {
                        neighborData = new SearchNodeData(
                            float.PositiveInfinity,
                            SearchNodeData.GetHCost(neighbor, goal),
                            null);
                        searchData.Add(neighbor, neighborData);
                    }
 
                    if (newGCost < neighborData.GCost)
                    {
                        neighborData.SetPathData(newGCost, current);
                        openList.Push(new SearchNodeEntry(neighbor, newGCost, newGCost + neighborData.HCost));
                    }
                }
            }
 
            return path;
        }
        
        public static ObstacleBox FromCollider(BoxCollider2D c)
        {
            Transform t = c.transform;
            Vector2 center = t.TransformPoint(c.offset);
            Vector3 s = t.lossyScale;
            Vector2 half = new Vector2(
                Mathf.Abs(c.size.x * s.x),
                Mathf.Abs(c.size.y * s.y)) * 0.5f;
            return new ObstacleBox(center, half, t.eulerAngles.z);
        }
    }
}