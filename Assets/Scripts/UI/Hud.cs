using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour
{
    public Sprite[] sprites;
    public Image lifeBar;
    public Sprite[] diamondNumbers;
    public SpriteRenderer HundredCount;
    public SpriteRenderer TenCount;
    public SpriteRenderer UnitCount;
    public GameObject blurScreen;
    public GameObject skillsMenu;
    public GameObject levelChangeMenu;
    public GameObject pauseMenu;

    private GameObject blurScreenInstance;
    private GameObject skillsMenuInstance;
    private GameObject levelChangeMenuInstance;    

    public static Hud instance;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        PauseMenu.isGamePaused = true;
        pauseMenu.SetActive(true);        
    }

    public void RefreshLife(int playerHealth)
    {
        lifeBar.sprite = sprites[playerHealth];
    }

    public void RefreshDiamonds(int diamondsCount)
    {        
        int hundredValue = diamondsCount / 100;
        diamondsCount = diamondsCount - (hundredValue * 100);
        int tenValue = diamondsCount / 10;
        diamondsCount = diamondsCount - (tenValue * 10);
        int unitValue = diamondsCount;

        HundredCount.sprite = diamondNumbers[hundredValue];
        TenCount.sprite = diamondNumbers[tenValue];
        UnitCount.sprite = diamondNumbers[unitValue];
    }

    public void TurnOnBlur()
    {
        blurScreenInstance = Instantiate(blurScreen, transform);
    }

    public void TurnOffBlur()
    {
        if (blurScreenInstance)
        {
            Destroy(blurScreenInstance);
        }
    }

    public void ShowSkillsMenu()
    {
        skillsMenuInstance = Instantiate(skillsMenu, transform);
    }

    public void HideSkillsMenu()
    {
        if (skillsMenuInstance)
        {
            Destroy(skillsMenuInstance);
        }
    }

    public void ShowLevelChangeMenu()
    {
        levelChangeMenuInstance = Instantiate(levelChangeMenu, transform);
    }

    public void HideLevelChangeMenu()
    {
        if (levelChangeMenuInstance)
        {
            Destroy(levelChangeMenuInstance);
        }
    }
}
