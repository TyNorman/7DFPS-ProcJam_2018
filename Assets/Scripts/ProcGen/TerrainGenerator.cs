using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TerrainGenerator : MonoBehaviour
{
    public enum RenderMode { Noise, Colour, Mesh };

    [Header("Map Properties")]
    [SerializeField] private RenderMode renderMode = RenderMode.Noise;
    [SerializeField] private Biome[] biomes;
    [SerializeField] private int mapWidth;
    [SerializeField] private int mapHeight;
    [SerializeField] private float noiseScale;
    [SerializeField] private float heightMultiplier;
    [SerializeField] private bool autoUpdate;

    [Header("Perlin Noise Properties")]
    [SerializeField] private int octaves;

    [Range(0, 1)]
    [SerializeField] private float persistance;

    [SerializeField] private float lacunarity;
    [SerializeField] private int seed;
    [SerializeField] private Vector2 offset;

    [Header("Falloff Map Properties")]
    [SerializeField] private bool useFalloff;

    [Header("Map Rendering")]
    [SerializeField] private TerrainMapRenderer terrainMap;

    private float[,] falloffMap;

    public bool AutoUpdate
    {
        get { return autoUpdate; }
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void GenerateMap()
    {
        int minSize = Mathf.Min(mapWidth, mapHeight); //Get the smaller size for the FalloffGenerator

        float[,] noiseMap = PerlinNoise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);
        Color[] colourMap = new Color[mapWidth * mapHeight];
        falloffMap = FalloffGenerator.GenerateFalloffMap(minSize);

        //Loop through the map and apply heights
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (useFalloff) //If using Falloff to create islands, subtract the falloff map and clamp between 0-1 values
                {
                    noiseMap[x, y] = Mathf.Clamp(noiseMap[x, y] - falloffMap[x, y], 0, 1);
                }

                float height = noiseMap[x, y];

                //Loop through the Biomes
                for (int i = 0; i < biomes.Length; i++)
                {
                    if (height <= biomes[i].height)
                    {
                        colourMap[y * mapWidth + x] = biomes[i].colour;
                        break;
                    }
                }
            }
        }

        if (renderMode == RenderMode.Noise)
            terrainMap.RenderMap(TextureMaker.TextureFromHeightMap(noiseMap));
        else if (renderMode == RenderMode.Colour)
            terrainMap.RenderMap(TextureMaker.TextureFromColourMap(colourMap, mapWidth, mapHeight));
        else if (renderMode == RenderMode.Mesh)
            terrainMap.RenderMesh(TerrainMeshGenerator.GenerateTerrainMesh(noiseMap, heightMultiplier), TextureMaker.TextureFromColourMap(colourMap, mapWidth, mapHeight));

    }

    private void OnValidate()
    {
        //Reset any Editor values that would not process properly
        if (mapWidth < 1)
            mapWidth = 1;
        if (mapHeight < 1)
            mapHeight = 1;
        if (lacunarity < 1)
            lacunarity = 1;
        if (octaves < 0)
            octaves = 0;
    }
}
