using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Justin.ProcGen.New
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class MeshGeneration : MonoBehaviour
    {
        [SerializeField] private int xSize;
        [SerializeField] private int zSize;

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
            vertices = new Vector3[]
            {
                new Vector3 (0,0,0),
                new Vector3 (0,0,1),
                new Vector3 (1,0,0),
                new Vector3 (1,0,1)
            };

            triangles = new int[]
            {
                0,1,2,
                2,1,3
            };
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

