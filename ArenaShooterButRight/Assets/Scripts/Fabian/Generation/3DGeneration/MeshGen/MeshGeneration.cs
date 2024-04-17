using System;
using System.Collections;
using Fabian.Generation._3DGeneration.PerlinNoise;
using UnityEngine;
using UnityEngine.Serialization;

namespace Fabian.Generation._3DGeneration.MeshGen
{
    //mesh generation per catlikecoding https://catlikecoding.com/unity/tutorials/procedural-grid/
    public class MeshGeneration
    {
        public static Mesh Generate(int width, int height, float[,] heightMap, float heightMultiplier)
        {
            Mesh mesh = new Mesh();
            Vector3[] vertices = new Vector3[(width + 1) * (height + 1)];
            Vector2[] uv = new Vector2[vertices.Length];
            int[] triangles = new int[(width - 1) * (height - 1) * 6];
            float topLeftX = (width - 1) / -2f;
            float topLeftZ = (height - 2) / 2f;
            int vertexIndex = 0;
            int triangleIndex = 0;

            for (int i = 0, y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++, i++)
                {
                    vertices[i] = new Vector3(topLeftX + x, heightMap[x,y] * heightMultiplier, topLeftZ - y);
                    uv[i] = new Vector2((float)x / width, (float)y / height);

                    if (x < width - 1 && y < height - 1)
                    {
                        triangles[triangleIndex] = triangles[triangleIndex + 4] = vertexIndex;
                        triangles[triangleIndex + 1] = triangles[triangleIndex + 3] = vertexIndex + width + 1;
                        triangles[triangleIndex + 2] = vertexIndex + width;
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
