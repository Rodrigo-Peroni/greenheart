using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    private enum FadeMode
    {
        In,
        Out
    }

    private enum MenuType
    {
        MainMenu,
        DifficultyMenu
    }

    [Serializable]
    public struct MenuItem
    {
        public Text itemText;
        public Transform selectionPoint;
    }

    public Image blackScreen;
    public Image[] logoImages;        
    public float commandRate;
    public GameObject howToPlayScreen;
        
    public MenuItem[] mainMenuItems;    
    public MenuItem[] difficultyMenuItems;

    public AudioClip menuMusic;
    public AudioClip menuChangeSfx;
    public AudioClip menuConfirmSfx;
    public AudioClip menuCancelSfx;

    private MenuType currentMenu = MenuType.MainMenu;
    private int menuOption = 0;

    private bool canChangeSelection = false;
    private float nextCommand;
    private bool isHowToPlayActive = false;

    private PlayerManager playerManager;    

    // Start is called before the first frame update
    void Start()
    {
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();

        // Turning the Black Screen On
        Color blackScreenColor = blackScreen.color;
        blackScreenColor.a = 1f;
        blackScreen.color = blackScreenColor;

        SetAlphaInImages(logoImages, 0f);
        SetAlphaInTexts(mainMenuItems, 0f);
        SetAlphaInTexts(difficultyMenuItems, 0f);

        StartCoroutine(ScreenFadeIn());
        SoundManager.instance.FadeInMusic(menuMusic, 5f);
    }

    // Update is called once per frame
    void Update()
    {   
        if (canChangeSelection)
        {
            float verticalValue = GetVerticalValue();

            if (verticalValue != 0f && Time.time > nextCommand)
            {
                if (verticalValue < 0)
                {
                    MoveCursorDown();
                }
                else
                {
                    MoveCursorUp();
                }
                nextCommand = Time.time + commandRate;
            }

            if (Input.GetButtonDown("Submit") && Time.time > nextCommand)
            {
                PerformActionOnCurrentSelection();
            }
        }

        if (isHowToPlayActive)
        {
            if (Input.GetButtonDown("Back") && Time.time > nextCommand)
            {                
                isHowToPlayActive = false;
                howToPlayScreen.SetActive(false);
                SoundManager.instance.PlayLowSound(menuCancelSfx);
                canChangeSelection = true;
            }
        }

        if (currentMenu == MenuType.DifficultyMenu && Input.GetButtonDown("Back") && Time.time > nextCommand)
        {
            menuOption = 2;
            SelectOption(menuOption, true);
            PerformActionOnCurrentSelection();
        }
    }

    private void SelectOption(int optionIndex, bool doNotPlaySound = false)
    {
        MenuItem[] menu = GetCurrentMenuItems();

        ChangeAllOptionTextsToWhite();
        ChangeTextColorToGreen(menu[optionIndex].itemText);
        if (!doNotPlaySound)
        {
            SoundManager.instance.PlaySound(menuChangeSfx);
        }        
    }

    #region FadeIns & FadeOuts

    private void SetAlphaInImages(Image[] images, float alphaValue)
    {
        foreach (Image image in images)
        {
            image.canvasRenderer.SetAlpha(alphaValue);
        }
    }

    private void FadeImages(Image[] images, float fadeTime, FadeMode fadeMode)
    {
        float fadeTarget = fadeMode == FadeMode.In ? 1f : 0f;

        foreach (Image image in images)
        {
            image.CrossFadeAlpha(fadeTarget, fadeTime, false);
        }
    }

    private void SetAlphaInTexts(MenuItem[] menuItems, float alphaValue)
    {
        foreach (MenuItem item in menuItems)
        {
            item.itemText.canvasRenderer.SetAlpha(alphaValue);
        }
    }

    private void FadeTexts(MenuItem[] menuItems, float fadeTime, FadeMode fadeMode)
    {
        float fadeTarget = fadeMode == FadeMode.In ? 1f : 0f;

        foreach (MenuItem item in menuItems)
        {
            item.itemText.CrossFadeAlpha(fadeTarget, fadeTime, false);
        }
    }

    #endregion    

    private IEnumerator ScreenFadeIn()
    {
        blackScreen.CrossFadeAlpha(0f, 4f, false);
        yield return new WaitForSeconds(4f); // Fade Time

        FadeImages(logoImages, 3f, FadeMode.In);
        yield return new WaitForSeconds(4f); // Fade Time

        StartCoroutine(DisplayMainMenu());
    }

    private IEnumerator ScreenFadeOut()
    {
        canChangeSelection = false;

        blackScreen.CrossFadeAlpha(1f, 3f, false);
        yield return new WaitForSeconds(3f); // Fade Time
    }

    private IEnumerator GoToCreditsMenu()
    {        
        StartCoroutine(ScreenFadeOut());
        SoundManager.instance.FadeOutMusic(3f, false);
        yield return new WaitForSeconds(3f);

        playerManager.IsInMenu = true;
        SceneManager.LoadScene("EndCredits", LoadSceneMode.Single);
    }

    private IEnumerator StartGame()
    {
        StartCoroutine(ScreenFadeOut());
        SoundManager.instance.FadeOutMusic(3f, false);
        yield return new WaitForSeconds(3f);
        
        SceneManager.LoadScene("Story", LoadSceneMode.Single);
    }

    #region Show/Hide Menus

    private IEnumerator DisplayMainMenu()
    {
        FadeTexts(mainMenuItems, 0.75f, FadeMode.In);
        yield return new WaitForSeconds(1f); // Fade Time
        
        menuOption = 0;
        SelectOption(menuOption, true);
        canChangeSelection = true;
    }

    private IEnumerator HideMainMenu()
    {        
        canChangeSelection = false;

        FadeTexts(mainMenuItems, 0.75f, FadeMode.Out);
        yield return new WaitForSeconds(0.75f); // Fade Time               
    }

    private IEnumerator DisplayDifficultyMenu()
    {
        FadeTexts(difficultyMenuItems, 0.75f, FadeMode.In);
        yield return new WaitForSeconds(1f); // Fade Time
        
        menuOption = 1;
        SelectOption(menuOption, true);
        canChangeSelection = true;
    }

    private IEnumerator HideDifficultyMenu()
    {        
        canChangeSelection = false;

        FadeTexts(difficultyMenuItems, 0.75f, FadeMode.Out);
        yield return new WaitForSeconds(0.75f);              
    }

    private IEnumerator GoToDifficultyMenu()
    {
        StartCoroutine(HideMainMenu());
        yield return new WaitForSeconds(0.75f);
        currentMenu = MenuType.DifficultyMenu;
        ChangeAllOptionTextsToWhite();
        StartCoroutine(DisplayDifficultyMenu());
    }

    private IEnumerator GoToMainMenu()
    {
        StartCoroutine(HideDifficultyMenu());
        yield return new WaitForSeconds(0.75f);
        currentMenu = MenuType.MainMenu;
        ChangeAllOptionTextsToWhite();
        StartCoroutine(DisplayMainMenu());
    }

    #endregion

    private void ChangeTextColorToGreen(Text textToChange)
    {
        Color green = new Color(0f, 1, 0.3683546f);
        textToChange.color = green;
    }

    private void ChangeAllOptionTextsToWhite()
    {
        MenuItem[] menu = GetCurrentMenuItems();

        foreach (MenuItem item in menu)
        {
            item.itemText.color = Color.white;            
        }
    }

    private MenuItem[] GetCurrentMenuItems()
    {
        return this.currentMenu == MenuType.MainMenu ? mainMenuItems : difficultyMenuItems;
    }

    #region Menu Movement Procedures

    private float GetVerticalValue()
    {
        if (Input.GetButton("Vertical") || Input.GetButtonDown("Vertical"))
        {
            return Input.GetAxis("Vertical");
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
        if (menuOption == GetCurrentMenuItems().Length - 1)
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
            menuOption = GetCurrentMenuItems().Length - 1;
        }
        else
        {
            menuOption--;
        }
        SelectOption(menuOption);
    }

    #endregion

    private void PerformActionOnCurrentSelection()
    {
        if (currentMenu == MenuType.MainMenu)
        {
            switch (menuOption)
            {
                case 0:
                    StartCoroutine(GoToDifficultyMenu());
                    break;
                case 1:
                    canChangeSelection = false;                                
                    howToPlayScreen.SetActive(true);
                    isHowToPlayActive = true;                                       
                    break;
                case 2:
                    StartCoroutine(GoToCreditsMenu());
                    break;
                case 3:
                    Application.Quit();
                    break;
            }
            SoundManager.instance.PlaySound(menuConfirmSfx);
        }
        else
        {
            switch (menuOption)
            {
                case 0:
                    playerManager.PrepareEasyMode();
                    StartCoroutine(StartGame());
                    SoundManager.instance.PlaySound(menuConfirmSfx);
                    break;
                case 1:
                    playerManager.PrepareNormalMode();
                    StartCoroutine(StartGame());
                    SoundManager.instance.PlaySound(menuConfirmSfx);
                    break;
                case 2:
                    StartCoroutine(GoToMainMenu());
                    SoundManager.instance.PlayLowSound(menuCancelSfx);
                    break;
            }
        }                          
    }
}
