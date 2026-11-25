using System.Collections.Generic;
using NKY.Script.AStar.Core;
using UnityEngine;

namespace NKY.Script.AStar.PathFind
{
    public static class ClearanceLookup
    {
        public static float GetEdgeClearance(
            IReadOnlyDictionary<Vector2Int, CellClearanceData> clearanceMap, 
            Vector2Int from, 
            Vector2Int to)
        {
            Vector2Int dir = to - from;

            // 1. 내가 저장한 4방향 (동, 북, 북동, 북서)
            if (dir == new Vector2Int(1, 0))
                return clearanceMap.TryGetValue(from, out var d) ? d.EastEdge : 0f;
            if (dir == new Vector2Int(0, 1))
                return clearanceMap.TryGetValue(from, out var d) ? d.NorthEdge : 0f;
            if (dir == new Vector2Int(1, 1))
                return clearanceMap.TryGetValue(from, out var d) ? d.NorthEastEdge : 0f;
            if (dir == new Vector2Int(-1, 1))
                return clearanceMap.TryGetValue(from, out var d) ? d.NorthWestEdge : 0f;

            // 2. 상대방이 저장한 대칭 4방향 (서, 남, 남서, 남동)
            if (dir == new Vector2Int(-1, 0))
                return clearanceMap.TryGetValue(to, out var d) ? d.EastEdge : 0f;
            if (dir == new Vector2Int(0, -1))
                return clearanceMap.TryGetValue(to, out var d) ? d.NorthEdge : 0f;
            if (dir == new Vector2Int(-1, -1))
                return clearanceMap.TryGetValue(to, out var d) ? d.NorthEastEdge : 0f;
            if (dir == new Vector2Int(1, -1))
                return clearanceMap.TryGetValue(to, out var d) ? d.NorthWestEdge : 0f;

            return 0f;
        }

        public static bool CanTraverse(
            IReadOnlyDictionary<Vector2Int, CellClearanceData> clearanceMap,
            Vector2Int from,
            Vector2Int to,
            float playerRadius)
        {
            return GetEdgeClearance(clearanceMap, from, to) >= playerRadius;
        }
    }
}