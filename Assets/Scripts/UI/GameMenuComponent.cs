using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenuComponent : MonoBehaviour
{
    //This is probably over-complicating it but just in case there's more than 2 options
    private enum MenuSelection { Resume, Exit };

    [Header("Text")]
    [SerializeField] private Text promptResume;
    [SerializeField] private Text promptExit;

    [Header("Objects")]
    [SerializeField] private GameObject cursor;

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
    }

    private void UpdateCursorMovement(int direction)
    {
        if (direction < 0) // Move up
        {
            if (currentSelection == MenuSelection.Resume)
                currentSelection = MenuSelection.Exit;
            else
                currentSelection = MenuSelection.Resume;
        }
        else //Move down
        {
            if (currentSelection == MenuSelection.Exit)
                currentSelection = MenuSelection.Resume;
            else
                currentSelection = MenuSelection.Exit;
        }

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
            case MenuSelection.Exit:
                newPos.y = promptExit.transform.localPosition.y;
                break;
            default:
                break;
        }

        cursor.transform.localPosition = newPos;
    }
}
