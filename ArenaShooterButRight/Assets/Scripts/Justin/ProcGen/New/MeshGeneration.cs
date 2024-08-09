using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

namespace Justin.ProcGen.New
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class MeshGeneration : MonoBehaviour
    {
        [Header("Map Values")]
        [SerializeField] private int xSize;
        [SerializeField] private int zSize;

        [Header("Noise Values")]
        [SerializeField] private int seed;
        [SerializeField] private int octaves;


        private Mesh m_Mesh;
        private Vector3[] vertices;
        private int[] triangles;

        // Start is called before the first frame update
        void Start()
        {
            m_Mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = m_Mesh;

            CreateMesh();
            UpdateMesh();
        }

        private void CreateMesh()
        {
            vertices = new Vector3[(xSize + 1) * (zSize + 1)];

            for (int i = 0, z = 0; z <= zSize; z++)
            {
                for (int x = 0; x <= xSize; x++)
                {
                    
                    vertices[i] = new Vector3(x, 0, z);
                    i++;
                }
            }

            triangles = new int[xSize * zSize * 6];
            int vert = 0;
            int tris = 0;

            for (int z = 0; z < zSize; z++)
            {
                for (int x = 0; x < xSize; x++)
                {
                    triangles[tris + 0] = vert + 0;
                    triangles[tris + 1] = vert + xSize + 1;
                    triangles[tris + 2] = vert + 1;
                    triangles[tris + 3] = vert + 1;
                    triangles[tris + 4] = vert + xSize + 1;
                    triangles[tris + 5] = vert + xSize + 2;

                    vert++;
                    tris += 6;
                }
                vert++;
            }
        }

        private void UpdateMesh()
        {
            m_Mesh.Clear();

            m_Mesh.vertices = vertices;
            m_Mesh.triangles = triangles;

            m_Mesh.RecalculateNormals();
        }
    }
}

