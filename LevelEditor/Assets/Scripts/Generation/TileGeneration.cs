using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGeneration : MonoBehaviour
{
    #region Variables
    [SerializeField]
    public float mapScale;

    [SerializeField]
    private float heightMultiplier;

    [SerializeField]
    private VisualizationMode visualizationMode;

    [SerializeField]
    private Color waterColor;

    public void setMapScale(float n) { mapScale = n; }
    public void setWaterColor(Color color) { waterColor = color; }
    #endregion

    #region Height
    [SerializeField]
    private TerrainType[] heightTerrainTypes;

    [SerializeField]
    private NoiseMapGeneration.Wave[] heightWaves;

    [SerializeField]
    private AnimationCurve heightCurve;
    #endregion

    #region Heat
    [SerializeField]
    private TerrainType[] heatTerrainTypes;

    [SerializeField]
    private NoiseMapGeneration.Wave[] heatWaves;

    [SerializeField]
    private AnimationCurve heatCurve;
    #endregion

    #region Moisture
    [SerializeField]
    private TerrainType[] moistureTerrainTypes;

    [SerializeField]
    private NoiseMapGeneration.Wave[] moistureWaves;

    [SerializeField]
    private AnimationCurve moistureCurve;
    #endregion 

    #region Biomes
    [SerializeField]
    private BiomeRow[] biomes;

    [System.Serializable]
    public class Biome
    {
        public string name;
        public Color color;
        public int index;
    }

    [System.Serializable]
    public class BiomeRow
    {
        public Biome[] biomes;
    }
    #endregion

    #region Constants
    [SerializeField]
    NoiseMapGeneration noiseMapGeneration;
    
    [SerializeField]
    private MeshRenderer tileRenderer;

    [SerializeField]
    private MeshFilter meshFilter;

    [SerializeField]
    private MeshCollider meshCollider;

    [System.Serializable]
    public class TerrainType
    {
        public string name;
        public float threshold;
        public Color color;
        public int index;
    }

    enum VisualizationMode
    {
        Height, Heat, Moisture, Biome
    }

    public class TileData
    {
        public float[,] heightMap;
        public float[,] heatMap;
        public float[,] moistureMap;
        public TerrainType[,] chosenHeightTerrainTypes;
        public TerrainType[,] chosenHeatTerrainTypes;
        public TerrainType[,] chosenMoistureTerrainTypes;
        public Biome[,] chosenBiomes;
        public Mesh mesh;
        public Texture2D texture;

        public TileData(float[,] heightMap, float[,] heatMap, float[,] moistureMap,
            TerrainType[,] chosenHeightTerrainTypes, TerrainType[,] chosenHeatTerrainTypes,
            TerrainType[,] chosenMoistureTerrainTypes, Biome[,] chosenBiomes,
            Mesh mesh, Texture2D texture)
        {
            this.heightMap = heightMap;
            this.heatMap = heatMap;
            this.moistureMap = moistureMap;
            this.chosenHeightTerrainTypes = chosenHeightTerrainTypes;
            this.chosenHeatTerrainTypes = chosenHeatTerrainTypes;
            this.chosenMoistureTerrainTypes = chosenMoistureTerrainTypes;
            this.chosenBiomes = chosenBiomes;
            this.mesh = mesh;
            this.texture = texture;
        }
    }
    #endregion

    public TileData GenerateTile(float centerVertexZ, float maxDistanceZ)
    {
        // Calculate tile depth and width based on the mesh vertices
        Vector3[] meshVertices = this.meshFilter.mesh.vertices;
        int tileDepth = (int)Mathf.Sqrt(meshVertices.Length);
        int tileWidth = tileDepth;

        #region HeightMap
        // Calculate the offset based on the tile position
        float offsetX = -this.gameObject.transform.position.x;
        float offsetZ = -this.gameObject.transform.position.z;

        // Generate a height map using perlin noise
        float[,] heightMap = this.noiseMapGeneration.GeneratePerlinNoiseMap(tileDepth, tileWidth, this.mapScale, offsetX, offsetZ, this.heightWaves);
        #endregion

        #region HeatMap
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
        #endregion

        #region MoistureMap
        // Generate moisture map using perlin noise
        float[,] moistureMap = this.noiseMapGeneration.GeneratePerlinNoiseMap(tileDepth, tileWidth, this.mapScale, offsetX, offsetZ, this.moistureWaves);
        for (int zIndex = 0; zIndex < tileDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++)
            {
                // Reducing height value from the heat map makes higher region dryer
                moistureMap[zIndex, xIndex] -= this.moistureCurve.Evaluate(heightMap[zIndex, xIndex]) * heightMap[zIndex, xIndex];
            }
        }
        #endregion

        #region Textures
        // Build a 2D texture from the height map
        TerrainType[,] chosenHeightTerrainTypes = new TerrainType[tileDepth, tileWidth];
        Texture2D heightTexture = BuildTexture(heightMap, this.heightTerrainTypes, chosenHeightTerrainTypes);

        // Build a 2D texture from the heat map
        TerrainType[,] chosenHeatTerrainTypes = new TerrainType[tileDepth, tileWidth];
        Texture2D heatTexture = BuildTexture(heatMap, this.heatTerrainTypes, chosenHeatTerrainTypes);

        // Build a 2D texture from the moisture map
        TerrainType[,] chosenMoistureTerrainTypes = new TerrainType[tileDepth, tileWidth];
        Texture2D moistureTexture = BuildTexture(moistureMap, this.moistureTerrainTypes, chosenMoistureTerrainTypes);

        // Build a biomes 2D texture from the three other noise variables
        Biome[,] chosenBiomes = new Biome[tileDepth, tileWidth];
        Texture2D biomeTexture = BuildBiomeTexture(chosenHeightTerrainTypes, chosenHeatTerrainTypes, chosenMoistureTerrainTypes, chosenBiomes);

        // Texture to pass into the TileData constructor
        Texture2D texture = new Texture2D(tileWidth,tileDepth);
        #endregion

        switch (this.visualizationMode)
        {
            // Assign material texture to be the heightTexture
            case VisualizationMode.Height:
                this.tileRenderer.material.mainTexture = heightTexture;
                texture = heightTexture;
                break;
            // Assign material texture to be the heatTexture
            case VisualizationMode.Heat:
                this.tileRenderer.material.mainTexture = heatTexture;
                texture = heatTexture;
                break;
            case VisualizationMode.Moisture:
                this.tileRenderer.material.mainTexture = moistureTexture;
                texture = moistureTexture;
                break;
            case VisualizationMode.Biome:
                this.tileRenderer.material.mainTexture = biomeTexture;
                texture = biomeTexture;
                break;
        }
        UpdateMeshVertices(heightMap);

        TileData tileData = new TileData(heightMap,heatMap,moistureMap,chosenHeightTerrainTypes,chosenHeatTerrainTypes,chosenMoistureTerrainTypes,chosenBiomes,this.meshFilter.mesh,texture);

        return tileData;
    }

    private Texture2D BuildTexture(float[,] map, TerrainType[] terrainTypes, TerrainType[,] chosenTerrainTypes)
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

                // Save the chosen terrain type
                chosenTerrainTypes[zIndex, xIndex] = terrainType;
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

    private Texture2D BuildBiomeTexture(TerrainType[,] heightTerrainTypes, TerrainType[,] heatTerrainTypes, TerrainType[,] moistureTerrainTypes, Biome[,] chosenBiomes)
    {
        int tileDepth = heatTerrainTypes.GetLength(0);
        int tileWidth = heatTerrainTypes.GetLength(1);

        Color[] colorMap = new Color[tileDepth * tileWidth];

        for (int zIndex = 0; zIndex < tileDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++)
            {
                int colorIndex = zIndex * tileWidth + xIndex;

                TerrainType heightTerrainType = heightTerrainTypes[zIndex, xIndex];

                // Prevent Water region
                if (heightTerrainType.name != "Water")
                {
                    // Get terrain types from heat and moisture
                    TerrainType heatTerrainType = heatTerrainTypes[zIndex, xIndex];
                    TerrainType moistureTerrainType = moistureTerrainTypes[zIndex, xIndex];

                    // Use terrain type index to access the biome
                    Biome biome = this.biomes[moistureTerrainType.index].biomes[heatTerrainType.index];
                    // Assign color according to the biome
                    colorMap[colorIndex] = biome.color;

                    // Save biome
                    chosenBiomes[zIndex, xIndex] = biome;
                } else
                {
                    // No biome, just water color
                    colorMap[colorIndex] = this.waterColor;
                }
            }
        }

        // Create new texture and assign its pixel colors

        Texture2D tileTexture = new Texture2D(tileWidth, tileDepth);
        tileTexture.filterMode = FilterMode.Point;
        tileTexture.wrapMode = TextureWrapMode.Clamp;
        tileTexture.SetPixels(colorMap);
        tileTexture.Apply();
        
        return tileTexture;
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
}
