using System;
using NUnit.Framework;
using UnityEngine;

namespace GGW.game._00.Codes.CCTV
{
    public class CctvRange : MonoBehaviour
    {
        [SerializeField] private LayerMask obstacleLayer;
        [SerializeField] private float coolTime = 0.2f;
        [SerializeField, UnityEngine.Range(0,360)] private float fov = 60f;
        [SerializeField] private  float viewDistance = 5f;
        [SerializeField] private int raycastCount = 50;
        
        private LayerMask _targetLayer;
        private Mesh mesh;
        private bool isIn = true;
        private float curCoolTime;
        
        public bool HasTarget { get; private set; }
        public float LastDetectTime { get; private set; } = float.NegativeInfinity;
        
        public event Action<Transform> FindPlayer;

        public void InitRange(float fovRange, float distance, LayerMask targetLayer)
        {
            fov = fovRange;
            viewDistance = distance;
            _targetLayer = targetLayer;
        }

        private void Start()
        {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
            curCoolTime = coolTime;
        }

        private void Update()
        {
            curCoolTime -= Time.deltaTime;
            if (curCoolTime <= 0)
            {
                isIn = true;
                curCoolTime = coolTime;
            }
        }

        private void LateUpdate()
        {
            HasTarget = false;
            DrawFov();
        }


        public void DrawFov()
        {
            LayerMask fovLayer = obstacleLayer | _targetLayer;
            float angle = fov * 0.5f;
            float angleIncrease = fov / raycastCount;

            Vector3[] vertices = new Vector3[raycastCount + 1 + 1];
            Vector2[] uv = new Vector2[vertices.Length];
            int[] triangles = new int[raycastCount * 3];
        
            vertices[0] = Vector3.zero;
        
            int vertexIndex = 1;
            int trianglesIndex = 0;
            
            for (int i = 0; i <= raycastCount; i++)
            {
                Vector3 vertex;
                
                Vector2 origin = transform.position;
                Vector2 direction = UtilsClass.GetVectorFromAngle(transform.eulerAngles.z + angle).normalized;
                
                RaycastHit2D hit = Physics2D.Raycast(origin, direction, viewDistance, fovLayer);
                
                bool hitSomething = hit.collider != null;
                
                if (!hitSomething)
                {
                    vertex = UtilsClass.GetVectorFromAngle(angle) * viewDistance;
                }
                else
                {
                    bool isTarget = (_targetLayer.value & (1 << hit.collider.gameObject.layer)) != 0;
                    
                    if (isTarget)
                    {
                        // "지금 보이는가" 상태 + 감지 시각: 쓰로틀(isIn)과 무관하게 갱신
                        HasTarget = true;
                        LastDetectTime = Time.time;

                        // 이벤트 발사만 쓰로틀 대상
                        if (isIn)
                        {
                            FindPlayer?.Invoke(hit.collider.transform);
                            isIn = false;
                        }
                    }
                    vertex = UtilsClass.GetVectorFromAngle(angle) * ((hit.distance / viewDistance) * viewDistance);
                }
                vertices[vertexIndex] = vertex;
                if (i > 0)
                {
                    triangles[trianglesIndex + 0] = 0;
                    triangles[trianglesIndex + 1] = vertexIndex - 1;
                    triangles[trianglesIndex + 2] = vertexIndex;

                    trianglesIndex += 3;
                }
            
                ++vertexIndex;
                angle -= angleIncrease;
            }
        
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uv;
        
        }
    }
}


