using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotoSaveMenuComponent : MonoBehaviour
{
    private const int NUM_ROWS = 4;
    private const int NUM_COLS = 6;

    [Header("Player Info")]
    [SerializeField] private PlayerScript playerInfo;

    [Header("Photo Preview")]
    [SerializeField] private Image photoPreview;

    [Header("UI Objects")]
    [SerializeField] private GameObject cursorObj;
    [SerializeField] private GameObject checkmarkObj;
    [SerializeField] private GameObject saveCursorObj;

    [Header("UI Properties")]
    [SerializeField] private int photoSpacing;

    private List<int> photosToSaveIndices = new List<int>();

    private Image[] photoPreviews;

    private Vector2 previewStartPos;

    private int selectedPhotoIndex = 0;
    private int lastPhotoSlot = 0; //Index of the last photo slot that isn't empty

    private bool highlightedSavePrompt = false;

	// Use this for initialization
	void Start ()
    {
        photoPreview.gameObject.SetActive(false); //Hide the initial preview obj
        checkmarkObj.SetActive(false);
        saveCursorObj.SetActive(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) //Move up
            UpdateCursorMovement(0, -1);
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) //Move down
            UpdateCursorMovement(0, 1);
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) //Move up
            UpdateCursorMovement(-1, 0);
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) //Move down
            UpdateCursorMovement(1, 0);

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            ProcessSelection();

        if (Input.GetKeyDown(KeyCode.Escape))
            CloseMenu();
    }

    public void TogglePhotoMenu(bool showMenu)
    {
        this.enabled = showMenu;
        gameObject.SetActive(showMenu);

        if (showMenu)
            PopulatePreviews();
    }

    private void PopulatePreviews()
    {
        previewStartPos = photoPreview.transform.localPosition;

        if (photoPreviews != null && photoPreviews.Length > 0)
        {
            for (int i = 0; i < photoPreviews.Length; i++)
            {
                if (photoPreviews[i] != null)
                    Destroy(photoPreviews[i].gameObject);
            }
        }

        photoPreviews = new Image[playerInfo.TakenPhotos.Length];

        if (playerInfo.NumPhotosTaken > 0)
        {
            cursorObj.SetActive(true);

            for (int i = 0; i < playerInfo.TakenPhotos.Length; i++)
            {
                if (playerInfo.TakenPhotos[i] != null)
                {
                    GameObject newPreviewObj = GameObject.Instantiate(photoPreview.gameObject);
                    Image currentPreview = newPreviewObj.GetComponent<Image>();
                    RectTransform rectTransform = newPreviewObj.GetComponent<RectTransform>();

                    photoPreviews[i] = currentPreview;

                    //Apply the saved photo to the preview
                    Sprite snapshotSprite = Sprite.Create(playerInfo.TakenPhotos[i], new Rect(0, 0, playerInfo.TakenPhotos[i].width, playerInfo.TakenPhotos[i].height), new Vector2(0.5f, 0.5f), 1);
                    currentPreview.sprite = snapshotSprite;

                    newPreviewObj.transform.SetParent(gameObject.transform);
                    newPreviewObj.SetActive(true);

                    rectTransform.anchoredPosition = photoPreview.rectTransform.anchoredPosition;
                    rectTransform.localScale = photoPreview.rectTransform.localScale;
                    rectTransform.localRotation = photoPreview.rectTransform.localRotation;

                    //Position the current preview within the row/column
                    int row = Mathf.CeilToInt(i / NUM_COLS);
                    int col = i - Mathf.CeilToInt(row * NUM_COLS);

                    float photoWidth = rectTransform.rect.width;
                    float photoHeight = rectTransform.rect.height;

                    //Debug.Log(row + "| " + col);

                    //If the photo is outside of the visible rows, hide it
                    if (row > NUM_ROWS)
                        newPreviewObj.SetActive(false);

                    Debug.Log(previewStartPos.x);

                    Vector2 newPos = Vector2.zero;
                    newPos.x = previewStartPos.x - ((photoWidth + photoSpacing) * col);
                    newPos.y = previewStartPos.y - ((photoHeight + photoSpacing) * row);

                    rectTransform.localPosition = newPos;

                    lastPhotoSlot = i;
                }
            }
        }
        else
            cursorObj.SetActive(false); //Hide the cursor if there's nothing

        PositionCursor(); //Position cursor on the first element
    }

    private void PositionCursor()
    {
        if (photoPreviews[selectedPhotoIndex] != null)
            cursorObj.transform.localPosition = photoPreviews[selectedPhotoIndex].transform.localPosition;

    }

    private void UpdateCursorMovement(int directionX, int directionY)
    {
        if (directionX != 0) //Move horizontally
            selectedPhotoIndex += directionX;
        else //Move vertically
            selectedPhotoIndex += directionY * NUM_COLS;

        if (selectedPhotoIndex > lastPhotoSlot && !highlightedSavePrompt) //Move to the Save Photos prompt
        {
            cursorObj.SetActive(false);
            saveCursorObj.SetActive(true);
            highlightedSavePrompt = true;
        }
        if (selectedPhotoIndex < 0)
            selectedPhotoIndex = 0;
        if (selectedPhotoIndex > lastPhotoSlot)
            selectedPhotoIndex = lastPhotoSlot;

        if (highlightedSavePrompt && directionY < 0)
        {
            cursorObj.SetActive(true);
            highlightedSavePrompt = false;
            saveCursorObj.SetActive(false);
            selectedPhotoIndex = lastPhotoSlot;
        }

        //Debug.Log(selectedPhotoIndex);

        PositionCursor();
    }

    private void ProcessSelection()
    {
        //Checkmark/de-checkmark the photo to select/deselect, either populate or remove it from a list of photos to save to disk
        if (!highlightedSavePrompt)
            ToggleSelectionPhoto();
        else
            SavePhotos();
    }

    private void ToggleSelectionPhoto()
    {
        if (photoPreviews[selectedPhotoIndex] != null)
        {
            if (photoPreviews[selectedPhotoIndex].transform.childCount > 0) //De-select
            {
                GameObject.Destroy(photoPreviews[selectedPhotoIndex].transform.GetChild(0).gameObject);
                photosToSaveIndices.Remove(selectedPhotoIndex);
            }
            else //Select
            {
                GameObject newCheckmark = GameObject.Instantiate(checkmarkObj);
                newCheckmark.SetActive(true);
                newCheckmark.transform.SetParent(photoPreviews[selectedPhotoIndex].transform);
                newCheckmark.transform.localScale = Vector3.one;
                newCheckmark.transform.localPosition = Vector2.zero;
                newCheckmark.transform.localRotation = Quaternion.identity;

                photosToSaveIndices.Add(selectedPhotoIndex);
            }
        }
    }

    private void SavePhotos()
    {
        for (int i = 0; i < photosToSaveIndices.Count; i++)
        {
            playerInfo.SavePhotoToDisc(photosToSaveIndices[i]);
        }

        //Clear the list and reset everything
        photosToSaveIndices.Clear();

        playerInfo.InMenu = false;

        CloseMenu();
    }

    private void CloseMenu()
    {
        for (int i = 0; i < photoPreviews.Length; i++)
        {
            if (photoPreviews[i] != null)
                GameObject.Destroy(photoPreviews[i].gameObject);
        }

        selectedPhotoIndex = 0;
        lastPhotoSlot = 0;
        highlightedSavePrompt = false;

        saveCursorObj.SetActive(false);

        TogglePhotoMenu(false);
    }
}
