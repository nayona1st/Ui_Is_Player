using UnityEngine;

namespace NKY.Script.AStar.Core
{
    public sealed class SearchNodeData
    {
        public float GCost { get; set; } = float.PositiveInfinity;
        public float HCost { get; set; }
        public float FCost => GCost + HCost;
        public IPathNode Parent { get; set; }
 
        public SearchNodeData(float gCost, float hCost, IPathNode parent)
        {
            GCost = gCost;
            HCost = hCost;
            Parent = parent;
        }
 
        public void SetPathData(float gCost, IPathNode parent)
        {
            GCost = gCost;
            Parent = parent;
        }
 
        public static float GetHCost(IPathNode from, IPathNode to)
        {
            return Vector2.Distance(from.WorldPosition, to.WorldPosition);
        }
    }
}