using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroLevelManager : MonoBehaviour
{
    public Image logoText;
    public Image logoHeart;
    public Text pressAnyKeyText;

    private bool canPressKey = false;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;

        logoText.canvasRenderer.SetAlpha(0.0f);
        logoHeart.canvasRenderer.SetAlpha(0.0f);
        pressAnyKeyText.canvasRenderer.SetAlpha(0.0f);

        StartCoroutine(ShowIntro());
    }

    private IEnumerator ShowIntro()
    {
        yield return new WaitForSeconds(3.0f);
        logoText.CrossFadeAlpha(1f, 2.0f, false);
        yield return new WaitForSeconds(3.5f);
        logoHeart.CrossFadeAlpha(1f, 1.5f, false);
        yield return new WaitForSeconds(2.0f);
        pressAnyKeyText.CrossFadeAlpha(1f, 1.0f, false);
        canPressKey = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKey && canPressKey)
        {
            SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        }
    }
}
