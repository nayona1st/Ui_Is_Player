using System.Collections.Generic;
using UnityEngine;

namespace NKY.Script.AStar
{
    public class AStarPathfinder
    {
        public IGridProvider Grid { get; private set; }

        public AStarPathfinder(IGridProvider grid)
        {
            Grid = grid;
        }

        public List<INode> FindPath(Vector2Int start, Vector2Int goal)
        {
            List<INode> path = new List<INode>();
            if(Grid.GetNodeByGrid(start) == null || Grid.GetNodeByGrid(goal) == null)
                return path;
            return path;
        }
    }
}