using UnityEngine;

namespace NKY.Script.AStar
{
    public class PathGridDebugTest : MonoBehaviour
    {
        private void Start()
        {
            PathGrid grid = new PathGrid(1f, Vector2.zero);

            bool addedA = grid.TryAddChunk(
                4, 3, new Vector2Int(0, 0));

            bool addedB = grid.TryAddChunk(
                2, 3, new Vector2Int(4, 0));

            Debug.Log($"청크 A 추가: {addedA}");
            Debug.Log($"청크 B 추가: {addedB}");
            
            INode lastA = grid.GetNodeByGrid(new Vector2Int(3, 1));
            INode firstB = grid.GetNodeByGrid(new Vector2Int(4, 1));

            Debug.Log($"A 마지막 셀 존재: {lastA != null}");
            Debug.Log($"B 첫 번째 셀 존재: {firstB != null}");

            IChunk chunkA = grid.GetChunkByGrid(new Vector2Int(3, 1));
            IChunk chunkB = grid.GetChunkByGrid(new Vector2Int(4, 1));

            Debug.Log($"A 셀의 청크 ID: {chunkA?.ChunkId}");
            Debug.Log($"B 셀의 청크 ID: {chunkB?.ChunkId}");
            
            INode nullNode = grid.GetNodeByGrid(new Vector2Int(-1, -5));
            
            Debug.Log($"널 셀 존재: {nullNode == null} // true 나와야함");

            bool addedC = grid.TryAddChunk(2, 2, new Vector2Int(-1, -1));
            
            Debug.Log($"청크 C 추가: {addedC} // false가 나와야함");
        }
    }
}