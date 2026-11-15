using System.Collections.Generic;
using NKY.Script.AStar.Core;
using NKY.Script.AStar.Graph;
using NKY.Script.AStar.Grid;
using UnityEngine;

namespace NKY.Script.AStar.PathFind
{
    public interface IPathFinder
    {
        IPathGraphProvider PathGraphProvider { get; }
        List<IPathNode> FindPath(IPathNode start, IPathNode goal, float radius);
    }
}