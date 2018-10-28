using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenuComponent : MonoBehaviour
{
    //This is probably over-complicating it but just in case there's more than 2 options
    private enum MenuSelection { Resume, Save, Exit };

    [Header("Text")]
    [SerializeField] private Text promptResume;
    [SerializeField] private Text promptSave;
    [SerializeField] private Text promptExit;

    [Header("Objects")]
    [SerializeField] private GameObject cursor;

    [Header("Player")]
    [SerializeField] private PlayerScript playerInfo;

    private MenuSelection currentSelection = MenuSelection.Resume;

    private bool inMenu = false;

    public bool InMenu
    {
        get { return inMenu; }
        set { inMenu = value; }
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) ) //Move up
            UpdateCursorMovement(-1);
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) //Move down
            UpdateCursorMovement(1);

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            ProcessSelection();
    }

    private void UpdateCursorMovement(int direction)
    {
        int currentIndex = (int)currentSelection;

        if (direction < 0) // Move up
        {
            currentIndex -= 1;

            if (currentIndex < (int)MenuSelection.Resume)
                currentIndex = (int)MenuSelection.Exit;
        }
        else //Move down
        {
            currentIndex += 1;

            if (currentIndex > (int)MenuSelection.Exit)
                currentIndex = (int)MenuSelection.Resume;
        }
        currentSelection = (MenuSelection)currentIndex;

        PositionCursor();
    }

    private void PositionCursor()
    {
        Vector2 newPos = cursor.transform.localPosition;
        switch (currentSelection)
        {
            case MenuSelection.Resume:
                newPos.y = promptResume.transform.localPosition.y;
                break;
            case MenuSelection.Save:
                newPos.y = promptSave.transform.localPosition.y;
                break;
            case MenuSelection.Exit:
                newPos.y = promptExit.transform.localPosition.y;
                break;
            default:
                break;
        }

        cursor.transform.localPosition = newPos;
    }

    private void ProcessSelection()
    {
        switch(currentSelection)
        {
            case MenuSelection.Resume:
                playerInfo.ToggleGameMenu(false);
                break;
            case MenuSelection.Save:
                playerInfo.ToggleGameMenu(false);
                playerInfo.ToggleSaveMenu(true);
                break;

            case MenuSelection.Exit:
                //TODO: Add a prompt, go through photo saving before finally quitting

                Application.Quit();
                break;
            default:
                break;
        }
    }
}
