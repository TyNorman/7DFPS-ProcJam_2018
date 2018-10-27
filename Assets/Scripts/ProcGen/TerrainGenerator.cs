using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TerrainGenerator : MonoBehaviour
{
    private const float EDGE_OFFSET = 2.0f; //Edge offset to ignore when spawning props (plants, etc.)

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

    public int Seed
    {
        get { return seed; }
        set { seed = value; }
    }

    public bool AutoUpdate
    {
        get { return autoUpdate; }
    }

    // Use this for initialization
    void Start ()
    {
        autoUpdate = false;
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


        if (autoUpdate == false)
            GeneratePlants(800);
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

    private Biome GetBiomeByID(Terrain.TerrainID id)
    {
        Biome returnBiome = new Biome();

        for (int i = 0; i < biomes.Length; i++)
        {
            if (biomes[i].terrainType == id)
            {
                returnBiome = biomes[i];
                break;
            }
        }
        return returnBiome;
    }

    private void GeneratePlants(int numToGenerate)
    {
        float meshWidth = terrainMap.TerrainMesh.bounds.extents.x * terrainMap.MeshObject.transform.localScale.x;
        float meshDepth = terrainMap.TerrainMesh.bounds.extents.z * terrainMap.MeshObject.transform.localScale.z;

        for (int i = 0; i < numToGenerate; i++)
        {
            GameObject newPlant = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Plants/Plant_Obj"));

            Vector3 newPos = RandomizePosition();

            newPlant.transform.SetParent(gameObject.transform);
            newPlant.transform.position = newPos;
        }
    }

    private Vector3 RandomizePosition()
    {
        Vector3 returnPos = new Vector3();

        float meshWidth = (terrainMap.TerrainMesh.bounds.extents.x - EDGE_OFFSET) * terrainMap.MeshObject.transform.localScale.x;
        float meshDepth = (terrainMap.TerrainMesh.bounds.extents.z - EDGE_OFFSET) * terrainMap.MeshObject.transform.localScale.z;

        returnPos.x = Random.Range(-meshWidth, meshWidth);
        returnPos.z = Random.Range(-meshDepth, meshDepth);

        //Use a raycast to determine the height
        Vector3 rayPos = new Vector3(returnPos.x, 100, returnPos.z);
        RaycastHit hit;
        Ray ray = new Ray(rayPos, Vector3.down);

        if (Physics.Raycast(ray, out hit))
            returnPos.y = hit.point.y;

        //Prevent plants from spawning on sand/water
        if (returnPos.y <= GetBiomeByID(Terrain.TerrainID.Sand).height)
        {
            returnPos = RandomizePosition();
        }

        return returnPos;
    }
}
