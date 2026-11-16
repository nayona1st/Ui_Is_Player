using System;
using System.Collections.Generic;
using NKY.Script.AStar.Core;
using NKY.Script.AStar.Graph;

namespace NKY.Script.AStar.PathFind
{
    public class CellPathfinder : IPathFinder
    {
        private readonly GraphAStar _aStar;
 
        public IPathGraphProvider PathGraphProvider { get; private set; }
 
        public CellPathfinder(IPathGraphProvider pathGraphProvider)
        {
            PathGraphProvider = pathGraphProvider ?? throw new ArgumentNullException(nameof(pathGraphProvider));
            _aStar = new GraphAStar(PathGraphProvider);
        }
 
        public List<IPathNode> FindPath(IPathNode start, IPathNode goal, float radius)
        {
            return _aStar.FindPath(start, goal, radius);
        }
    }
}