using UnityEngine;
using UnityEngine.Rendering;

namespace Justin.ProcGen
{
    public static class MeshGenerator
    {
        // Generate the Terrain-Mesh based on the Height of the NoiseMap
        public static MeshData GenerateTerrainMesh(float[,] heightMap, float meshHeightMultiplier, AnimationCurve meshHeightCurve, int levelOfDetail)
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);

            // Set levelOfDetail to 1 otherwise multiply by 2
            int lodIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
            int verticesPerLine = (width - 1) / lodIncrement + 1;

            // Create Meshdata with more space for vertices, to enable the hard edges look
            MeshData meshData = new MeshData(verticesPerLine * verticesPerLine * 6);

            // Loop through every Mapposition
            for (int y = 0; y < height - 1; y += lodIncrement)
            {
                for (int x = 0; x < width - 1; x += lodIncrement)
                {
                    // Define the 4 edges of the current Quad
                    Vector3[] quadVertices = new Vector3[]
                    {
                    new Vector3(x, meshHeightCurve.Evaluate(heightMap[x, y]) * meshHeightMultiplier, y),
                    new Vector3(x + lodIncrement, meshHeightCurve.Evaluate(heightMap[x + lodIncrement, y]) * meshHeightMultiplier, y),
                    new Vector3(x, meshHeightCurve.Evaluate(heightMap[x, y + lodIncrement]) * meshHeightMultiplier, y + lodIncrement),
                    new Vector3(x + lodIncrement, meshHeightCurve.Evaluate(heightMap[x + lodIncrement, y + lodIncrement]) * meshHeightMultiplier, y + lodIncrement)
                    };

                    // Add the two triangles of the Quad
                    meshData.AddTriangle(quadVertices[0], quadVertices[2], quadVertices[1], verticesPerLine);
                    meshData.AddTriangle(quadVertices[1], quadVertices[2], quadVertices[3], verticesPerLine);
                }
            }

            return meshData;
        }
    }

    public class MeshData
    {
        public Vector3[] vertices;
        public int[] triangles;
        public Vector3[] normals;
        public Vector2[] uvs;

        private int triangleIndex;

        public MeshData(int vertexCount)
        {
            vertices = new Vector3[vertexCount];
            uvs = new Vector2[vertexCount];
            triangles = new int[vertexCount];
            normals = new Vector3[vertexCount];
        }

        // Method to add Triangles
        public void AddTriangle(Vector3 v0, Vector3 v1, Vector3 v2, int verticesPerLine)
        {
            int vertexIndex = triangleIndex;

            // Add Vertices
            vertices[vertexIndex] = v0;
            vertices[vertexIndex + 1] = v1;
            vertices[vertexIndex + 2] = v2;

            // Calculate the Normal, to receive hard edges
            Vector3 normal = Vector3.Cross(v1 - v0, v2 - v0).normalized;
            normals[vertexIndex] = normal;
            normals[vertexIndex + 1] = normal;
            normals[vertexIndex + 2] = normal;

            //// Set UV-Coordinates
            uvs[vertexIndex] = new Vector2(v0.x / (verticesPerLine - 1), v0.z / (verticesPerLine - 1));
            uvs[vertexIndex + 1] = new Vector2(v1.x / (verticesPerLine - 1), v1.z / (verticesPerLine - 1));
            uvs[vertexIndex + 2] = new Vector2(v2.x / (verticesPerLine - 1), v2.z / (verticesPerLine - 1));

            // Add the indices for the Triangels
            triangles[triangleIndex] = vertexIndex;
            triangles[triangleIndex + 1] = vertexIndex + 1;
            triangles[triangleIndex + 2] = vertexIndex + 2;

            triangleIndex += 3;
        }

        // Method to Create the Mesh
        public Mesh CreateMesh()
        {
            Mesh mesh = new Mesh();
            mesh.indexFormat = IndexFormat.UInt32;
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.normals = normals;
            mesh.uv = uvs;
            return mesh;
        }
    }
}