using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TerrainType
{
    public string name;
    public float threshold;
    public Color color;
}

public class TileGeneration : MonoBehaviour
{

    [SerializeField]
    private TerrainType[] heightTerrainTypes;

    [SerializeField]
    private TerrainType[] heatTerrainTypes;

    [SerializeField]
    private NoiseMapGeneration.Wave[] heightWaves;

    [SerializeField]
    private NoiseMapGeneration.Wave[] heatWaves;

    [SerializeField]
    NoiseMapGeneration noiseMapGeneration;

    [SerializeField]
    private MeshRenderer tileRenderer;

    [SerializeField]
    private MeshFilter meshFilter;

    [SerializeField]
    private MeshCollider meshCollider;

    [SerializeField]
    private float mapScale;

    [SerializeField]
    private float heightMultiplier;

    [SerializeField]
    private AnimationCurve heightCurve;

    [SerializeField]
    private AnimationCurve heatCurve;

    [SerializeField]
    private VisualizationMode visualizationMode;

    public void GenerateTile(float centerVertexZ, float maxDistanceZ)
    {
        // Calculate tile depth and width based on the mesh vertices
        Vector3[] meshVertices = this.meshFilter.mesh.vertices;
        int tileDepth = (int)Mathf.Sqrt(meshVertices.Length);
        int tileWidth = tileDepth;

        // Calculate the offset based on the tile position
        float offsetX = -this.gameObject.transform.position.x;
        float offsetZ = -this.gameObject.transform.position.z;

        // Generate a height map using perlin noise
        float[,] heightMap = this.noiseMapGeneration.GeneratePerlinNoiseMap(tileDepth, tileWidth, this.mapScale, offsetX, offsetZ, this.heightWaves);

        // Calculate vertex offset based on Tile position and the distance between vertices
        Vector3 tileDimensions = this.meshFilter.mesh.bounds.size;
        float distanceBetweenVertices = tileDimensions.z / (float)tileDepth;
        float vertexOffsetZ = this.gameObject.transform.position.z / distanceBetweenVertices;

        // Generate a heat map using uniform noise
        float[,] uniformHeatMap = this.noiseMapGeneration.GenerateUniformNoiseMap(tileDepth, tileWidth, centerVertexZ, maxDistanceZ, vertexOffsetZ);

        // Generate a heat map using perlin noise

        float[,] perlinHeatMap = this.noiseMapGeneration.GeneratePerlinNoiseMap(tileDepth,tileWidth,this.mapScale,offsetX,offsetZ,this.heatWaves);

        // Proper heat map

        float[,] heatMap = new float[tileDepth, tileWidth];

        for (int zIndex = 0; zIndex < tileDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++)
            {
                // Mix both heat maps together by multiplying their values
                heatMap[zIndex, xIndex] = uniformHeatMap[zIndex, xIndex] * perlinHeatMap[zIndex, xIndex];
                // Adding height value to heat map to make higher region colder
                heatMap[zIndex, xIndex] += this.heatCurve.Evaluate(heightMap[zIndex, xIndex]) * heightMap[zIndex, xIndex];
            }
        }

        // Build a 2D texture from the height map
        Texture2D heightTexture = BuildTexture(heightMap, this.heightTerrainTypes);

        // Build a 2D texture from the heat map
         Texture2D heatTexture = BuildTexture(heatMap, this.heatTerrainTypes);

        switch (this.visualizationMode)
        {
            // Assign material texture to be the heightTexture
            case VisualizationMode.Height:
                this.tileRenderer.material.mainTexture = heightTexture;
                break;
            // Assign material texture to be the heatTexture
            case VisualizationMode.Heat:
                this.tileRenderer.material.mainTexture = heatTexture;
                break;
        }
        
        UpdateMeshVertices(heightMap);
    }

    private Texture2D BuildTexture(float[,] map, TerrainType[] terrainTypes)
    {
        int tileDepth = map.GetLength(0);
        int tileWidth = map.GetLength(1);

        Color[] colorMap = new Color[tileDepth * tileWidth];
        
        for (int zIndex = 0; zIndex < tileDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++)
            {
                // Transform the 2D Map index in an Array index
                int colorIndex = zIndex * tileWidth + xIndex;
                float noise = map[zIndex, xIndex];

                // Choose a terrain type according to its height value
                TerrainType terrainType = ChooseTerrainType(noise, terrainTypes);

                // Assign as color a shade of grey proportional to the height value
                colorMap[colorIndex] = terrainType.color;
            }
        }
        // Create a new texture and set its pixel colors
        Texture2D tileTexture = new Texture2D(tileWidth, tileDepth);
        tileTexture.wrapMode = TextureWrapMode.Clamp;
        tileTexture.SetPixels(colorMap);
        tileTexture.Apply();

        return tileTexture;
    }

    TerrainType ChooseTerrainType(float noise, TerrainType[] terrainTypes)
    {
        // For each terrain type, check if the height is lower than the other terrain type
        foreach(TerrainType terrainType in terrainTypes)
        {
            if (noise < terrainType.threshold)
            {
                // Return the first one
                return terrainType;
            }
        }
        return terrainTypes[terrainTypes.Length - 1];
    }

    private void UpdateMeshVertices(float[,] heightMap)
    {
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);

        Vector3[] meshVertices = this.meshFilter.mesh.vertices;

        // Iterate through all the heightMap coordinates, updating the vertex index
        int vertexIndex = 0;
        for (int zIndex = 0; zIndex < tileDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++)
            {
                float height = heightMap[zIndex, xIndex];
                Vector3 vertex = meshVertices[vertexIndex];

                // Change the vertex Y coordinate, proportional to the height value
                meshVertices[vertexIndex] = new Vector3(vertex.x, this.heightCurve.Evaluate(height) * this.heightMultiplier, vertex.z);
                vertexIndex++;
            }
        }

        // Update the vertices in the mesh and update its properties
        this.meshFilter.mesh.vertices = meshVertices;
        this.meshFilter.mesh.RecalculateBounds();
        this.meshFilter.mesh.RecalculateNormals();
        // Update the mesh collider
        this.meshCollider.sharedMesh = this.meshFilter.mesh;
    }

    enum VisualizationMode
    {
        Height, Heat
    }
}
