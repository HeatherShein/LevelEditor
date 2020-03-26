using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NoiseMapGeneration;
using static TileGeneration;
using static LevelGeneration;

public class RockGeneration : MonoBehaviour
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
    private GameObject[] rockPrefab;

    [SerializeField]
    private float rockHeightOffset;

    [SerializeField]
    private GameObject rockContainer;
    #endregion

    public void SetMapScale(float size) { mapScale = size; }

    public void GenerateRocks(int mapDepth, int mapWidth, float mapScale, float distanceBetweenVertices, LevelData levelData)
    {
        // Generate a rock noise using Perlin noise
        float[,] rockMap = this.noiseMapGeneration.GeneratePerlinNoiseMap(mapDepth, mapWidth, mapScale, 0, 0, this.waves);

        /*
        float mapSizeZ = mapDepth * distanceBetweenVertices;
        float mapSizeX = mapWidth * distanceBetweenVertices;
        */
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
                if (terrainType.name != "Water")
                {
                    float rockValue = rockMap[zIndex, xIndex];

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
                            float neighborValue = rockMap[neighborZ, neighborX];
                            // Keep the maximum rock noise value in radius
                            if (neighborValue >= maxValue)
                            {
                                maxValue = neighborValue;
                            }
                        }
                    }

                    // Get a random offset
                    bool found = false;
                    int offsetX = 0;
                    int offsetZ = 0;
                    TileCoordinate tileCoordinateOffset;
                    TileData tileDataOffset;
                    int tileWidthOffset = 0;
                    Vector3[] meshVerticesOffset = tileData.mesh.vertices;
                    int vertexIndexOffset = 0;
                    while (!found)
                    {
                        offsetX = Random.Range(0, 10);
                        offsetZ = Random.Range(0, 10);
                        if(xIndex + offsetX > 0 && xIndex + offsetX < mapWidth && zIndex + offsetZ > 0 && zIndex + offsetZ < mapDepth)
                        {
                            // Suitable offset
                            found = !found;

                            // Retrieve Tile Data corresponding to this Tile Coordinate System
                            tileCoordinateOffset = levelData.ConvertToTileCoordinate(zIndex + offsetZ, xIndex + offsetX);
                            tileDataOffset = levelData.tilesData[tileCoordinateOffset.tileZIndex, tileCoordinateOffset.tileXIndex];
                            tileWidthOffset = tileDataOffset.heightMap.GetLength(1);

                            // Calculate the mesh vertex index
                            meshVerticesOffset = tileDataOffset.mesh.vertices;
                            vertexIndexOffset = tileCoordinateOffset.coordinateZIndex * tileWidthOffset + tileCoordinateOffset.coordinateXIndex;
                        }
                    }

                    // If current value is the max, place a rock at this location
                    if (rockValue == maxValue)
                    {
                        // Instantiating with an offset of the tile, because rocks are meant to be spawned at the center of the tile
                        Vector3 rockPosition = new Vector3((xIndex + offsetX) * distanceBetweenVertices - (tileWidthOffset / 2), meshVerticesOffset[vertexIndexOffset].y + this.rockHeightOffset, (zIndex + offsetZ) * distanceBetweenVertices - (tileWidth / 2));
                        GameObject rock = Instantiate(this.rockPrefab[biome.index], rockPosition, Quaternion.identity) as GameObject;
                        rock.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                        rock.transform.parent = rockContainer.transform; // Clean the project explorer
                    }
                }
            }
        }


    }
}
