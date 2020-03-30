using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TileGeneration;
using static LevelGeneration;

public class RiverGeneration : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private int nbRivers;

    [SerializeField]
    private float heightThreshold;

    [SerializeField]
    private Color riverColor;
    #endregion

    #region Setters
    public void setNbRivers(int n) { nbRivers = n; }
    public void setRiverColor(Color color) { riverColor = color; }
    public void setHeightThreshold(float n) { heightThreshold = n; }
    #endregion

    public void GenerateRivers(int mapDepth, int mapWidth, LevelData levelData)
    {
        for (int riverIndex = 0; riverIndex < nbRivers; riverIndex++)
        {
            Vector3 riverOrigin = ChooseRiverOrigin(mapDepth, mapWidth, levelData);
            BuildRiver(mapDepth, mapWidth, riverOrigin, levelData);
        }
    }

    private Vector3 ChooseRiverOrigin(int mapDepth, int mapWidth, LevelData levelData)
    {
        bool found = false;
        int randomZIndex = 0;
        int randomXIndex = 0;

        while (!found)
        {
            // Generate random numbers
            randomZIndex = Random.Range(0, mapDepth);
            randomXIndex = Random.Range(0, mapWidth);

            // Get Tile Data from coordinates
            TileCoordinate tileCoordinate = levelData.ConvertToTileCoordinate(randomZIndex,randomXIndex);
            TileData tileData = levelData.tilesData[tileCoordinate.tileZIndex, tileCoordinate.tileXIndex];

            // Compare height values
            float heightValue = tileData.heightMap[tileCoordinate.coordinateZIndex, tileCoordinate.coordinateXIndex];
            if(heightValue >= this.heightThreshold)
            {
                found = true;
            }
        }
        return new Vector3(randomXIndex, 0, randomZIndex);
    }

    private void BuildRiver(int mapDepth, int mapWidth, Vector3 riverOrigin, LevelData levelData)
    {
        HashSet<Vector3> visitedCoordinates = new HashSet<Vector3>();

        Vector3 currentCoordinate = riverOrigin;

        bool foundWater = false;

        while (!foundWater)
        {
            // Retrieve Tile Data corresponding to coordinates
            TileCoordinate tileCoordinate = levelData.ConvertToTileCoordinate((int)currentCoordinate.z, (int)currentCoordinate.x);
            TileData tileData = levelData.tilesData[tileCoordinate.tileZIndex, tileCoordinate.tileXIndex];

            // Save current coordinate
            visitedCoordinates.Add(currentCoordinate);

            // Check if water is found
            if (tileData.chosenHeightTerrainTypes[tileCoordinate.coordinateZIndex,tileCoordinate.coordinateXIndex].name == "Water")
            {
                foundWater = true;
            }
            else
            {
                // Change texture of Tile data to show a river
                tileData.texture.SetPixel(tileCoordinate.coordinateXIndex, tileCoordinate.coordinateZIndex, this.riverColor);
                tileData.texture.Apply();

                // Pick neighbor coordinates if possible
                List<Vector3> neighbors = new List<Vector3>();
                if (currentCoordinate.z > 0)
                {
                    neighbors.Add(new Vector3(currentCoordinate.x, 0, currentCoordinate.z - 1));
                }
                if (currentCoordinate.z < mapDepth - 1)
                {
                    neighbors.Add(new Vector3(currentCoordinate.x, 0, currentCoordinate.z + 1));
                }
                if (currentCoordinate.x > 0)
                {
                    neighbors.Add(new Vector3(currentCoordinate.x - 1, 0, currentCoordinate.z));
                }
                if (currentCoordinate.x < mapWidth - 1)
                {
                    neighbors.Add(new Vector3(currentCoordinate.x + 1, 0, currentCoordinate.z));
                }

                // Pick minimum neighbor that has not been visited yet and flow to it
                float minHeight = float.MaxValue;
                Vector3 minNeighbor = new Vector3(0, 0, 0);
                foreach (Vector3 neighbor in neighbors)
                {
                    // Retrieve neighbor coordinates
                    TileCoordinate neighborTileCoordinate = levelData.ConvertToTileCoordinate((int)neighbor.z, (int)neighbor.x);
                    TileData neighborTileData = levelData.tilesData[neighborTileCoordinate.tileZIndex, neighborTileCoordinate.tileXIndex];

                    float neighborHeight = tileData.heightMap[neighborTileCoordinate.coordinateZIndex, neighborTileCoordinate.coordinateXIndex];
                    if (neighborHeight < minHeight && !visitedCoordinates.Contains(neighbor))
                    {
                        minHeight = neighborHeight;
                        minNeighbor = neighbor;
                    }
                }
                currentCoordinate = minNeighbor;
            }
        }
    }
}
