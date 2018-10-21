using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TerrainMeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        //Centering the mesh
        float topLeftX = (width-1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        MeshData meshData = new MeshData(width, height);
        int vertIndex = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                meshData.vertices[vertIndex] = new Vector3(topLeftX + x, heightMap[x, y] * heightMultiplier, topLeftZ - y);
                meshData.uvs[vertIndex] = new Vector2(x / (float)width, y / (float)height);

                //NOTE: LEFT OFF HERE
                //https://www.youtube.com/watch?v=4RpVBYW1r5M&list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3&index=5
                //12:11

                if (x < width - 1 && y < height - 1) //Ignore the bottom-right edge verts
                {
                    //Add the MeshData in a clockwise order
                    //CLOCKWISE: i, i + w + 1, i + w
                    //           i + w + 1, i, i + 1

                    //COUNTER-CLOCKWISE TEST
                    meshData.AddTri(vertIndex, vertIndex + width, vertIndex + width + 1);
                    meshData.AddTri(vertIndex + width + 1, vertIndex + 1, vertIndex);

                    //Contrary to the tutorial and documentation, this CLOCKWISE version is flipped upside-down

                    //meshData.AddTri(vertIndex, vertIndex + width + 1, vertIndex + width);
                    //meshData.AddTri(vertIndex + width + 1, vertIndex, vertIndex + 1);
                }

                vertIndex++;
            }
        }
        return meshData;
    }
}

public class MeshData
{
    public Vector3[] vertices;
    public Vector2[] uvs;
    public int[] triangles;

    private int triIndex = 0;

    public MeshData(int meshWidth, int meshHeight)
    {
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    public void AddTri(int a, int b, int c)
    {
        triangles[triIndex] = a;
        triangles[triIndex+1] = b;
        triangles[triIndex+2] = c;

        triIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        return mesh;
    }
}
