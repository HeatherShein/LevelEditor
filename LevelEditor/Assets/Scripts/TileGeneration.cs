using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TerrainType
{
    public string name;
    public float height;
    public Color color;
}

public class TileGeneration : MonoBehaviour
{

    [SerializeField]
    private TerrainType[] terrainTypes;

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

    // Start is called before the first frame update
    void Start()
    {
        GenerateTile();
    }

    void GenerateTile()
    {
        // Calculate tile depth and width based on the mesh vertices
        Vector3[] meshVertices = this.meshFilter.mesh.vertices;
        int tileDepth = (int)Mathf.Sqrt(meshVertices.Length);
        int tileWidth = tileDepth;

        // Calculate the offset based on the tile position
        float[,] heightMap = this.noiseMapGeneration.GenerateNoiseMap(tileDepth,tileWidth,this.mapScale);

        // Generate a height map using noise
        Texture2D tileTexture = BuildTexture(heightMap);

        this.tileRenderer.material.mainTexture = tileTexture;
    }

    private Texture2D BuildTexture(float[,] heightMap)
    {
        int tileDepth = heightMap.GetLength(0);
        int tileWidth = heightMap.GetLength(1);

        Color[] colorMap = new Color[tileDepth * tileWidth];
        
        for (int zIndex = 0; zIndex < tileDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < tileWidth; xIndex++)
            {
                // Transform the 2D Map index in an Array index
                int colorIndex = zIndex * tileWidth + xIndex;
                float height = heightMap[zIndex, xIndex];

                // Choose a terrain type according to its height value
                TerrainType terrainType = ChooseTerrainType(height);

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

    TerrainType ChooseTerrainType(float height)
    {
        // For each terrain type, check if the height is lower than the other terrain type
        foreach(TerrainType terrainType in terrainTypes)
        {
            if (height < terrainType.height)
            {
                // Return the first one
                return terrainType;
            }
        }
        return terrainTypes[terrainTypes.Length - 1];
    }
}
