using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Fabian.Generation._3DGeneration.MeshGen
{
    //mesh generation per catlikecoding https://catlikecoding.com/unity/tutorials/procedural-grid/
    public static class MeshGeneration
    {
        public static Mesh Generate(int width, int height, float[,] heightMap, float heightMultiplier, AnimationCurve curve, int levelOfDetail)
        {
            Mesh mesh = new Mesh();
            int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
            int verticesPerLine = (width - 1) / meshSimplificationIncrement + 1;
            
            Vector3[] vertices = new Vector3[verticesPerLine * verticesPerLine];
            Vector2[] uv = new Vector2[vertices.Length];
            int[] triangles = new int[(verticesPerLine - 1) * (verticesPerLine - 1) * 6];
            
            float topLeftX = (width - 1) / -2f;
            float topLeftZ = (height - 2) / 2f;
            
            int vertexIndex = 0;
            int triangleIndex = 0;

            for (int i = 0, y = 0; y < height; y += meshSimplificationIncrement)
            {
                for (int x = 0; x < width; x += meshSimplificationIncrement, i++)
                {
                    vertices[i] = new Vector3(topLeftX + x, curve.Evaluate(heightMap[x,y]) * heightMultiplier, topLeftZ - y);
                    uv[i] = new Vector2((float)x / width, (float)y / height);

                    if (x < width - 1 && y < height - 1)
                    {
                        triangles[triangleIndex] = triangles[triangleIndex + 4] = vertexIndex;
                        triangles[triangleIndex + 1] = triangles[triangleIndex + 3] = vertexIndex + verticesPerLine + 1;
                        triangles[triangleIndex + 2] = vertexIndex + verticesPerLine;
                        triangles[triangleIndex + 5] = vertexIndex + 1;
                        triangleIndex += 6;
                    }

                    vertexIndex++;
                }
            }

            mesh.name = "Proc Gen";
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }
    }
}
