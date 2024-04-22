using System.Collections.Generic;
using UnityEngine;

namespace Fabian.Generation._3DGeneration.NoiseGen
{
    public class EndlessTerrain : MonoBehaviour
    {
        public const float MAXVIEWDISTANCE = 450;
        public Transform viewer;

        public static Vector2 ViewerPosition;
        private static MapGeneration _mapGeneration;
        private int _chunkSize;
        private int _chunksVisibleInViewDst;

        private Dictionary<Vector2, TerrainChunk> _terrainChunksDic = new Dictionary<Vector2, TerrainChunk>();
        private List<TerrainChunk> _terrainChunksVisibleLastUpdate = new List<TerrainChunk>();
        
        private void Start()
        {
            _mapGeneration = FindObjectOfType<MapGeneration>();
            _chunkSize = MapGeneration.MapChunkSize - 1;
            _chunksVisibleInViewDst = Mathf.RoundToInt(MAXVIEWDISTANCE / _chunkSize);
        }

        private void Update()
        {
            ViewerPosition = new Vector2(viewer.position.x, viewer.position.z);
            UpdateVisibleChunks();
        }

        private void UpdateVisibleChunks()
        {
        
            for (int i = 0; i < _terrainChunksVisibleLastUpdate.Count; i++)
            {
                _terrainChunksVisibleLastUpdate[i].SetVisible(false);
            }
            _terrainChunksVisibleLastUpdate.Clear();
        
            int currentChunkCoordX = Mathf.RoundToInt(ViewerPosition.x / _chunkSize);
            int currentChunkCoordY = Mathf.RoundToInt(ViewerPosition.y / _chunkSize);

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
                        _terrainChunksDic.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, _chunkSize, this.transform));
                    }
                }
            }

        }

        public class TerrainChunk
        {
            private GameObject _meshObject;
            private Vector2 _position;
            private Bounds _bounds;
            public TerrainChunk(Vector2 coord, int size, Transform parent)
            {
                _position = coord * size;
                _bounds = new Bounds(_position, Vector2.one * size);
                Vector3 positionV3 = new Vector3(_position.x, 0, _position.y);

                _meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
                _meshObject.transform.position = positionV3;
                _meshObject.transform.localScale = Vector3.one * size / 10f;
                _meshObject.layer = 3;
                _meshObject.transform.parent = parent;
                SetVisible(false);
                
                _mapGeneration.RequestMapData(OnMapDataReceived);
            }

            void OnMapDataReceived(MapData mapData)
            {
                Debug.Log("Data received");
            }

            public void UpdateTerrainChunk()
            {
                float viewerDistanceFromNearestEdge = Mathf.Sqrt(_bounds.SqrDistance(ViewerPosition));
                bool visible = viewerDistanceFromNearestEdge <= MAXVIEWDISTANCE;
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
    }
}
