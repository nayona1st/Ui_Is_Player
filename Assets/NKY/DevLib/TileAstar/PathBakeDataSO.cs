using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DevLib.TileAstar
{
    [CreateAssetMenu(fileName = "Path bake data", menuName = "Lib/TileAster/Bake Data", order = 0)]
    public class PathBakeDataSO : ScriptableObject
    {
        public List<NodeData> points = new List<NodeData>();
        private Dictionary<Vector3Int, NodeData> pointsDict; // 런타임 전용 딕셔너리

        public void InitializeBakeData()
        {
            if (pointsDict == null || points.Count != pointsDict.Count)
                pointsDict = points.ToDictionary(node => node.cellPosition); // Points 들을 ToDict로 딕셔너리화
            // 고유한 cellPosition이 딕셔너리의 키 , NodeData가 Value가 됨
        }
        
        public void ClearPoint() => points?.Clear();

        public void AddPoint(Vector3 worldPosition, Vector3Int cellPosition)
        {
            points.Add(new NodeData(worldPosition, cellPosition));
        }
        
        public bool HasNode(Vector3Int cellPosition)
            => pointsDict != null && pointsDict.ContainsKey(cellPosition); // 사전에 같은 키가 존재하는지 확인
        // 있다면 true , 없다면 false

        public bool GetNodeIfExist(Vector3Int cellPosition, out NodeData node)
        {
            if (HasNode(cellPosition)) // 가져올려는 cellPosition이 사전에 있으면 가져와줌
            {
                node = pointsDict[cellPosition];
                return true; // 성공
            }

            node = default;
            return false; // 실패
        }
    }
}