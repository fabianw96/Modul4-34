using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Fabian.Generation._3DGeneration.MeshGen
{
    //mesh generation per catlikecoding https://catlikecoding.com/unity/tutorials/procedural-grid/
    public static class MeshGeneration
    {
        public static MeshData Generate(float[,] heightMap, float heightMultiplier, AnimationCurve curve, int levelOfDetail)
        {
            AnimationCurve heightCurve = new AnimationCurve(curve.keys);
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);
            int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
            int verticesPerLine = (width - 1) / meshSimplificationIncrement + 1;

            MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);
            int[] triangles = new int[(verticesPerLine - 1) * (verticesPerLine - 1) * 6];
            
            float topLeftX = (width - 1) / -2f;
            float topLeftZ = (height - 2) / 2f;
            
            int vertexIndex = 0;
            int triangleIndex = 0;

            for (int i = 0, y = 0; y < height; y += meshSimplificationIncrement)
            {
                for (int x = 0; x < width; x += meshSimplificationIncrement, i++)
                {
                    meshData.Vertices[i] = new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[x,y]) * heightMultiplier, topLeftZ - y);
                    meshData.Uvs[i] = new Vector2((float)x / width, (float)y / height);

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
            
            meshData.Triangles = triangles;
            
            return meshData;
        }
    }
    
    public class MeshData {
        public Vector3[] Vertices;
        public int[] Triangles;
        public Vector2[] Uvs;

        public MeshData(int meshWidth, int meshHeight) {
            Vertices = new Vector3[meshWidth * meshHeight];
            Uvs = new Vector2[meshWidth * meshHeight];
            Triangles = new int[(meshWidth-1)*(meshHeight-1)*6];
        }

        public Mesh CreateMesh() {
            Mesh mesh = new Mesh ();
            mesh.vertices = Vertices;
            mesh.triangles = Triangles;
            mesh.uv = Uvs;
            mesh.RecalculateNormals ();
            return mesh;
        }

    }
}
