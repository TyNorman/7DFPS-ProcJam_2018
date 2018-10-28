using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMapRenderer : MonoBehaviour
{
    [SerializeField] private Renderer renderer;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;

    private GameObject meshObject;
    private MeshCollider meshCollider;

    public GameObject MeshObject
    {
        get { return meshObject; }
    }

    public Mesh TerrainMesh
    {
        get { return meshObject.GetComponent<MeshFilter>().sharedMesh; }
    }

    public MeshCollider meshColliderObj
    {
        get { return meshCollider; }
    }

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

        //Get the mesh GameObject and add a MeshCollider to it
        if (meshCollider != null)
            meshCollider.sharedMesh = meshFilter.sharedMesh;

        if (meshCollider == null)
        {
            meshObject = meshRenderer.gameObject;
            meshCollider = meshObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = meshFilter.sharedMesh;
        }
    }
}
