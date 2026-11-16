using System.Collections.Generic;
using NKY.Script.AStar.Core;

namespace NKY.Script.AStar.Graph
{
    public interface IPathGraphProvider
    {
        IEnumerable<IPathNode> GetNeighbors(IPathNode node, float radius);
        float GetCost(IPathNode from, IPathNode to);
    }
}