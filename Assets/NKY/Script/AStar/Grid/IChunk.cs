using UnityEngine;

namespace NKY.Script.AStar.Grid
{
    public interface IChunk
    {
        int ChunkId { get; }
        int SizeX { get; }
        int SizeY { get; }
        INode[,] Nodes { get; }
        Vector2Int OriginGridPos { get; }
        Vector2Int EndGridPos { get; }
        bool GetNode(Vector2Int localPos, out INode node);
        
    }
}