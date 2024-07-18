using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fabian.EngineTool.PrefabSpawner
{
    public class PrefabSpawnerModel
    {
        public List<GameObject> PrefabChoiceLst { get; set; } = new();
        public List<GameObject> SpawnedPrefabs { get; set; } = new();
        public Dictionary<GameObject, Transform> PositionDictionary { get; set; } = new();
        public List<string> LayerMasks { get; set; } = new();
        public Collider[] FoundCollidersForDeletion { get; set; }
        
        public float Radius { get; set; }
        public float MinDistanceBetweenPrefabs { get; set; }

        public PrefabSpawnerModel()
        {
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
}
