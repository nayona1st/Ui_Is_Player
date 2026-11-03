using System;
using Unity.Mathematics;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEditor.PackageManager;
using UnityEngine.Diagnostics;

public class CCTV : MonoBehaviour
{
    [Range(0,360)]float fov = 60f;
    private int raycastCount = 50;
    private  float viewDistance = 5f;
    private Mesh mesh;
    
    public Action<Transform> _findPlayer;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        
    }

    private void LateUpdate()
    {
        DrawFov();
    }


    private void DrawFov()
    {
        float angle = fov / 0.5f;
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
            RaycastHit2D hit2D =Physics2D.Raycast(transform.position,
                UtilsClass.GetVectorFromAngle(transform.eulerAngles.z + angle), viewDistance);
            if (hit2D.collider == null)
            {
                vertex = UtilsClass.GetVectorFromAngle(angle) * viewDistance;
            }
            else
            {
                if (hit2D.collider.CompareTag("Player"))
                {
                    _findPlayer?.Invoke(hit2D.collider.transform);
                }
                vertex = UtilsClass.GetVectorFromAngle(angle) * (hit2D.distance / viewDistance) * viewDistance;
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
