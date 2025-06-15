using Assets.Systems.Skills;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelChangeMenu : MonoBehaviour
{
    public float commandRate;

    public Text confirmationMessage;
    public Image handSelector;
    public Transform noButtonHandPoint;
    public Transform yesButtonHandPoint;
    public GameObject selectorFrame;
    public AudioClip menuChangeSfx;
    public AudioClip menuConfirmSfx;
    public AudioClip menuCancelSfx;

    private float nextCommand;
    private int handPosition = 2;

    private PlayerManager playerManager;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();

        confirmationMessage.text = GetConfirmationMessage();

        MoveCursorToPosition(handPosition, true);
    }

    private string GetConfirmationMessage()
    {
        string message = "Are you sure you want to go back to the start of ";
        switch (playerManager.NextLevel)
        {
            case "Menu":
                return "Are you sure you want to go back to the Main Menu? All progress will be lost.";
            case "Level1":
                return message + "Level 1?";
            case "Level2":
                return message + "Level 2?";
            case "Level3":
                return message + "Level 3?";
            default:
                return "Are you sure you want to go back?";
        }
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalValue = GetHorizontalValue();

        if (horizontalValue != 0f && Time.time > nextCommand)
        {
            if (horizontalValue < 0)
            {
                MoveCursorLeft();
            }
            else
            {
                MoveCursorRight();
            }
            nextCommand = Time.time + commandRate;
        }

        if (Input.GetButtonDown("Submit") && Time.time > nextCommand)
        {
            PerformActionOnCurrentSelection();
        }

        if(Input.GetButtonDown("Back") && Time.time > nextCommand)
        {
            handPosition = 2;
            MoveCursorToPosition(handPosition, true);
            PerformActionOnCurrentSelection();
        }
    }

    private void PerformActionOnCurrentSelection()
    {
        Hud.instance.TurnOffBlur();
        Hud.instance.HideLevelChangeMenu();
        if (handPosition == 2) //NO
        {
            SoundManager.instance.PlayLowSound(menuCancelSfx);
            GameObject.Find("Player").GetComponent<Player>().ExecuteDoorOut();
        }
        else
        {
            SoundManager.instance.PlaySound(menuConfirmSfx);
            playerManager.LoadNextLevel();
        }  
    }

    #region Cursor Movement

    private void MoveCursorLeft()
    {
        if (handPosition == 2)
        {
            handPosition = 1;
        }
        else
        {
            handPosition = 2;
        }
        MoveCursorToPosition(handPosition);
    }

    private void MoveCursorRight()
    {
        MoveCursorLeft();
    }

    private void MoveCursorToPosition(int handPosition, bool doNotPlaySound = false)
    {
        switch (handPosition)
        {
            case 1:
                handSelector.transform.position = yesButtonHandPoint.position;
                selectorFrame.transform.position = yesButtonHandPoint.parent.position;
                break;
            case 2:
                handSelector.transform.position = noButtonHandPoint.position;
                selectorFrame.transform.position = noButtonHandPoint.parent.position;
                break;
        }
        if (!doNotPlaySound)
        {
            SoundManager.instance.PlaySound(menuChangeSfx);
        }        
    }

    #endregion

    #region Get Directional Values

    private float GetHorizontalValue()
    {
        if (Input.GetButton("Horizontal") || Input.GetButtonDown("Horizontal"))
        {
            return Input.GetAxis("Horizontal");
        }
        else
        {
            float dPadHValue = Input.GetAxis("Horizontal");
            if (dPadHValue != 0f)
            {
                return dPadHValue;
            }
        }

        return 0f;
    }

    #endregion


}
