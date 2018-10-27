using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerScript : MonoBehaviour
{
    [Header("Camera Flash Properties")]
    [SerializeField] private Image cameraUI;
    [SerializeField] private Image cameraFlash;

    [SerializeField] private float flashAlpha = 0.95f;
    [SerializeField] private float flashDelay = 0.25f;

    [SerializeField] private Camera playerCam;

    [Header("Snapshot Test")]
    [SerializeField] private Image photoPreview;

    [Header("UI Canvas")]
    [SerializeField] private Canvas canvas;

    [Header("GameMenu")]
    [SerializeField] private GameMenuComponent gameMenu;

    private Color hideFlash = new Color(0, 0, 0, 0);
    private Color showFlash;

	// Use this for initialization
	void Start ()
    {
        //Hide the flash
        showFlash = new Color(1, 1, 1, flashAlpha);
        cameraFlash.color = hideFlash;
        photoPreview.enabled = false;

        //Hide the Game Menu
        ToggleGameMenu(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            TakePhoto();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) //Show the Game Menu
        {
            if (gameMenu.enabled == false)
                ToggleGameMenu(true);
            else
                ToggleGameMenu(false);
        }
    }

    private void TakePhoto()
    {
        //Try to remove the camera from the shot
        canvas.enabled = false;

        //Render out the screenshot
        int currentCamPosX = (int)gameObject.transform.position.x;
        int currentCamPosY = (int)gameObject.transform.position.y;

        RenderTexture renderSpace = new RenderTexture(Screen.width, Screen.height, 24);

        playerCam.targetTexture = renderSpace;
        Texture2D snapshotTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, true);
        playerCam.Render();
        RenderTexture.active = renderSpace;

        //ReadPixels from the top-left to the bottom-right of the screen
        snapshotTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);

        snapshotTexture.filterMode = FilterMode.Point;
        snapshotTexture.Apply(); //Make sure to Apply() so the texture is temp-saved
        playerCam.targetTexture = null;
        RenderTexture.active = null; //notes say to set this to avoid errors

        Sprite returnSnapshot = Sprite.Create(snapshotTexture, new Rect(0, 0, snapshotTexture.width, snapshotTexture.height), new Vector2(0.5f, 0.5f), 1);

        photoPreview.sprite = returnSnapshot;

        canvas.enabled = true;

        if (photoPreview.enabled == false)
            photoPreview.enabled = true;

        //Animate the camera
        cameraFlash.color = showFlash;

        Tween cameraTween = cameraFlash.DOColor(hideFlash, 0.5f)
            .SetEase(Ease.Flash)
            .SetDelay(flashDelay);
    }

    private void ToggleGameMenu(bool showMenu)
    {
        gameMenu.InMenu = showMenu;
        gameMenu.enabled = showMenu;
        gameMenu.gameObject.SetActive(showMenu);
    }
}
