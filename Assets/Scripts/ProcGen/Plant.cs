using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    [Header("Plant Sprites")]
    [SerializeField] private Sprite[] plantSprites;

	// Use this for initialization
	void Start ()
    {
        RandomizePlantSprite();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void RandomizePlantSprite()
    {
        Sprite randSprite = null;

        //Randomize the plant sprite
        int randIndex = Random.Range(0, plantSprites.Length);
        randSprite = plantSprites[randIndex];

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            SpriteRenderer spriteRenderer = gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>();

            if (spriteRenderer != null && randSprite != null)
                spriteRenderer.sprite = randSprite;
        }
    }
}
