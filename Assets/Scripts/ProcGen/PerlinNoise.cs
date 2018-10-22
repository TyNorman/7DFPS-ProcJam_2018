using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PerlinNoise //Used to provide terrain generation with Perlin Noise
{
    private const int RAND_COORD_MINMAX = 500; //Randomized coords for Mathf.Perlin

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float noiseScale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random PRNG = new System.Random(seed); //Create a psuedo-random number generator using the seed
        Vector2[] octaveOffsets = new Vector2[octaves];

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = PRNG.Next(-RAND_COORD_MINMAX, RAND_COORD_MINMAX) + offset.x; //Used to give Mathf.Perlin a coordinate value
            float offsetY = PRNG.Next(-RAND_COORD_MINMAX, RAND_COORD_MINMAX) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (noiseScale <= 0)
            noiseScale = 0.001f;

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        //Using half Width/Height to scale from the center using noiseScale
        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth) / noiseScale * frequency + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) / noiseScale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance; //Amplitude decreases each octave (0-1 value)
                    frequency *= lacunarity; //Frequency increases each octave (>1 value)


                    //noiseMap[x, y] = perlinValue;
                }

                if (noiseHeight > maxNoiseHeight)
                    maxNoiseHeight = noiseHeight;
                else if (noiseHeight < minNoiseHeight)
                    minNoiseHeight = noiseHeight;

                noiseMap[x, y] = noiseHeight;
            }
        }

        //Perform a second pass loop to clamp the noiseMap to the min/max values
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]); //Returns 0, 0.5, and 1 values
            }
        }

        return noiseMap;
    }
}
