using UnityEngine;
using UnityEngine.Rendering;

namespace Justin.ProcGen
{
    /// <summary>
    /// The MeshGenerator class provides functionality to generate a 3D mesh from a height map.
    /// It creates the vertices, triangles, normals, and UVs necessary to construct a terrain mesh.
    /// The class also supports different levels of detail (LoD) to optimize the mesh for performance.
    /// </summary>
    public static class MeshGenerator
    {
        /// <summary>
        /// Generates the terrain mesh based on the height map and other parameters.
        /// </summary>
        /// <param name="heightMap">2D array representing the height values of the terrain.</param>
        /// <param name="meshHeightMultiplier">Multiplier to scale the height of the terrain.</param>
        /// <param name="meshHeightCurve">Curve that adjusts the height based on the height map values.</param>
        /// <param name="levelOfDetail">Level of detail to reduce the number of vertices for optimization.</param>
        /// <returns>MeshData object containing vertices, triangles, normals, and UVs of the terrain mesh.</returns>
        public static MeshData GenerateTerrainMesh(float[,] heightMap, float meshHeightMultiplier, AnimationCurve meshHeightCurve, int levelOfDetail)
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);

            // Determines the step size for vertices based on the set level of detail.
            // If levelOfDetail is 0, use all vertices (no simplification). Otherwise, increase step size to skip vertices.
            int lodIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
            int verticesPerLine = (width - 1) / lodIncrement + 1;

            // Create MeshData with space for vertices, triangles, normals, and UVs.
            MeshData meshData = new MeshData(verticesPerLine * verticesPerLine * 6);

            for (int y = 0; y < height - 1; y += lodIncrement)
            {
                for (int x = 0; x < width - 1; x += lodIncrement)
                {
                    // Define the four vertices of the current quad.
                    Vector3[] quadVertices = new Vector3[]
                    {
                    new Vector3(x, meshHeightCurve.Evaluate(heightMap[x, y]) * meshHeightMultiplier, y),
                    new Vector3(x + lodIncrement, meshHeightCurve.Evaluate(heightMap[x + lodIncrement, y]) * meshHeightMultiplier, y),
                    new Vector3(x, meshHeightCurve.Evaluate(heightMap[x, y + lodIncrement]) * meshHeightMultiplier, y + lodIncrement),
                    new Vector3(x + lodIncrement, meshHeightCurve.Evaluate(heightMap[x + lodIncrement, y + lodIncrement]) * meshHeightMultiplier, y + lodIncrement)
                    };

                    // Add the two triangles that make up the current quad.
                    meshData.AddTriangle(quadVertices[0], quadVertices[2], quadVertices[1], verticesPerLine);
                    meshData.AddTriangle(quadVertices[1], quadVertices[2], quadVertices[3], verticesPerLine);
                }
            }

            return meshData;
        }
    }

    /// <summary>
    /// The MeshData class stores the data needed to construct a mesh, including vertices, triangles, normals, and UVs.
    /// It provides methods to add triangles and create the mesh.
    /// </summary>
    public class MeshData
    {
        public Vector3[] vertices;
        public int[] triangles;
        public Vector3[] normals;
        public Vector2[] uvs;

        private int triangleIndex;

        /// <summary>
        /// Constructor for MeshData that initializes the arrays with a specified size.
        /// </summary>
        /// <param name="vertexCount">The number of vertices that will be in the mesh.</param>
        public MeshData(int vertexCount)
        {
            vertices = new Vector3[vertexCount];
            uvs = new Vector2[vertexCount];
            triangles = new int[vertexCount];
            normals = new Vector3[vertexCount];
        }

        /// <summary>
        /// Adds a triangle to the mesh using three vertices.
        /// </summary>
        /// <param name="v0">First vertex of the triangle.</param>
        /// <param name="v1">Second vertex of the triangle.</param>
        /// <param name="v2">Third vertex of the triangle.</param>
        /// <param name="verticesPerLine">Number of vertices per line in the mesh, used for UV mapping.</param>
        public void AddTriangle(Vector3 v0, Vector3 v1, Vector3 v2, int verticesPerLine)
        {
            int vertexIndex = triangleIndex;

            // Add vertices to the array.
            vertices[vertexIndex] = v0;
            vertices[vertexIndex + 1] = v1;
            vertices[vertexIndex + 2] = v2;

            // Add triangle indices to the array.
            triangles[triangleIndex] = vertexIndex;
            triangles[triangleIndex + 1] = vertexIndex + 1;
            triangles[triangleIndex + 2] = vertexIndex + 2;

            // Calculate normals for the lighting.
            Vector3 normal = Vector3.Cross(v1 - v0, v2 - v0).normalized;
            normals[vertexIndex] = normal;
            normals[vertexIndex + 1] = normal;
            normals[vertexIndex + 2] = normal;

            // Calculate UVs based on vertex positions.
            uvs[vertexIndex] = new Vector2(v0.x / (verticesPerLine - 1), v0.z / (verticesPerLine - 1));
            uvs[vertexIndex + 1] = new Vector2(v1.x / (verticesPerLine - 1), v1.z / (verticesPerLine - 1));
            uvs[vertexIndex + 2] = new Vector2(v2.x / (verticesPerLine - 1), v2.z / (verticesPerLine - 1));


            triangleIndex += 3;
        }

        /// <summary>
        /// Creates the mesh from the MeshData by assigning vertices, triangles, normals, and UVs.
        /// </summary>
        /// <returns>The generated Mesh object ready to be used in Unity.</returns>
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