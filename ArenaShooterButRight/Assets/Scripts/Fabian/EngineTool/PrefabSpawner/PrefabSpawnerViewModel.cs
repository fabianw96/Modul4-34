using System;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Mathematics;
using UnityEngine;

namespace Fabian.EngineTool.PrefabSpawner
{
    public class PrefabSpawnerViewModel : INotifyPropertyChanged
    {
        private PrefabSpawnerModel _model = new PrefabSpawnerModel();
        
        public event PropertyChangedEventHandler PropertyChanged;

        public List<GameObject> PrefabChoiceList
        {
            get => _model.PrefabChoiceLst;
            private set => _model.PrefabChoiceLst = value;
        }

        private List<GameObject> SpawnedPrefabs
        {
            get => _model.SpawnedPrefabs;
            set => _model.SpawnedPrefabs = value;
        }

        private Dictionary<GameObject, Transform> PositionDictionary => _model.PositionDictionary;
        public List<String> LayerMasks => _model.LayerMasks;
        public Collider[] FoundCollidersForDeletion
        {
            get => _model.FoundCollidersForDeletion;
            set => _model.FoundCollidersForDeletion = value;
        }

        public float Radius
        {
            get => _model.Radius;
            set
            {
                if (!Mathf.Approximately(_model.Radius, value))
                {
                    _model.Radius = value;
                    OnPropertyChanged(nameof(Radius));
                }
            }
        }

        public float MinDistanceBetweenPrefabs
        {
            get => _model.MinDistanceBetweenPrefabs;
            set
            {
                if (!Mathf.Approximately(_model.MinDistanceBetweenPrefabs, value))
                {
                    _model.MinDistanceBetweenPrefabs = value;
                    OnPropertyChanged(nameof(MinDistanceBetweenPrefabs));
                }
            }
        }

        public String ChosenLayer { get; private set; }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ClearSpawnedObjects()
        {
            foreach (var obj in SpawnedPrefabs)
            {
                UnityEngine.Object.DestroyImmediate(obj);
            }
            SpawnedPrefabs.Clear();
            PositionDictionary.Clear();
        }

        public void SpawnPrefabs(Vector3 position, string layerName, bool checkForOverlap)
        {
            int rnd = UnityEngine.Random.Range(0, PrefabChoiceList.Count);
            GameObject prefab =
                UnityEngine.Object.Instantiate(PrefabChoiceList[rnd], position + new Vector3(UnityEngine.Random.insideUnitCircle.x * Radius, 0, UnityEngine.Random.insideUnitCircle.y * Radius), quaternion.identity);

            if (checkForOverlap)
            {
                CheckForOverlappingObjects(prefab);
            }

            if (prefab == null) return;
            
            prefab.AddComponent<MeshCollider>();
            prefab.GetComponent<MeshRenderer>().sharedMaterial.enableInstancing = true;
            prefab.layer = LayerMask.NameToLayer(layerName);
            
            PositionDictionary.Add(prefab, prefab.transform);
            SpawnedPrefabs.Add(prefab);
        }

        public void DeletePrefabs(string layerName)
        {
            if (FoundCollidersForDeletion == null) return;

            foreach (Collider coll in FoundCollidersForDeletion)
            {
                if (coll.gameObject.layer != LayerMask.NameToLayer(layerName)) continue;

                if (!PositionDictionary.ContainsKey(coll.gameObject)) continue;

                PositionDictionary.Remove(coll.gameObject);
                UnityEngine.Object.DestroyImmediate(coll.gameObject);
            }
        }

        private void CheckForOverlappingObjects(GameObject o)
        {
            foreach (KeyValuePair<GameObject, Transform> obj in PositionDictionary)
            {
                if (obj.Value != null && Vector3.Distance(o.transform.position, obj.Value.position) < MinDistanceBetweenPrefabs)
                {
                    Debug.Log("Found Overlapping objects, deleting.");
                    UnityEngine.Object.DestroyImmediate(o);
                    return;
                }
            }
        }

        public void SaveDataToJson(String sceneName, string lastUsedLayer)
        {
            String jsonPath = Application.dataPath + "/PrefabSpawner-"+ sceneName +".json";
            PrefabSpawnerContainer spawnerContainer = new PrefabSpawnerContainer
            {
                prefabChoiceLst = PrefabChoiceList,
                spawnedPrefabs = SpawnedPrefabs,
                layer =  lastUsedLayer,
                radius = Radius,
                minDistanceBetweenPrefabs = MinDistanceBetweenPrefabs
            };

            String jsonString = JsonUtility.ToJson(spawnerContainer);
            
            if (System.IO.File.Exists(jsonPath))
            {
                System.IO.File.Delete(jsonPath);
            }
            
            System.IO.File.WriteAllText(jsonPath, jsonString);
            Debug.Log("Saved to: " + jsonPath);
        }

        public void LoadDataFromJson(String sceneName)
        {
            PrefabChoiceList.Clear();
            SpawnedPrefabs.Clear();
            PositionDictionary.Clear();
            
            String jsonPath = Application.dataPath + "/PrefabSpawner-"+ sceneName +".json";
            Debug.Log(jsonPath);

            if (!System.IO.File.Exists(jsonPath))
            {
                Debug.LogWarning("No save found for this Scene!");
                return;
            }
            
            String loadedJsonString = System.IO.File.ReadAllText(jsonPath);
            PrefabSpawnerContainer spawnerContainer = JsonUtility.FromJson<PrefabSpawnerContainer>(loadedJsonString);

            PrefabChoiceList = spawnerContainer.prefabChoiceLst;
            SpawnedPrefabs = spawnerContainer.spawnedPrefabs;
            ChosenLayer = spawnerContainer.layer;
            
            int iterations = SpawnedPrefabs.Count;
            for (int i = 0; i < iterations; i++)
            {
                if (SpawnedPrefabs[0] == null)
                {
                    SpawnedPrefabs.RemoveAt(0);
                }
            }

            foreach (GameObject obj in SpawnedPrefabs)
            {   
                PositionDictionary.Add(obj, obj.transform);
            }
            
            Debug.LogWarning("Found " + PositionDictionary.Count + " out of " + iterations + " Objects. Lost objects have been deleted manually.");
            
            Radius = spawnerContainer.radius;
            MinDistanceBetweenPrefabs = spawnerContainer.minDistanceBetweenPrefabs;
        }
    }
}
