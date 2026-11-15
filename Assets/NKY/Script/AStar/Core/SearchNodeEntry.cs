using System;

namespace NKY.Script.AStar.Core
{
    public sealed class SearchNodeEntry : IComparable<SearchNodeEntry>
    {
        public IPathNode Node { get; }
        public float GCost { get; }
        public float FCost { get; }
 
        public SearchNodeEntry(IPathNode node, float gCost, float fCost)
        {
            Node = node ?? throw new ArgumentNullException(nameof(node));
            GCost = gCost;
            FCost = fCost;
        }
 
        public int CompareTo(SearchNodeEntry other)
        {
            if (other == null) return 1;
            return FCost.CompareTo(other.FCost);
        }
    }
}