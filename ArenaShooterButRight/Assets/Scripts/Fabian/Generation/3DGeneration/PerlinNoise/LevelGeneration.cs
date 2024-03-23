using Unity.Mathematics;
using UnityEngine;

namespace Fabian.Generation._3DGeneration.PerlinNoise
{
    public class LevelGeneration : MonoBehaviour
    {
        [SerializeField] private int mapWidthInTiles;
        [SerializeField] private int mapDepthInTiles;
        [SerializeField] private GameObject tilePrefab;

        private void Start()
        {
            GenerateMap();
        }

        private void GenerateMap()
        {
            Vector3 tileSize = tilePrefab.GetComponent<MeshRenderer>().bounds.size;
            int tileWidth = (int)tileSize.x;
            int tileDepth = (int)tileSize.z;

            for (int xTileIndex = 0; xTileIndex < mapWidthInTiles; xTileIndex++)
            {
                for (int zTileIndex = 0; zTileIndex < mapDepthInTiles; zTileIndex++)
                {
                    Vector3 tilePosition = new Vector3(this.transform.position.x + xTileIndex * tileWidth, this.transform.position.y, this.transform.position.z + zTileIndex * tileDepth);
                    
                    GameObject tile = Instantiate(tilePrefab, tilePosition, quaternion.identity);
                }
            }
        }
    }
}
