using System;
using System.Collections;
using Fabian.Generation._3DGeneration.PerlinNoise;
using UnityEngine;
using UnityEngine.Serialization;

namespace Fabian.Generation._3DGeneration.MeshGen
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))] [ExecuteInEditMode]

    public class MeshGeneration : MonoBehaviour
    {
        //get this from the MapGen script.
        private int _xSize;
        private int _ySize;
        private Vector3[] _vertices;
        private Mesh _mesh;
        [SerializeField] private MapGeneration mapGeneration;

        public void Start()
        {
            _xSize = mapGeneration.mapWidth;
            _ySize = mapGeneration.mapHeight;
            
            GetComponent<MeshFilter>().mesh = _mesh = new Mesh();
            _mesh.name = "Procedural Grid";
            Generate();
        }

        private void Generate()
        {
        
            _vertices = new Vector3[(_xSize + 1) * (_ySize + 1)];
            Vector2[] uv = new Vector2[_vertices.Length];
            
            for (int i = 0, y=0; y <= _ySize; y++)
            {
                for (int x = 0; x <= _xSize; x++, i++)
                {
                    _vertices[i] = new Vector3(x,0, y);
                    uv[i] = new Vector2((float)x / _xSize, (float)y / _ySize);
                }
            }
            
            _mesh.vertices = _vertices;
            _mesh.uv = uv;

            int[] triangles = new int[_xSize * _ySize * 6];

            for (int ti = 0, vi = 0, y = 0; y < _ySize; y++, vi++)
            {
                for (int x = 0; x < _xSize; x++, ti += 6, vi++)
                {
                    triangles[ti] = vi;
                    triangles[ti + 1] = triangles[ti + 4] = vi + _xSize + 1;
                    triangles[ti + 2] = triangles[ti + 3] = vi + 1;
                    triangles[ti + 5] = vi + _xSize + 2;
                }
            }
            _mesh.triangles = triangles;
            _mesh.RecalculateNormals();
        }
    }
}
