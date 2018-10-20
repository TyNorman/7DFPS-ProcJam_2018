using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PerlinNoise //Used to provide terrain generation with Perlin Noise
{
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float noiseScale)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        if (noiseScale <= 0)
            noiseScale = 0.001f;

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                float sampleX = x/noiseScale;
                float sampleY = y/noiseScale;

                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                noiseMap[x, y] = perlinValue;
            }
        }

        return noiseMap;
    }
}
