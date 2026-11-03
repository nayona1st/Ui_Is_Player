using UnityEngine;

namespace NKY.Script.AStar
{
    public interface IChunk
    {
        int ChunkId { get; }
        int SizeX { get; }
        int SizeY { get; }
        INode[,] Nodes { get; }
        Vector2Int OriginGridPos { get; }
        bool GetNode(Vector2Int localPos, out INode node);
        
    }
}