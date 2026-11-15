using System.Collections.Generic;
using NKY.Script.AStar.Core;
using NKY.Script.AStar.Graph;
using NKY.Script.AStar.Obstacle;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace NKY.Script.AStar.Grid
{
public class PathGridManager : MonoBehaviour
    {
        public static PathGridManager Instance { get; private set; }
        [Header("Map Design")]
        [SerializeField] private Tilemap groundTilemap;
        [SerializeField] private List<BoxCollider2D> obstacleColliders;
        
        [Header("Map Setting")]
        [Tooltip("그리드 기준 (0, 0)의 위치할 트랜스폼")]
        [SerializeField] private Transform originTrm;
        [Tooltip("셀의 사이즈")]
        [SerializeField] private float sellSize = 1f;
        [Tooltip("청크의 가로 사이즈")]
        [SerializeField] private int chunkSizeX = 16;
        [Tooltip("청크의 세로 사이즈")]
        [SerializeField] private int chunkSizeY = 16;
        [Tooltip("% 중요 % 게임 내의 Agent중에서 가장 넓은 반지름을 넣어야함")]
        [SerializeField] private float maxRadius = 2f;

        public PathGrid Grid { get; private set; }
        public GridGraphProvider GraphProvider { get; private set; }
        public Dictionary<Vector2Int, CellClearanceData> ClearanceMap { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
            }
            Initialize();
        }

        public void Initialize()
        {
            Vector2 originPos = originTrm != null ? (Vector2)originTrm.position : Vector2.zero;
            Grid = new PathGrid(sellSize, originPos);
            
            CreateChunks();
            
            GraphProvider = new GridGraphProvider(Grid);
            
            BakeMap();

            Debug.Log($"청크 생성 완료: {Grid.Chunks.Count}개");
        }

        public void BakeMap()
        {
            // BoxCollider2D 목록을 ObstacleBox 구조체 리스트로 변환
            List<ObstacleBox> boxes = new List<ObstacleBox>();
            foreach (var col in obstacleColliders)
            {
                if (col != null)
                    boxes.Add(GraphAStar.FromCollider(col));
            }

            // Clearance 베이킹 수행
            ClearanceMap = MapBaker.Bake(Grid, boxes, maxRadius);
            
            // 베이킹 결과를 그래프 프로바이더에 설정
            GraphProvider.SetClearanceMap(ClearanceMap);
        }

        private void CreateChunks()
        {
            BoundsInt bounds = groundTilemap.cellBounds;
            Vector2Int gridStartPos = new Vector2Int(bounds.xMin, bounds.yMin);

            int offsetX = 0;
            int offsetY = 0;

            int mapSizeX = bounds.size.x;
            int mapSizeY = bounds.size.y;
            
            Vector2Int chunkPos = gridStartPos;

            while (offsetX < mapSizeX)
            {
                int leftSizeX = mapSizeX - offsetX;
                int sizeX = Mathf.Min(chunkSizeX, leftSizeX);
                
                while (offsetY < mapSizeY)
                {
                    int leftSizeY = mapSizeY - offsetY;
                    int sizeY = Mathf.Min(chunkSizeY, leftSizeY);

                    if (!Grid.TryAddChunk(sizeX, sizeY, chunkPos, gridPos =>
                        {
                            bool hasTile = groundTilemap.HasTile(
                                new Vector3Int(gridPos.x, gridPos.y, 0));
                            return new TileInfo(1, hasTile);
                        }))
                        return;

                    chunkPos.y += sizeY;
                    offsetY += sizeY;
                }

                chunkPos.x += sizeX;
                chunkPos.y = gridStartPos.y;
                offsetX += sizeX;
                offsetY = 0;
            }
        }
    }
}