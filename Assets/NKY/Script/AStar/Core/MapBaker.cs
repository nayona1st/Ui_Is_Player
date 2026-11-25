using System.Collections.Generic;
using NKY.Script.AStar.Grid;
using NKY.Script.AStar.Obstacle;
using UnityEngine;

namespace NKY.Script.AStar.Core
{
   public struct CellClearanceData
    {
        public float EastEdge;      // (+1,  0)
        public float NorthEdge;     // ( 0, +1)
        public float NorthEastEdge; // (+1, +1)
        public float NorthWestEdge; // (-1, +1)
    }

    public static class MapBaker
    {
        private static readonly Vector2Int[] MainDirections = new Vector2Int[]
        {
            new Vector2Int(1, 0),   // 동
            new Vector2Int(0, 1),   // 북
            new Vector2Int(1, 1),   // 북동
            new Vector2Int(-1, 1)   // 북서
        };
        
        public static Dictionary<Vector2Int, CellClearanceData> Bake(
            IGridProvider grid, 
            IReadOnlyList<ObstacleBox> boxes, 
            float maxRadius)
        {
            var clearanceMap = new Dictionary<Vector2Int, CellClearanceData>();
            if (grid == null) return clearanceMap;

            // 1. 모든 셀의 Node Clearance 및 4방향 Edge Clearance 베이킹
            foreach (INode node in grid.GetAllNodes())
            {
                if (node == null) continue;

                Vector2Int pos = node.NodeGridPos;
                Vector2 worldPos = node.NodeWorldPos;

                CellClearanceData data = new CellClearanceData
                {
                    EastEdge = maxRadius,
                    NorthEdge = maxRadius,
                    NorthEastEdge = maxRadius,
                    NorthWestEdge = maxRadius
                };
                
                for (int i = 0; i < MainDirections.Length; i++)
                {
                    Vector2Int dir = MainDirections[i];
                    Vector2Int targetPos = pos + dir;
                    INode targetNode = grid.GetNodeByGrid(targetPos);

                    if (targetNode == null || !targetNode.CanMove)
                    {
                        SetEdgeValue(ref data, i, 0f);
                        continue;
                    }

                    float edgeDist = CalculateEdgeClearance(worldPos, targetNode.NodeWorldPos, boxes, maxRadius);
                    SetEdgeValue(ref data, i, edgeDist);
                }

                clearanceMap[pos] = data;
            }

            return clearanceMap;
        }

        private static float CalculateEdgeClearance(Vector2 start, Vector2 end, IReadOnlyList<ObstacleBox> boxes, float maxRadius)
        {
            float min = maxRadius;
            if (boxes == null || boxes.Count == 0) return min;

            for (int i = 0; i < boxes.Count; i++)
            {
                float dist = boxes[i].DistanceToSegment(start, end);
                if (dist < min)
                {
                    min = dist;
                    if (min <= 0f) return 0f;
                }
            }
            return min;
        }

        private static void SetEdgeValue(ref CellClearanceData data, int dirIndex, float value)
        {
            switch (dirIndex)
            {
                case 0: data.EastEdge = value; break;
                case 1: data.NorthEdge = value; break;
                case 2: data.NorthEastEdge = value; break;
                case 3: data.NorthWestEdge = value; break;
            }
        }
    }
}