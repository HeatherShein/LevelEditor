using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NoiseMapGeneration;
using static TileGeneration;
using static LevelGeneration;

public class TreeGeneration : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private NoiseMapGeneration noiseMapGeneration;

    [SerializeField]
    private Wave[] waves;

    [SerializeField]
    public float mapScale;

    [SerializeField]
    private float[] neighborRadius;

    [SerializeField]
    private GameObject[] treePrefab;

    [SerializeField]
    private float treeHeightOffset;

    [SerializeField]
    private GameObject treeContainer;

    [SerializeField]
    private Vector3 localScale;
    #endregion

    #region Setters
    public void SetLocalScale(Vector3 size) { localScale = size; }

    public void SetNeighborRadius(float[] neighbor) 
    {
        for (int i = 0; i < neighbor.Length; i++)
        {
            this.neighborRadius[i] = neighbor[i];
        }
    }
    #endregion
    public void GenerateTrees(int mapDepth, int mapWidth, float mapScale, float distanceBetweenVertices, LevelData levelData)
    {
        // Generate a tree noise using Perlin noise
        float[,] treeMap = this.noiseMapGeneration.GeneratePerlinNoiseMap(mapDepth, mapWidth, mapScale, 0, 0, this.waves);

        for (int zIndex = 0; zIndex < mapDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < mapWidth; xIndex++)
            {
                // Retrieve Tile Data corresponding to this Tile Coordinate System
                TileCoordinate tileCoordinate = levelData.ConvertToTileCoordinate(zIndex, xIndex);
                TileData tileData = levelData.tilesData[tileCoordinate.tileZIndex, tileCoordinate.tileXIndex];
                int tileWidth = tileData.heightMap.GetLength(1);

                // Calculate the mesh vertex index
                Vector3[] meshVertices = tileData.mesh.vertices;
                int vertexIndex = tileCoordinate.coordinateZIndex * tileWidth + tileCoordinate.coordinateXIndex;

                // Get terrain type of this coordinate
                TerrainType terrainType = tileData.chosenHeightTerrainTypes[tileCoordinate.coordinateZIndex, tileCoordinate.coordinateXIndex];

                // Get biome of this coordinate
                Biome biome = tileData.chosenBiomes[tileCoordinate.coordinateZIndex, tileCoordinate.coordinateXIndex];

                // Prevent placing on water
                if(terrainType.name != "Water")
                {
                    float treeValue = treeMap[zIndex, xIndex];

                    int terrainTypeIndex = terrainType.index;

                    // Compare current tree noise value to neighbor ones
                    int neighborZBegin = (int)Mathf.Max(0, zIndex - this.neighborRadius[biome.index]);
                    int neighborZEnd = (int)Mathf.Min(mapDepth - 1, zIndex + this.neighborRadius[biome.index]);
                    int neighborXBegin = (int)Mathf.Max(0, xIndex - this.neighborRadius[biome.index]);
                    int neighborXEnd = (int)Mathf.Min(mapWidth - 1, xIndex + this.neighborRadius[biome.index]);

                    float maxValue = 0f;
                    
                    for (int neighborZ = neighborZBegin; neighborZ < neighborZEnd; neighborZ++)
                    {
                        for (int neighborX = neighborXBegin; neighborX < neighborXEnd; neighborX++)
                        {
                            float neighborValue = treeMap[neighborZ, neighborX];
                            // Keep the maximum tree noise value in radius
                            if (neighborValue >= maxValue)
                            {
                                maxValue = neighborValue;
                            }
                        }
                    }

                    // If current value is the max, place a tree at this location
                    if (treeValue == maxValue)
                    {
                        Debug.Log("On passe ici");
                        // Instantiating with an offset of the tile, because trees are meant to be spawned at the center of the tile
                        Vector3 treePosition = new Vector3(xIndex * distanceBetweenVertices - (tileWidth/2), meshVertices[vertexIndex].y + this.treeHeightOffset, zIndex * distanceBetweenVertices - (tileWidth/2));
                        GameObject tree = Instantiate(this.treePrefab[biome.index], treePosition, Quaternion.identity) as GameObject;
                        tree.transform.localScale = this.localScale;
                        tree.transform.parent = treeContainer.transform; // Clean the project explorer
                    }
                }
            }
        }


    }
}
