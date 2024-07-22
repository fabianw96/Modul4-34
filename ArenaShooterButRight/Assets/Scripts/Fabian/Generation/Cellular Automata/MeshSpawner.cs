using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Fabian.Generation.Cellular_Automata
{
    [ExecuteInEditMode]
    public class MeshSpawner : MonoBehaviour
    {
        //Generate planes, depending on the noise value set their color to White or Black.
        //Black planes are Dead planes, White planes are Alive planes.
        
        [SerializeField] private Vector3 size;
        [SerializeField] private Material defaultMaterial;
        [SerializeField] private bool spawnCubes;
        [SerializeField] private CellularAutomaton cellularAutomaton;
        
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private GameObject _generatedGameObject;
        private Mesh _generatedMesh;
        
        public List<GameObject> spawnedMeshList;


        private void OnEnable()
        {
            if (spawnedMeshList != null)
            {
                foreach (var obj in spawnedMeshList)
                {
                    DestroyImmediate(obj);
                }
                spawnedMeshList.Clear();
            }
            
            SpawnMeshes();
            cellularAutomaton.GetListFromSpawner(spawnedMeshList);
        }

        private void SpawnMeshes()
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    for (int z = 0; z < size.z; z++)
                    {
                        _generatedGameObject = new GameObject
                        {
                            name = "Object " + x + y + z,
                            transform =
                            {
                                position = new Vector3(x + 0, y + 0, z + 0)
                            },
                        };
                    
                        _generatedGameObject.transform.SetParent(gameObject.transform);
                    
                        _meshFilter = _generatedGameObject.AddComponent<MeshFilter>();
                        _meshRenderer = _generatedGameObject.AddComponent<MeshRenderer>();
            
                        _meshRenderer.sharedMaterial = defaultMaterial;

                        _meshFilter.sharedMesh = !spawnCubes ? GeneratePlane() : GenerateCube();
                    
                        spawnedMeshList.Add(_generatedGameObject);
                    }
                }
            }
            
            Debug.Log("Spawned all meshes!");
        }

        private Mesh GenerateCube()
        {
            Mesh mesh = new Mesh
            {
                name = "Cube Mesh"
            };
            
            mesh.Clear();

            mesh.vertices = new[]
            {
                //Front 0 - 3
                new Vector3(0,0,0),
                new Vector3(1,0,0),
                new Vector3(0,1,0),
                new Vector3(1,1,0),
                //Back 4 - 7
                new Vector3(0,0,1),
                new Vector3(1,0,1),
                new Vector3(0,1,1),
                new Vector3(1,1,1),
                //bot 8 - 11
                new Vector3(0,0,1),
                new Vector3(1,0,1),
                new Vector3(0,0,0),
                new Vector3(1,0,0),
                //top 12 - 15
                new Vector3(0,1,0),
                new Vector3(1,1,0),
                new Vector3(0,1,1),
                new Vector3(1,1,1),
                //right 16 - 19
                new Vector3(1,0,0),
                new Vector3(1,0,1),
                new Vector3(1,1,0),
                new Vector3(1,1,1),
                //left 20 - 23
                new Vector3(0,0,1),
                new Vector3(0,0,0),
                new Vector3(0,1,1),
                new Vector3(0,1,0),
            };

            mesh.triangles = new[]
            {
                //front
                0,2,1,
                1,2,3,
                //back
                4,5,6,
                6,5,7,
                //bot
                8,10,9,
                9,10,11,
                //top
                12,14,13,
                13,14,15,
                //right
                16, 18, 17,
                17, 18, 19,
                //left
                20, 22, 21,
                21, 22, 23
            };
            
            mesh.RecalculateNormals();
            return mesh;
        }

        private Mesh GeneratePlane()
        {
            Mesh mesh = new Mesh
            {
                name = "Plane Mesh"
            };

            mesh.Clear();
            mesh.vertices = new []
            {
                new Vector3(0,0,0),
                new Vector3(1,0,0),
                new Vector3(0,0,1),
                new Vector3(1,0,1),
            };
            mesh.triangles = new []
            {
                0,2,1,
                1,2,3
            };
            
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}
