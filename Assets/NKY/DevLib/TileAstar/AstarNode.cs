using System;
using UnityEngine;

namespace DevLib.TileAstar
{
    public class AstarNode : IComparable<AstarNode>
    {
        public Vector2 worldPosition;
        public Vector3Int cellPosition; // vector3Int (1,1,1) (1,0,1) 
        public NodeData nodeData; // 나중에 만들 구조체 

        public AstarNode parentNode; // 부모 노드
        
        public float G;
        public float F; // 가장 작은애 F = G+H
        
        //H는 걍 구해
        
        public int CompareTo(AstarNode other)
        {
            if (Mathf.Approximately(other.F, F)) // 다른노드랑 같다면 0
                return 0;
            return other.F < F ? -1 : 1; // 오름차순 정렬을 만든다
            // 다른 노드가 내 노드보다 작다면 -1
            // 다른 노드가 내 노드보다 크다면 1
        }

        public override bool Equals(object obj)
        {
            if (obj is AstarNode node)
                return Equals(node);
            
            return false;
        }

        public bool Equals(AstarNode node)
        {
            if (node is null) return false; // 비교 노드가 비어있으면
            return cellPosition == node.cellPosition; // 셀포지션이 같으면 같다 판단.
        }

        public override int GetHashCode() => cellPosition.GetHashCode();

        public static bool operator ==(AstarNode lhs, AstarNode rhs) // 오퍼레이터 오버로딩 
        {
            if (lhs is null)
            {
                if (rhs is null) return true; // 둘다 널이면 true
                return false; // 아니면 널 == 널 안되니까 false
            }
            return lhs.Equals(rhs); // 둘다 널이 아닐경우 비교해서 cellPosition으로 판단
        }
        public static bool operator != (AstarNode lhs , AstarNode rhs) => !(lhs == rhs); // 둘이 다르면 리턴
        
        // 프라이어티 큐 (우선순위 큐)
        // min-heap 기본 트리식 노드에서 자식이 부모보다 작으면 부모를 이기고 자식이 위로 올라감 
    }
}