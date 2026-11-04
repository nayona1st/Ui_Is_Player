using JetBrains.Annotations;
using UnityEngine;

namespace NKY.Script.AStar
{
    public class Chunk : IChunk
    {
        public int ChunkId { get; } // 청크의 고유 id (크기가 다 달라서 id가 없으면 구분이 안됨)
        public int SizeX { get; private set; } // x셀 개수
        public int SizeY { get; private set; } // y 셀 개수
        public INode[,] Nodes { get; private set; } // 보유 노드
        public Vector2Int OriginGridPos { get; private set; } // 청크의 포지션(왼쪽 하단 기준)

        public Chunk(int chunkId,int sizeX, int sizeY, Vector2Int origin)
        {
            ChunkId = chunkId;
            SizeX = sizeX;
            SizeY = sizeY;
            OriginGridPos = origin;
            Nodes = new INode[sizeX, sizeY];
        }

        public bool GetNode(Vector2Int localPos, [CanBeNull] out INode node) // 청크안에서 백터를 주면 그 위치에 있는 노드를 반환하는 함수
        {
            if (localPos.x < 0 || localPos.x >= SizeX ||
                localPos.y < 0 || localPos.y >= SizeY)
            {
                node = null;
                return false;
            }

            node = Nodes[localPos.x, localPos.y];
            return true;
        }
    }
}