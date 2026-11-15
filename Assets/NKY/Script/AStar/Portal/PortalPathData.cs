using System.Collections.Generic;
using NKY.Script.AStar.Grid;

namespace NKY.Script.AStar.Portal
{
    public class PortalPathData
    {
        public AStar.Portal.Portal StartPortal { get; }
        public AStar.Portal.Portal EndPortal { get; }

        public List<INode> Path { get; }
        public float Cost { get; }

        public PortalPathData(AStar.Portal.Portal startPortal, AStar.Portal.Portal endPortal, List<INode> path, float cost)
        {
            StartPortal = startPortal;
            EndPortal = endPortal;
            Path = path;
            Cost = cost;
        }
    }
}