using System;
using System.Collections.Generic;
using UnityEngine;

namespace NKY.Script.AStar
{
    public class PathGrid : IGridProvider
    {
        public Vector2 Origin { get; private set; }
        public float CellSize { get; private set; }
        
        // 청크의 시작 전역 좌표를 키로 사용
        public Dictionary<Vector2Int, IChunk> Chunks { get;} = new();
        
        // 전역 셀 좌표 -> 노드
        private readonly Dictionary<Vector2Int, INode> _nodesByGrid = new();

        // 전역 셀 좌표 -> 소속 청크
        private readonly Dictionary<Vector2Int, IChunk> _chunkByGrid = new();
        
        private int _nextChunkId = 0;

        public PathGrid(float cellSize, Vector2 origin)
        {
            if (cellSize <= 0f)
                throw new ArgumentOutOfRangeException(nameof(cellSize));

            Origin = origin;
            CellSize = cellSize;
        }

        public bool TryAddChunk(int width, int height, Vector2Int origin)
        {
            if (width <= 0 || height <= 0)
                return false;
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector2Int gridPos = origin
                                         + new Vector2Int(x, y);

                    if (_nodesByGrid.ContainsKey(gridPos))
                        return false;
                }
            }
            
            Chunk chunk = new Chunk(_nextChunkId, width, height, origin);
            
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector2Int gridPos = origin
                                         + new Vector2Int(x, y);

                    Vector2 worldPos = GridToWorld(gridPos);

                    INode node = new Node(
                        1,
                        true,
                        worldPos,
                        gridPos);

                    chunk.Nodes[x, y] = node;

                    _nodesByGrid.Add(gridPos, node);
                    _chunkByGrid.Add(gridPos, chunk);
                }
            }
            
            Chunks.Add(origin, chunk);
            _nextChunkId++;

            return true;
        }

        public bool TryRemoveChunk(Vector2Int origin)
        {
            if (!Chunks.TryGetValue(origin, out IChunk chunk)) return false;

            foreach (INode node in chunk.Nodes)
            {
                Vector2Int gridPos = node.NodeGridPos;
                if (!_nodesByGrid.ContainsKey(gridPos) || !_chunkByGrid.ContainsKey(gridPos))
                {
                    return false;
                }

                if (_chunkByGrid[gridPos] != chunk )
                {
                    return false;
                }

                if (_nodesByGrid[gridPos] != node)
                {
                    return false;
                }
            }
            foreach (INode node in chunk.Nodes)
            {
                Vector2Int gridPos = node.NodeGridPos;
                _nodesByGrid.Remove(gridPos);
                _chunkByGrid.Remove(gridPos);
            }
            Chunks.Remove(origin);
            return true;
        }

        public Vector2 GridToWorld(Vector2Int gridPos)
        {
            return Origin + new Vector2(gridPos.x * CellSize + CellSize * 0.5f, gridPos.y * CellSize + CellSize * 0.5f);
        }
        
        public Vector2Int WorldToGrid(Vector2 worldPos)
        {
            Vector2 gridPos = new Vector2(worldPos.x - Origin.x, worldPos.y - Origin.y);
            return new Vector2Int(Mathf.FloorToInt(gridPos.x / CellSize), Mathf.FloorToInt(gridPos.y /  CellSize));
        }
        

        public INode GetNodeByGrid(Vector2Int worldCellPos)
        {
            return _nodesByGrid.GetValueOrDefault(worldCellPos);
        }

        public IChunk GetChunkByGrid(Vector2Int worldCellPos)
        {
            return _chunkByGrid.GetValueOrDefault(worldCellPos);
        }

        public List<INode> GetNeighbors(Vector2Int gridPos)
        {
            List<INode> neighbors = new List<INode>();
            if(!_nodesByGrid.ContainsKey(gridPos))return neighbors;
            for (int x = -1; x<= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;
                    bool isDiagonal = x != 0 && y != 0;
                    Vector2Int sideA = gridPos + new Vector2Int(x, 0);
                    Vector2Int sideB = gridPos + new Vector2Int(0, y);
                    if (isDiagonal)
                    {
                       INode nodeA = GetNodeByGrid(sideA);
                       INode nodeB = GetNodeByGrid(sideB);
                       if(nodeA == null || nodeB == null)
                           continue;
                       if(!nodeA.CanMove || !nodeB.CanMove)
                           continue;
                    }
                    Vector2Int neighborPos = new Vector2Int(gridPos.x + x, gridPos.y + y);
                    if (_nodesByGrid.TryGetValue(neighborPos, out INode node) && node.CanMove)
                    {
                        neighbors.Add(node);
                    }
                }
            }
            return neighbors;
        }
    }
}