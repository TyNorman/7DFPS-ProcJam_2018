using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMapRenderer : MonoBehaviour
{
    [SerializeField] private Renderer renderer;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void RenderMap(Texture2D mapTexture)
    {
        renderer.sharedMaterial.mainTexture = mapTexture;
        renderer.transform.localScale = new Vector3(mapTexture.width, 1, mapTexture.height);
    }

    public void RenderMesh(MeshData meshData, Texture2D texture)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
}
