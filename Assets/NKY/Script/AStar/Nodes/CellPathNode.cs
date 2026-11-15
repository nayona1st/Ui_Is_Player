using System;
using NKY.Script.AStar.Core;
using NKY.Script.AStar.Grid;
using UnityEngine;

namespace NKY.Script.AStar.Nodes
{
    public sealed class CellPathNode : IPathNode
    {
        public int NodeId { get; }
        public PathNodeType NodeType => PathNodeType.Cell;
        public Vector2 WorldPosition => SourceNode.NodeWorldPos;
        public IChunk OwnerChunk { get; }
 
        // Cell->Cell Edge에서 CanMove, MoveCost를 읽기 위해 원본을 노출
        public INode SourceNode { get; }
 
        public CellPathNode(INode sourceNode, IChunk ownerChunk, int nodeId)
        {
            SourceNode = sourceNode ?? throw new ArgumentNullException(nameof(sourceNode));
            OwnerChunk = ownerChunk ?? throw new ArgumentNullException(nameof(ownerChunk));
            NodeId = nodeId;
        }
    }
}