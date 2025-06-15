using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndCredits : MonoBehaviour
{
    public Text textDevelopedBy;
    public Text textAssetsBy;
    public Text textThanksArtists;
    public GameObject logo;
    public Text textPressAnyKey;
    public AudioClip creditsMusic;

    [SerializeField]
    private Text[] assetsByBodies = null;

    private Image[] logoImages;

    private bool isExiting = false;
    private PlayerManager playerManager;
    private bool isInMenu;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
        isInMenu = playerManager.IsInMenu;

        if (isInMenu)
        {
            SoundManager.instance.FadeInMusic(creditsMusic, 2.0f);
        }

        textDevelopedBy.canvasRenderer.SetAlpha(0.0f);
        textAssetsBy.canvasRenderer.SetAlpha(0.0f);
        textThanksArtists.canvasRenderer.SetAlpha(0.0f);
        foreach (Text body in assetsByBodies)
        {
            body.canvasRenderer.SetAlpha(0.0f);
        }
        logoImages = logo.GetComponentsInChildren<Image>();
        foreach (Image image in logoImages)
        {
            image.canvasRenderer.SetAlpha(0.0f);
        }
        textPressAnyKey.canvasRenderer.SetAlpha(0.0f);

        StartCoroutine(StartEndCreditsSequence());
    }

    private void Update()
    {
        if (Input.anyKey && !isExiting && isInMenu)
        {
            isExiting = true;            
            StopAllCoroutines();            

            textDevelopedBy.CrossFadeAlpha(0f, 2.5f, false);
            textAssetsBy.CrossFadeAlpha(0f, 2.5f, false);
            textThanksArtists.CrossFadeAlpha(0f, 2.5f, false);
            foreach (Text body in assetsByBodies)
            {
                body.CrossFadeAlpha(0f, 2.5f, false);
            }
            foreach (Image image in logoImages)
            {
                image.CrossFadeAlpha(0f, 2.5f, false);
            }
            textPressAnyKey.CrossFadeAlpha(0f, 2.5f, false);

            SoundManager.instance.FadeOutMusic(2.0f);
            Invoke("GoBackToMenu", 4.5f);
        }
    }

    private void GoBackToMenu()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    private IEnumerator StartEndCreditsSequence()
    {
        yield return new WaitForSeconds(2.0f);

        textDevelopedBy.CrossFadeAlpha(1f, 2.5f, false);
        yield return new WaitForSeconds(5.0f);
        textDevelopedBy.CrossFadeAlpha(0f, 2.5f, false);
        yield return new WaitForSeconds(4.5f);

        textAssetsBy.CrossFadeAlpha(1f, 2.5f, false);
        foreach (Text body in assetsByBodies)
        {
            body.CrossFadeAlpha(1f, 2.5f, false);
            yield return new WaitForSeconds(8.0f);
            body.CrossFadeAlpha(0f, 2.5f, false);
            yield return new WaitForSeconds(2.5f);
        }
        textAssetsBy.CrossFadeAlpha(0f, 2.5f, false);

        yield return new WaitForSeconds(4.5f);
        textThanksArtists.CrossFadeAlpha(1f, 2.5f, false);
        yield return new WaitForSeconds(7.0f);
        textThanksArtists.CrossFadeAlpha(0f, 2.5f, false);

        yield return new WaitForSeconds(4.5f);
        foreach (Image image in logoImages)
        {
            image.CrossFadeAlpha(0.9f, 3.5f, false);
        }

        yield return new WaitForSeconds(5.5f);
        textPressAnyKey.CrossFadeAlpha(1f, 2.5f, false);

        isInMenu = true;
    }
}
