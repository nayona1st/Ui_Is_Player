using UnityEngine;

namespace NKY.Script.AStar
{
    public class PathNodeData
    {
        public float GCost { get; set; } = float.PositiveInfinity;
        public float HCost { get; set; } = 0f;
        public float FCost => GCost + HCost;
        public INode ParentNode { get; set; } = null;
    }
}