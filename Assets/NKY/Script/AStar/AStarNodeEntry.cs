using System;

namespace NKY.Script.AStar
{
    public class AStarNodeEntry : IComparable<AStarNodeEntry>
    {
        public INode Node { get; }
        
        public float GCost { get; }
        public float FCost { get; }
        
        public AStarNodeEntry(INode node, float gCost, float fCost)
        {
            Node = node ?? throw new ArgumentNullException(nameof(node));
            GCost = gCost;
            FCost = fCost;
        }
        
        public int CompareTo(AStarNodeEntry other)
        {
            if (other == null) return -1;
            return FCost.CompareTo(other.FCost);
        }
    }
}