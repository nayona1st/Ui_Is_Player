using System;
using NaughtyAttributes;
using UnityEngine;

namespace NKY.Script.AStar
{
    [Serializable]
    public class Node : INode
    {
        [ShowNativeProperty] public int MoveCost { get; set; }
        [ShowNativeProperty] public bool CanMove { get; set; }
        public Vector2 NodeWorldPos { get; }
        public Vector2Int NodeGridPos { get; }

        public void SetNode(int cost, bool canMove)
        {
            MoveCost = cost;
            CanMove = canMove;
        }

        public Node(int cost, bool canMove, Vector2 nodeWorldPos, Vector2Int gridPos)
        {
            MoveCost = cost;
            CanMove = canMove;
            NodeWorldPos = nodeWorldPos;
            NodeGridPos = gridPos;
        }
    }
}