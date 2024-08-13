using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fabian.EngineTool.PrefabSpawner
{
    public class PrefabSpawnerModel
    {
        public List<GameObject> PrefabChoiceLst { get; set; } = new List<GameObject>();
        public List<GameObject> SpawnedPrefabs { get; set; } = new List<GameObject>();
        public Dictionary<GameObject, Transform> PositionDictionary { get; } = new Dictionary<GameObject, Transform>();
        public List<string> LayerMasks { get; } = new List<string>();
        public Collider[] FoundCollidersForDeletion { get; set; }

        public float Radius { get; set; }
        public float MinDistanceBetweenPrefabs { get; set; }

        public PrefabSpawnerModel()
        {
            //populate list with all layer
            for (int i = 0; i < 32; i++)
            {
                string layerName = LayerMask.LayerToName(i);
                if (!string.IsNullOrEmpty(layerName))
                {
                    LayerMasks.Add(layerName);
                }
            }
        }
    }

    [Serializable]
    public struct PrefabSpawnerContainer
    {
        public List<GameObject> prefabChoiceLst;
        public List<GameObject> spawnedPrefabs;
        public string layer;
        public float radius;
        public float minDistanceBetweenPrefabs;
    }
}
