using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private Image cameraUI;
    [SerializeField] private Image cameraFlash;

    [SerializeField] private float flashAlpha = 0.95f;
    [SerializeField] private float flashDelay = 0.25f;

    private Color hideFlash = new Color(0, 0, 0, 0);
    private Color showFlash;

	// Use this for initialization
	void Start ()
    {
        showFlash = new Color(1, 1, 1, flashAlpha);
        cameraFlash.color = hideFlash;

        TakePhoto();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            TakePhoto();
        }
    }

    private void TakePhoto()
    {
        cameraFlash.color = showFlash;

        Tween cameraTween = cameraFlash.DOColor(hideFlash, 0.5f)
            .SetEase(Ease.Flash)
            .SetDelay(flashDelay);
    }
}
