using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMapGeneration : MonoBehaviour
{

    [System.Serializable]
    public class Wave
    {
        public float seed;
        public float frequency;
        public float amplitude;
    }

    public float[,] GeneratePerlinNoiseMap(int mapDepth, int mapWidth, float scale, float offsetX, float offsetZ, Wave[] waves)
    {
        // Create an empty noise map with the mapDepth and mapWidth coordinates
        float[,] noiseMap = new float[mapDepth, mapWidth];

        for (int zIndex = 0; zIndex < mapDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < mapWidth; xIndex++)
            {
                // Calculate sample indices based on the coordinates and the scale
                float sampleX = (xIndex+offsetX) / scale; // Add offsets to allow continuous generation for multiple tiles
                float sampleZ = (zIndex+offsetZ) / scale;

                float noise = 0f;
                float normalization = 0f;

                foreach (Wave wave in waves)
                {
                    // Generate noise value using PerlinNoise for a given Wave
                    noise += wave.amplitude * Mathf.PerlinNoise(sampleX * wave.frequency + wave.seed, sampleZ * wave.frequency + wave.seed);
                    normalization += wave.amplitude;
                }
                // Normalize noise value
                noise /= normalization;

                noiseMap[zIndex, xIndex] = noise;
            }
        }
        return noiseMap;
    }

    public float[,] GenerateUniformNoiseMap(int mapDepth, int mapWidth, float centerVertexZ, float maxDistanceZ, float offsetZ)
    {
        // Create an empty noise map with the mapDepth and mapWidth coordinates
        float[,] noiseMap = new float[mapDepth, mapWidth];

        for (int zIndex = 0; zIndex < mapDepth; zIndex++)
        {
            // Calculate the sample by summing the index and the offset
            float sampleZ = zIndex + offsetZ;
            // Calculate the noise proportional to the distance of the sample to the center of the level
            float noise = Mathf.Abs(sampleZ - centerVertexZ) / maxDistanceZ;
            // Apply the noise for all point with this Z coordinate
            for (int xIndex = 0; xIndex < mapWidth; xIndex++)
            {
                noiseMap[mapDepth - zIndex - 1, xIndex] = noise;
            }
        }
        return noiseMap;
    }
}
