using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerQueenLevel : MonoBehaviour
{
    public float speed;
    public CameraController cameraControl;
    public Image blackScreen;
    public Image theEndImage;
    public Text dedicatoryText;
    public GameObject hammer;
    public GameObject greenHeart;
    public Transform hammerSpawnPoint;
    public Transform heartSpawnPointQueen;
    public Transform heartSpawnPointKing;

    private SpriteRenderer sprite;
    private Rigidbody2D body;
    private Animator animator;

    private PlayerManager playerManager;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();

        cameraControl.SetYOffset(2.6f);

        sprite = GetComponent<SpriteRenderer>();
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        StartCoroutine(StartLevelCinematic());

        theEndImage.canvasRenderer.SetAlpha(0.0f);
        dedicatoryText.canvasRenderer.SetAlpha(0.0f);
    }

    private void Update()
    {
        SetAnimations();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        body.velocity = new Vector2(speed, body.velocity.y);
    }

    public void SetAnimations()
    {
        animator.SetFloat("VelocityY", body.velocity.y);
        animator.SetBool("IsJumpingOrFalling", false);
        animator.SetBool("IsWalking", body.velocity.x != 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("LevelTriggers"))
        {
            if (collision.gameObject.name.Equals("PlayerStop"))
            {
                speed = 0f;
            }
            else if (collision.gameObject.name.Equals("CameraChange"))
            {
                cameraControl.smoothTimeY = 1.0f;
                cameraControl.SetYOffset(3.5f);
            }
        }
    }

    private IEnumerator StartLevelCinematic()
    {        
        blackScreen.CrossFadeAlpha(0f, 5f, false);
        yield return new WaitForSeconds(5f); // Fade-In Time

        StartCoroutine(StartMovement());
        yield return new WaitForSeconds(2.0f);

        while (!(speed == 0f))
        {
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(2.0f);
        animator.SetTrigger("DropHammer");
        GameObject hammerInstance = Instantiate(hammer, hammerSpawnPoint);
        hammerInstance.GetComponent<Rigidbody2D>().AddForce(new Vector2(-100f, 100f));

        yield return new WaitForSeconds(1.0f);
        StartCoroutine(StartGeneratingGreenHearts());
        yield return new WaitForSeconds(8.0f);

        blackScreen.CrossFadeAlpha(1f, 2.5f, false);
        yield return new WaitForSeconds(3.5f); // Fade-Out Time
        
        theEndImage.CrossFadeAlpha(1f, 2.5f, false);
        yield return new WaitForSeconds(5.5f); 

        theEndImage.CrossFadeAlpha(0f, 2.5f, false);
        yield return new WaitForSeconds(3.5f);

        dedicatoryText.CrossFadeAlpha(1f, 2.5f, false);
        yield return new WaitForSeconds(5.0f);

        dedicatoryText.CrossFadeAlpha(0f, 2.5f, false);
        yield return new WaitForSeconds(3.5f);

        playerManager.IsInMenu = false;
        SceneManager.LoadScene("EndCredits", LoadSceneMode.Single);
    }

    private IEnumerator StartMovement()
    {
        yield return new WaitForSeconds(1.2f);
        speed = 3f;
    }

    private IEnumerator StartGeneratingGreenHearts()
    {
        float timeToWait = 0.5f;

        while (true)
        {
            yield return new WaitForSeconds(timeToWait);
            Instantiate(greenHeart, heartSpawnPointKing);
            yield return new WaitForSeconds(timeToWait);
            Instantiate(greenHeart, heartSpawnPointQueen);
        }        
    }
}

