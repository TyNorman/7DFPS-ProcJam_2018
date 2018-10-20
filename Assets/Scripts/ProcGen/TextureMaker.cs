using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureMaker
{
    public static Texture2D TextureFromColourMap(Color[] colourMap, int width, int height)
    {
        Texture2D returnTexture = new Texture2D(width, height);
        returnTexture.filterMode = FilterMode.Point;
        returnTexture.wrapMode = TextureWrapMode.Clamp;
        returnTexture.SetPixels(colourMap);
        returnTexture.Apply();

        return returnTexture;
    }

    public static Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        //Get the width/height dimensions from the 2D noiseMap array
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Color[] colourMap = new Color[width * height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
            }
        }

        return TextureFromColourMap(colourMap, width, height);
    }
}
