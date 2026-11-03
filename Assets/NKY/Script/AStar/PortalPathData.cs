using System.Collections.Generic;

namespace NKY.Script.AStar
{
    public class PortalPathData
    {
        public Portal StartPortal { get; }
        public Portal EndPortal { get; }

        public List<INode> Path { get; }
        public float Cost { get; }

        public PortalPathData(Portal startPortal, Portal endPortal, List<INode> path, float cost)
        {
            StartPortal = startPortal;
            EndPortal = endPortal;
            Path = path;
            Cost = cost;
        }
    }
}