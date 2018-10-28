using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.IO;

public class PlayerScript : MonoBehaviour
{
    private const int MAX_NUM_PHOTOS = 30; //The limit for how many photos the player can take

    [Header("Camera Flash Properties")]
    [SerializeField] private Image cameraUI;
    [SerializeField] private Image cameraFlash;

    [SerializeField] private float flashAlpha = 0.95f;
    [SerializeField] private float flashDelay = 0.25f;

    [SerializeField] private Camera playerCam;

    [Header("Snapshot Previews")]
    [SerializeField] private Image[] photoPreviews;

    [Header("UI Canvas")]
    [SerializeField] private Canvas canvas;

    [Header("Text")]
    [SerializeField] private Text numPhotosText;

    [Header("Menus")]
    [SerializeField] private GameMenuComponent gameMenu;
    [SerializeField] private PhotoSaveMenuComponent saveMenu;

    private Texture2D[] takenPhotos = new Texture2D[MAX_NUM_PHOTOS];

    private Color hideFlash = new Color(0, 0, 0, 0);
    private Color showFlash;

    private int numPhotosTaken = 0; //How many photos have been taken, capped at a limit
    private int photoIndex = 0; //Index of the current Photo Preview to populate

    private bool inMenu = false;

    public Texture2D[] TakenPhotos
    {
        get { return takenPhotos; }
    }

    public int NumPhotosTaken
    {
        get { return numPhotosTaken; }
        set { numPhotosTaken = value; }
    }

    public bool InMenu
    {
        get { return inMenu; }
        set { inMenu = value; }
    }

	// Use this for initialization
	void Start ()
    {
        //Hide the flash
        showFlash = new Color(1, 1, 1, flashAlpha);
        cameraFlash.color = hideFlash;

        for (int i = 0; i < photoPreviews.Length; i++)
            photoPreviews[i].enabled = false;

        //Hide the Game Menu
        ToggleGameMenu(false);
        ToggleSaveMenu(false);

        UpdateNumPhotosText();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (!inMenu)
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
        if (numPhotosTaken < MAX_NUM_PHOTOS)
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

            Sprite snapshotSprite = Sprite.Create(snapshotTexture, new Rect(0, 0, snapshotTexture.width, snapshotTexture.height), new Vector2(0.5f, 0.5f), 1);

            //Get the next available photo preview
            if (photoIndex >= photoPreviews.Length)
                photoIndex = 0;

            photoPreviews[photoIndex].sprite = snapshotSprite;

            canvas.enabled = true;

            if (photoPreviews[photoIndex].enabled == false)
                photoPreviews[photoIndex].enabled = true;

            photoIndex++;

            takenPhotos[numPhotosTaken] = snapshotTexture; //Save the photo

            //Update the num photos taken text
            numPhotosTaken++;
            UpdateNumPhotosText();

            //TESTING
            SavePhotoToDisc(numPhotosTaken-1);

            //Animate the camera
            cameraFlash.color = showFlash;

            DOTween.KillAll();

            Tween cameraTween = cameraFlash.DOColor(hideFlash, 0.5f)
                .SetEase(Ease.Flash)
                .SetDelay(flashDelay);
        }
    }

    public void ToggleGameMenu(bool showMenu)
    {
        gameMenu.InMenu = showMenu;
        gameMenu.enabled = showMenu;
        gameMenu.gameObject.SetActive(showMenu);
        inMenu = showMenu;
    }

    public void ToggleSaveMenu(bool showMenu)
    {
        inMenu = showMenu;
        saveMenu.TogglePhotoMenu(showMenu);
    }

    private void UpdateNumPhotosText()
    {
        numPhotosText.text = numPhotosTaken + "/" + MAX_NUM_PHOTOS;
    }

    public void SavePhotoToDisc(int index) //Save a selected photo to disc
    {
        if (takenPhotos[index] != null)
        {
            string fileName = "SavedPhoto_" + index;

            byte[] bytes = takenPhotos[index].EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "/../" + fileName + ".png", bytes);
        }
    }
}
