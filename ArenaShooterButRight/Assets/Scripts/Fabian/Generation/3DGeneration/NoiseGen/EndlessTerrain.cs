using System.Collections.Generic;
using Fabian.Generation._3DGeneration.MeshGen;
using UnityEngine;
using UnityEngine.Serialization;

namespace Fabian.Generation._3DGeneration.NoiseGen
{
    public class EndlessTerrain : MonoBehaviour
    {
        [SerializeField] private Transform viewer;
        [SerializeField] private Material mapMaterial;
        private static float _maxviewdistance = 450;
        public LODInfo[] detailLevels;
        private const float ViewerMoveThresholdForChunkUpdate = 25f;
        private const float SqrViewerMoveThresholdForChunkUpdate = ViewerMoveThresholdForChunkUpdate * ViewerMoveThresholdForChunkUpdate;

        
        private static Vector2 _viewerPosition;
        private Vector2 _viewerPositionOld;
        private static MapGeneration _mapGeneration;
        private int _chunkSize;
        private int _chunksVisibleInViewDst;

        private Dictionary<Vector2, TerrainChunk> _terrainChunksDic = new Dictionary<Vector2, TerrainChunk>();
        private List<TerrainChunk> _terrainChunksVisibleLastUpdate = new List<TerrainChunk>();
        
        private void Start()
        {
            _mapGeneration = FindObjectOfType<MapGeneration>();
            _maxviewdistance = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
            _chunkSize = MapGeneration.MapChunkSize - 1;
            _chunksVisibleInViewDst = Mathf.RoundToInt(_maxviewdistance / _chunkSize);
            UpdateVisibleChunks();
        }

        private void Update()
        {
            _viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
            if ((_viewerPositionOld - _viewerPosition).sqrMagnitude > SqrViewerMoveThresholdForChunkUpdate)
            {
                _viewerPositionOld = _viewerPosition;
                UpdateVisibleChunks();
            }
        }

        private void UpdateVisibleChunks()
        {
        
            for (int i = 0; i < _terrainChunksVisibleLastUpdate.Count; i++)
            {
                _terrainChunksVisibleLastUpdate[i].SetVisible(false);
            }
            _terrainChunksVisibleLastUpdate.Clear();
        
            int currentChunkCoordX = Mathf.RoundToInt(_viewerPosition.x / _chunkSize);
            int currentChunkCoordY = Mathf.RoundToInt(_viewerPosition.y / _chunkSize);

            for (int yOffset = -_chunksVisibleInViewDst; yOffset <= _chunksVisibleInViewDst; yOffset++)
            {
                for (int xOffset = -_chunksVisibleInViewDst; xOffset <= _chunksVisibleInViewDst; xOffset++)
                {
                    Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                    if (_terrainChunksDic.ContainsKey(viewedChunkCoord))
                    {
                        _terrainChunksDic[viewedChunkCoord].UpdateTerrainChunk();
                        if (_terrainChunksDic[viewedChunkCoord].IsVisible())
                        {
                            _terrainChunksVisibleLastUpdate.Add(_terrainChunksDic[viewedChunkCoord]);
                        }
                    }
                    else
                    {
                        _terrainChunksDic.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, _chunkSize, this.transform, mapMaterial, detailLevels));
                    }
                }
            }

        }

        public class TerrainChunk
        {
            private GameObject _meshObject;
            private Vector2 _position;
            private Bounds _bounds;
            private MeshRenderer _meshRenderer;
            private MeshFilter _meshFilter;
            private LODInfo[] _detailLevels;
            private LODMesh[] _lodMeshes;
            private FWMapData _fwMapData;
            private bool _mapDataReceived;
            private int _prevLodIndex = -1;
            public TerrainChunk(Vector2 coord, int size, Transform parent, Material material, LODInfo[] detailLevels)
            {
                _detailLevels = detailLevels;
                _position = coord * size;
                _bounds = new Bounds(_position, Vector2.one * size);
                Vector3 positionV3 = new Vector3(_position.x, 0, _position.y);

                _meshObject = new GameObject("Terrain Chunk");
                _meshRenderer = _meshObject.AddComponent<MeshRenderer>();
                _meshFilter = _meshObject.AddComponent<MeshFilter>();

                _meshRenderer.material = material;
                _meshObject.transform.position = positionV3;
                _meshObject.layer = 3;
                _meshObject.transform.parent = parent;
                SetVisible(false);

                _lodMeshes = new LODMesh[_detailLevels.Length];

                for (int i = 0; i < detailLevels.Length; i++)
                {
                    _lodMeshes[i] = new LODMesh(detailLevels[i].lod, UpdateTerrainChunk);
                }
                
                _mapGeneration.RequestMapData(_position, OnMapDataReceived);
            }

            void OnMapDataReceived(FWMapData fwMapData)
            {
                _fwMapData = fwMapData;
                _mapDataReceived = true;

                Texture2D texture = TextureGeneration.TextureFromColorMap(fwMapData.ColorMap, MapGeneration.MapChunkSize,
                    MapGeneration.MapChunkSize);
                _meshRenderer.material.mainTexture = texture;
                
                UpdateTerrainChunk();
            }
            
            public void UpdateTerrainChunk()
            {
                if (!_mapDataReceived) return;
                
                float viewerDistanceFromNearestEdge = Mathf.Sqrt(_bounds.SqrDistance(_viewerPosition));
                bool visible = viewerDistanceFromNearestEdge <= _maxviewdistance;
                if (visible)
                {
                    int lodIndex = 0;

                    for (int i = 0; i < _detailLevels.Length - 1; i++)
                    {
                        if (viewerDistanceFromNearestEdge > _detailLevels[i].visibleDstThreshold)
                        {
                            lodIndex = i + 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                    
                    if (lodIndex != _prevLodIndex)
                    {
                        LODMesh lodMesh = _lodMeshes[lodIndex];
                        if (lodMesh.HasMesh)
                        {
                            _prevLodIndex = lodIndex;
                            _meshFilter.mesh = lodMesh.Mesh;
                        }
                        else if(!lodMesh.HasRequestedMesh)
                        {
                            lodMesh.RequestMesh(_fwMapData);
                        }
                    }
                }
                
                SetVisible(visible);
            }

            public void SetVisible(bool visible)
            {
                _meshObject.SetActive(visible);
            }

            public bool IsVisible()
            {
                return _meshObject.activeSelf;
            }
        }

        class LODMesh
        {
            public Mesh Mesh;
            public bool HasRequestedMesh;
            public bool HasMesh;
            private readonly int _lod;
            private System.Action _updateCallback;

            public LODMesh(int lod, System.Action updateCallback)
            {
                _lod = lod;
                _updateCallback = updateCallback;
            }

            private void OnMeshDataReceived(FWMeshData fwMeshData)
            {
                Mesh = fwMeshData.CreateMesh();
                HasMesh = true;

                _updateCallback();
            }

            public void RequestMesh(FWMapData fwMapData)
            {
                HasRequestedMesh = true;
                _mapGeneration.RequestMeshData(fwMapData, _lod, OnMeshDataReceived);
            }
        }
        
        [System.Serializable]
        public struct LODInfo
        {
            public int lod;
            public float visibleDstThreshold;
        }
    }
    
}
