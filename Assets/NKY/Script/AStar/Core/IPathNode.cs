using NKY.Script.AStar.Grid;
using UnityEngine;

namespace NKY.Script.AStar.Core
{
    public interface IPathNode
    {
        int NodeId { get; }
        PathNodeType NodeType { get; }
        Vector2 WorldPosition { get; }
        IChunk OwnerChunk { get; }
    }
}