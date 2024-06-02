using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerationScript : MonoBehaviour
{
    [SerializeField] private ComputeShader computeShader;
    [SerializeField] private Material meshMaterial;
    [SerializeField] private int userWidth = 10;
    [SerializeField] private int userHeight = 10;

    private ComputeBuffer _vertexBuffer;
    private ComputeBuffer _uvBuffer;
    
    private Mesh _mesh;
    private Vector3[] _vertices;
    private Vector2[] _uvs;
    private int[] _triangles;
    
    private static readonly int Height = Shader.PropertyToID("Height");
    private static readonly int Width = Shader.PropertyToID("Width");
    private static readonly int VertexBuffer = Shader.PropertyToID("VertexBuffer");
    private static readonly int UvBuffer = Shader.PropertyToID("UvBuffer");
    
    public void Start()
    {
        userHeight += 241;
        userWidth += 241;
        
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;
        
        GenerateMeshData();

        ApplyMeshData();
    }

    private void GenerateMeshData()
    {
        int numOfVertices = userWidth * userHeight;
        int numOfTriangles = (userWidth - 1) * (userHeight - 1) * 6;

        _vertexBuffer = new ComputeBuffer(numOfVertices, sizeof(float) * 3);
        _uvBuffer = new ComputeBuffer(numOfVertices, sizeof(float) * 2);
        
        computeShader.SetInt(Width, userWidth);
        computeShader.SetInt(Height, userHeight);
        
        computeShader.SetBuffer(0, VertexBuffer, _vertexBuffer);
        computeShader.SetBuffer(0, UvBuffer, _uvBuffer);
        
        computeShader.Dispatch(0,  userHeight,  userWidth, 1);

        _vertices = new Vector3[numOfVertices];
        _uvs = new Vector2[numOfVertices];
        _triangles = new int[numOfTriangles];
        
        _vertexBuffer.GetData(_vertices);
        _uvBuffer.GetData(_uvs);
        
        _triangles = GenerateTriangles(numOfTriangles); 
        
        _vertexBuffer.Dispose();
        _uvBuffer.Dispose();
    }

    private int[] GenerateTriangles(int triangleAmount)
    {
        int[] tris = new int[triangleAmount];

        int triangleIndex = 0;
        
        for (int y = 0; y <= userHeight - 2; y++)
        {
            for (int x = 0; x <= userWidth - 2; x++)
            {
                int vertexIndex = y * userWidth + x;
                
                tris[triangleIndex] = vertexIndex;
                tris[triangleIndex + 1] = vertexIndex + userWidth;
                tris[triangleIndex + 2] = vertexIndex + userWidth + 1;
                tris[triangleIndex + 3] = vertexIndex;
                tris[triangleIndex + 4] = vertexIndex + userWidth + 1;
                tris[triangleIndex + 5] = vertexIndex + 1;
                
                triangleIndex += 6;
            }
        }
        return tris;
    }
    
    private void ApplyMeshData()
    {
        _mesh.vertices = _vertices;
        _mesh.triangles = _triangles;
        _mesh.uv = _uvs;
        
        _mesh.RecalculateNormals();
        GetComponent<MeshRenderer>().material = meshMaterial;
    }
}
