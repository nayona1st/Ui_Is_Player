using System;
using System.Collections.Generic;
using NKY.Script.AStar.Core;
using NKY.Script.AStar.Grid;
using NKY.Script.AStar.Nodes;
using NKY.Script.AStar.PathFind;
using UnityEngine;

namespace NKY.Script.AStar.Graph
{
    public class GridGraphProvider : IPathGraphProvider
    {
        private readonly IGridProvider _grid;
        private readonly Dictionary<Vector2Int, CellPathNode> _cellNodes = new();
        private IReadOnlyDictionary<Vector2Int, CellClearanceData> _clearanceMap;
 
        public GridGraphProvider(IGridProvider grid)
        {
            _grid = grid ?? throw new ArgumentNullException(nameof(grid));
        }
        
        public void SetClearanceMap(IReadOnlyDictionary<Vector2Int, CellClearanceData> clearanceMap)
        {
            _clearanceMap = clearanceMap;
            ClearCache(); // 베이크 데이터 교체 시 캐시 초기화
        }
 
        public bool TryGetCellNode(Vector2Int gridPos, out CellPathNode node)
        {
// 1. 이미 생성된 캐시 노드가 있으면 재사용
            if (_cellNodes.TryGetValue(gridPos, out node))
                return true;

            // 2. 그리드에 해당 셀이 존재하고 이동 가능한지 확인
            INode rawNode = _grid.GetNodeByGrid(gridPos);
            if (rawNode is { CanMove: true })
            {
                node = new CellPathNode(rawNode, _grid.GetChunkByGrid(gridPos), PathNodeIdIssuer.Issue());
                _cellNodes[gridPos] = node; // 캐시 저장
                return true;
            }

            node = null;
            return false;
        }

        public IEnumerable<IPathNode> GetNeighbors(IPathNode node, float radius)
        {
            List<IPathNode> neighbors = new List<IPathNode>();
            if (node is not CellPathNode cell) return neighbors;

            Vector2Int fromPos = cell.SourceNode.NodeGridPos;

            foreach (INode neighbor in _grid.GetNeighbors(fromPos))
            {
                Vector2Int toPos = neighbor.NodeGridPos;

                // Clearance 검사 (베이크 데이터가 없으면 기본 CanMove 사용)
                if (_clearanceMap != null)
                {
                    if (!ClearanceLookup.CanTraverse(_clearanceMap, fromPos, toPos, radius))
                    {
                        continue;
                    }
                }

                if (TryGetCellNode(toPos, out CellPathNode neighborNode))
                    neighbors.Add(neighborNode);
            }

            return neighbors;
        }
 
        public float GetCost(IPathNode from, IPathNode to)
        {
            if (from == null || to is not CellPathNode toCell)
                return float.PositiveInfinity;
 
            return Vector2.Distance(from.WorldPosition, to.WorldPosition) * toCell.SourceNode.MoveCost;
        }
 
        public void ClearCache()
        {
            _cellNodes.Clear();
        }
    }
}