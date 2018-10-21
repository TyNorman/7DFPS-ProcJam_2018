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

    [Header("Map Rendering")]
    [SerializeField] private TerrainMapRenderer terrainMap;

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
        float[,] noiseMap = PerlinNoise.GenerateNoiseMap(mapWidth, mapHeight, noiseScale);
        Color[] colourMap = new Color[mapWidth * mapHeight];

        //Loop through the map and apply heights
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
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
}
