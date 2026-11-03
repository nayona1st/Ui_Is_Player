using System.Collections.Generic;
using UnityEngine;

namespace NKY.Script.AStar
{
    public interface IGridProvider
    {
        float CellSize { get; }
        public Dictionary<Vector2Int, IChunk> Chunks { get; }
        IChunk GetChunkByGrid(Vector2Int position);
        INode GetNodeByGrid(Vector2Int position);
        List<INode> GetNeighbors(Vector2Int gridPos);
    }
}