using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level1LevelManager : MonoBehaviour
{
    public Image blackScreen;
    public AudioClip level1Music;

    private PlayerManager playerManager;    
    private SpriteRenderer playerSpriteRenderer;
    private Color playerSpriteColor;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();        
        playerSpriteRenderer = GameObject.Find("Player").GetComponent<SpriteRenderer>();
        playerSpriteColor = playerSpriteRenderer.color;

        if (playerManager.IsEnteringLevel1ForTheFirstTime)
        {
            blackScreen.canvasRenderer.SetAlpha(1.0f);
            playerSpriteColor.a = 0f;
            playerSpriteRenderer.color = playerSpriteColor;
            StartCoroutine(ScreenFadeIn());
            playerManager.IsEnteringLevel1ForTheFirstTime = false;
        }
        else
        {
            blackScreen.canvasRenderer.SetAlpha(0.0f);
            playerSpriteColor.a = 1f;
            playerSpriteRenderer.color = playerSpriteColor;
        }

        SoundManager.instance.StopMusic();
        SoundManager.instance.FadeInMusic(level1Music, 0.3f);
    }

    private IEnumerator ScreenFadeIn()
    {
        Time.timeScale = 0f;
        blackScreen.CrossFadeAlpha(0f, 2.0f, true);
        yield return new WaitForSecondsRealtime(2.0f);
        Time.timeScale = 1f;
        playerSpriteColor.a = 1f;
        playerSpriteRenderer.color = playerSpriteColor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
