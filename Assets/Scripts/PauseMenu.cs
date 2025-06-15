using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool isGamePaused = false;

    public GameObject selector;
    public GameObject[] buttons;
    public float commandRate;

    public AudioClip menuChangeSfx;
    public AudioClip menuConfirmSfx;
    public AudioClip menuCancelSfx;

    private int menuOption = 0;
    private float nextCommand;

    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();        
    }

    // Update is called once per frame
    void Update()
    {        
        float verticalValue = GetVerticalValue();
        if (verticalValue != 0f)
        {
            Debug.Log(verticalValue);
        }
        

        if (verticalValue != 0f && Time.unscaledTime > nextCommand)
        {
            if (verticalValue < 0)
            {
                MoveCursorDown();
            }
            else
            {
                MoveCursorUp();
            }
            nextCommand = Time.unscaledTime + commandRate;
        }

        if (Input.GetButtonDown("Submit") && Time.unscaledTime > nextCommand)  
        {
            PerformActionOnCurrentSelection();
        }

        if (Input.GetButtonDown("Back") && Time.unscaledTime > nextCommand)
        {
            menuOption = 0;
            SelectOption(menuOption, true);
            PerformActionOnCurrentSelection();
        }
    }

    #region Menu Movement Procedures

    private float GetVerticalValue()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            return 1f;
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            return -1f;
        }
        else
        {
            float dPadVValue = Input.GetAxis("Vertical");
            if (dPadVValue != 0f)
            {
                return dPadVValue;
            }
        }

        return 0f;
    }

    private void MoveCursorDown()
    {
        if (menuOption == 2)
        {
            menuOption = 0;
        }
        else
        {
            menuOption++;
        }
        SelectOption(menuOption);
    }

    private void MoveCursorUp()
    {
        if (menuOption == 0)
        {
            menuOption = 2;
        }
        else
        {
            menuOption--;
        }
        SelectOption(menuOption);
    }

    #endregion

    private void SelectOption(int optionIndex, bool doNotPlaySound = false)
    {
        selector.transform.position = buttons[optionIndex].transform.position;
        if (!doNotPlaySound)
        {
            SoundManager.instance.PlaySound(menuChangeSfx);
        }
    }

    private void PerformActionOnCurrentSelection()
    {
        switch (menuOption)
        {
            case 0:
                Unpause();
                SoundManager.instance.PlayLowSound(menuCancelSfx);
                gameObject.SetActive(false);
                break;
            case 1:
                Unpause();
                SoundManager.instance.PlaySound(menuConfirmSfx);
                SceneManager.LoadScene("Menu", LoadSceneMode.Single);
                break;
            case 2:
                SoundManager.instance.PlaySound(menuConfirmSfx);
                Application.Quit();
                break;
        }
    }

    private void Unpause()
    {
        Time.timeScale = 1f;
        isGamePaused = false;        
        player.EnableInputs();
    }

    private void Pause()
    {
        player.DisableInputs();
    }

    private void OnEnable()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        Pause();
    }
}
