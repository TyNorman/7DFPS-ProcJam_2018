using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMeshMaker : MonoBehaviour
{
    [SerializeField] public Renderer renderer;

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
}
