using UnityEngine;

namespace NKY.Script.AStar.Grid
{
    public interface INode
    {
        int MoveCost { get; }
        bool CanMove { get; }
        Vector2 NodeWorldPos { get; }
        Vector2Int NodeGridPos { get; }
        void SetNode(int cost, bool canMove);
    }
}