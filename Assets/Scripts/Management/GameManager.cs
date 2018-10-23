using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TerrainGenerator terrainGenerator;

	// Use this for initialization
	void Start ()
    {
        CreateMap();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void CreateMap()
    {
        terrainGenerator.GenerateMap();
    }
}
