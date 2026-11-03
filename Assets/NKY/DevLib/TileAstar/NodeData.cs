using System;
using System.Collections.Generic;
using UnityEngine;

namespace DevLib.TileAstar
{
    [Serializable]
    public struct NodeData : IEquatable<NodeData>
    {
        public Vector3 worldPosition;
        public Vector3Int cellPosition;
        public List<LinkData> neighbors; // 이웃들

        public NodeData(Vector3 worldPosition, Vector3Int cellPosition)
        {
            this.worldPosition = worldPosition;
            this.cellPosition = cellPosition;
            neighbors = new List<LinkData>();
        }
        
        public void AddNeighbor(NodeData nextNode)
        {
            neighbors.Add(new LinkData()
                {
                    startPosition =  worldPosition, // 시작점은 내 worldPos
                    startCellPosition = cellPosition, // 시작 cellPos도 내 cellPos
                    endPosition = nextNode.worldPosition, // 끝점은 다음노드의 worldPos
                    endCellPosition = nextNode.cellPosition,
                    cost = Vector3Int.Distance(cellPosition, nextNode.cellPosition) // 거리
                });
        }
        
        public bool Equals(NodeData other) => cellPosition == other.cellPosition; // 얘도 cellPosition으로 비교

        public override bool Equals(object obj)
            => obj is NodeData node && Equals(node); // obj가 nodeData 이면서 해당 노드랑 같다면 true
        // obj 가 NodeData가 아니거나 해당 노드랑 다르다면 false

        public override int GetHashCode() => cellPosition.GetHashCode();
        
        public static bool operator == (NodeData lhs, NodeData rhs)
            => lhs.Equals(rhs);
        public static bool operator != (NodeData lhs, NodeData rhs)
            => !lhs.Equals(rhs);
    }
}