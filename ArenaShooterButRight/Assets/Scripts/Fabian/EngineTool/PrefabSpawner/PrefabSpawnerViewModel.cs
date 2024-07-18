using System;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Mathematics;
using UnityEngine;

namespace Fabian.EngineTool.PrefabSpawner
{
    public class PrefabSpawnerViewModel : INotifyPropertyChanged
    {
        private PrefabSpawnerModel _model;
        public event PropertyChangedEventHandler PropertyChanged;

        public PrefabSpawnerViewModel()
        {
            _model = new PrefabSpawnerModel();
        }

        public List<GameObject> PrefabChoiceList => _model.PrefabChoiceLst;
        public List<GameObject> SpawnedPrefabs => _model.SpawnedPrefabs;
        public Dictionary<GameObject, Transform> PositionDictionary => _model.PositionDictionary;
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

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ClearSpawnedObjects()
        {
            foreach (var obj in _model.SpawnedPrefabs)
            {
                UnityEngine.Object.DestroyImmediate(obj);
            }
            _model.SpawnedPrefabs.Clear();
            _model.PositionDictionary.Clear();
        }

        public void SpawnPrefabs(Vector3 position, string layerName)
        {
            int rnd = UnityEngine.Random.Range(0, _model.PrefabChoiceLst.Count);
            GameObject prefab =
                UnityEngine.Object.Instantiate(_model.PrefabChoiceLst[rnd], position + new Vector3(UnityEngine.Random.insideUnitCircle.x * _model.Radius, 0, UnityEngine.Random.insideUnitCircle.y * _model.Radius), quaternion.identity);
            CheckForOverlappingObjects(prefab);

            if (prefab == null) return;
            
            prefab.AddComponent<MeshCollider>();
            prefab.GetComponent<MeshRenderer>().sharedMaterial.enableInstancing = true;
            prefab.layer = LayerMask.NameToLayer(layerName);
            
            _model.PositionDictionary.Add(prefab, prefab.transform);
            _model.SpawnedPrefabs.Add(prefab);
        }

        public void DeletePrefabs(string layerName)
        {
            if (_model.FoundCollidersForDeletion == null) return;

            foreach (Collider coll in _model.FoundCollidersForDeletion)
            {
                if (coll.gameObject.layer != LayerMask.NameToLayer(layerName)) continue;

                if (!_model.PositionDictionary.ContainsKey(coll.gameObject)) continue;

                _model.PositionDictionary.Remove(coll.gameObject);
                UnityEngine.Object.DestroyImmediate(coll.gameObject);
            }
        }

        private void CheckForOverlappingObjects(GameObject o)
        {
            foreach (KeyValuePair<GameObject, Transform> obj in _model.PositionDictionary)
            {
                if (obj.Value != null && Vector3.Distance(o.transform.position, obj.Value.position) < _model.MinDistanceBetweenPrefabs)
                {
                    UnityEngine.Object.DestroyImmediate(o);
                    return;
                }
            }
        }
    }
}
