using UnityEngine;
using UnityEngine.Rendering;

namespace NKY.Script.AStar
{
    public class PathNodeData
    {
        public float GCost { get; set; } = float.PositiveInfinity;
        public float HCost { get; set; } = 0f;
        public float FCost => GCost + HCost;
        public INode ParentNode { get; set; } = null;

        public PathNodeData(float gCost, float hCost, INode parentNode)
        {
            GCost = gCost;
            HCost = hCost;
            ParentNode = parentNode;
        }

        public void SetPathData(float gCost, INode parentNode)
        {
            GCost = gCost;
            ParentNode = parentNode;
        }

        public static float GetHCost(Vector2Int start, Vector2Int end)
        {
            float dx = Mathf.Abs(start.x - end.x);
            float dy = Mathf.Abs(start.y - end.y);
            return Mathf.Max(dx, dy) + (Mathf.Sqrt(2) - 1) * Mathf.Min(dx,dy);
        }
    }
}