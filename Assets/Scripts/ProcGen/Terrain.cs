using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Biome
{
    public Terrain.TerrainID terrainType;
    public float height;
    public Color colour;
}

public static class Terrain
{
    public enum TerrainID { Water, Sand, Grass, Rock, Snow };
}
