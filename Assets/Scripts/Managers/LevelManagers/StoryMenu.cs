using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoryMenu : MonoBehaviour
{
    public Text storyText;
    public Text stopPoint;
    public Text startPoint;
    public float speed;
    public Camera mainCamera;
    public AudioClip storyMusic;

    private bool canUpdate = false;
    private bool isTextInCamera = false;
    private bool isExiting = false;
    private bool canSkip = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ScrollText());
        SoundManager.instance.FadeInMusic(storyMusic, 5.0f);
        storyText.canvasRenderer.SetAlpha(1.0f);        
    }

    // Update is called once per frame
    void Update()
    {
        if (canUpdate)
        {
            storyText.transform.position = new Vector3(storyText.transform.position.x, 
                storyText.transform.position.y + (speed * Time.deltaTime), storyText.transform.position.z);
        }

        if (Input.anyKey && !isExiting &&canSkip)
        {
            isExiting = true;
            StartCoroutine(InterruptStory());
        }
    }

    private IEnumerator InterruptStory()
    {
        storyText.CrossFadeAlpha(0f, 2.0f, false);
        SoundManager.instance.FadeOutMusic(2.0f, false);
        yield return new WaitForSeconds(2.5f);
        canUpdate = false;
        SceneManager.LoadScene("Level1", LoadSceneMode.Single);
    }

    private IEnumerator ScrollText()
    {
        yield return new WaitForSeconds(2.0f);
        canUpdate = true;

        while (canUpdate)
        {
            if (!canSkip)
            {
                if (startPoint.rectTransform.IsFullyVisibleFrom(mainCamera))
                {                    
                    Invoke("ActivateSkip", 2.0f);
                }
            }

            if (stopPoint.rectTransform.IsFullyVisibleFrom(mainCamera))
            {
                isTextInCamera = true;
            }
            else
            {
                if (isTextInCamera)
                {
                    StartCoroutine(LoadFirstLevel());
                }
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator LoadFirstLevel()
    {
        canUpdate = false;
        SoundManager.instance.FadeOutMusic(2.0f, false);
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene("Level1", LoadSceneMode.Single);
    }

    private void ActivateSkip()
    {
        canSkip = true;
    }
}
