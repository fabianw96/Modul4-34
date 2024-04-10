using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshGeneration : MonoBehaviour
{
    [SerializeField] private int xSize;
    [SerializeField] private int ySize;
    private Vector3[] _vertices;

    private void Awake()
    {
        StartCoroutine(Generate());
    }

    private IEnumerator Generate()
    {
        
        _vertices = new Vector3[(xSize + 1) * (ySize + 1)];

        for (int i = 0, y=0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                _vertices[i] = new Vector3(x, y);
                yield return new WaitForSeconds(0.05f);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (_vertices == null)
        {
            return;
        }
        
        Gizmos.color = Color.black;
        for (int i = 0; i < _vertices.Length; i++)
        {
            Gizmos.DrawSphere(transform.TransformPoint(_vertices[i]), 0.1f);
        }
    }
}
