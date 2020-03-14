using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{

    #region Variables

    [SerializeField]
    private int mapWidthInTiles, mapDepthInTiles;

    [SerializeField]
    private GameObject tilePrefab;

    [SerializeField]
    private float centerVertexZ, maxDistanceZ;

    [SerializeField]
    private TreeGeneration treeGeneration;

    [SerializeField]
    private RiverGeneration riverGeneration;

    [SerializeField]
    private bool containsTrees;

    [SerializeField]
    private bool containsRivers;
    #endregion

    #region Classes
    public class LevelData
    {
        private int tileDepthInVertices, tileWidthInVertices;

        public TileGeneration.TileData[,] tilesData;

        public LevelData(int tileDepthInVertices, int tileWidthInVertices, int mapDepthInTiles, int mapWidthInTiles)
        {
            tilesData = new TileGeneration.TileData[tileDepthInVertices * mapDepthInTiles, tileWidthInVertices * mapWidthInTiles];

            this.tileDepthInVertices = tileDepthInVertices;
            this.tileWidthInVertices = tileWidthInVertices;
        }

        public void AddTileData(TileGeneration.TileData tileData, int tileZIndex, int tileXIndex)
        {
            this.tilesData[tileZIndex, tileXIndex] = tileData;
        }

        public TileCoordinate ConvertToTileCoordinate(int zIndex, int xIndex)
        {
            // Tile is calculated by dividing the index by the number of tiles in that axis
            int tileZIndex = (int)Mathf.Floor((float)zIndex / (float)this.tileDepthInVertices);
            int tileXIndex = (int)Mathf.Floor((float)xIndex / (float)this.tileWidthInVertices);

            // Coordinate index is calculated by getting the remainder of the division above plus translating the origin to the bottom left corner
            int coordinateZIndex = this.tileDepthInVertices - (zIndex % this.tileDepthInVertices) - 1;
            int coordinateXIndex = this.tileWidthInVertices - (xIndex % this.tileWidthInVertices) - 1;

            TileCoordinate tileCoordinate = new TileCoordinate(tileZIndex, tileXIndex, coordinateZIndex, coordinateXIndex);
            return tileCoordinate;
        }
    }

    public class TileCoordinate
    {
        public int tileZIndex;
        public int tileXIndex;
        public int coordinateZIndex;
        public int coordinateXIndex;

        public TileCoordinate(int tileZIndex, int tileXIndex, int coordinateZIndex, int coordinateXIndex)
        {
            this.tileZIndex = tileZIndex;
            this.tileXIndex = tileXIndex;
            this.coordinateZIndex = coordinateZIndex;
            this.coordinateXIndex = coordinateXIndex;
        }
    }
    #endregion


    public void SetContainsRivers(bool val)
    {
        containsRivers = val;
    }

    public void SetSize(int width, int depth)
    {
        mapWidthInTiles = width;
        mapDepthInTiles = depth;
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        // Get the tile dimension the tile prefab
        Vector3 tileSize = tilePrefab.GetComponent<MeshRenderer>().bounds.size;
        int tileWidth = (int)tileSize.x;
        int tileDepth = (int)tileSize.z;

        // Calculate the number of vertices of the tile in each axis using its mesh
        Vector3[] tileMeshVertices = tilePrefab.GetComponent<MeshFilter>().sharedMesh.vertices;
        int tileDepthInVertices = (int)Mathf.Sqrt(tileMeshVertices.Length);
        int tileWidthInVertices = tileDepthInVertices;

        float distanceBetweenVertices = (float)tileDepth / (float)tileDepthInVertices;

        // Build an empty level data object
        LevelData levelData = new LevelData(tileDepthInVertices, tileWidthInVertices, this.mapDepthInTiles, this.mapWidthInTiles);

        // For each tile, instantiate a tile in the correct position
        for (int xTileIndex = 0; xTileIndex < mapWidthInTiles; xTileIndex++)
        {
            for (int zTileIndex = 0; zTileIndex < mapDepthInTiles; zTileIndex++)
            {
                // Calculate the tile position based on the X and the Z indices
                Vector3 tilePosition = new Vector3(this.gameObject.transform.position.x + xTileIndex * tileWidth,
                    this.gameObject.transform.position.y,
                    this.gameObject.transform.position.z + zTileIndex * tileDepth);
                // Instantiate a new tile
                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as GameObject;
                // Generate tile texture
                TileGeneration.TileData tileData = tile.GetComponent<TileGeneration>().GenerateTile(this.centerVertexZ, this.maxDistanceZ);
                levelData.AddTileData(tileData, zTileIndex, xTileIndex);
            }
        }

        // Generate trees for the level
        if (this.containsTrees)
        {
            this.treeGeneration.GenerateTrees(
            this.mapDepthInTiles * tileDepthInVertices,
            this.mapWidthInTiles * tileWidthInVertices,
            distanceBetweenVertices,
            this.treeGeneration.mapScale,
            levelData);
        }

        // Generate rivers for the level
        if (this.containsRivers)
        {
            this.riverGeneration.GenerateRivers(
            this.mapDepthInTiles * tileDepthInVertices,
            this.mapWidthInTiles * tileWidthInVertices,
            levelData);
        }
    }
}
