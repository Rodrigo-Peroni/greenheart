using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLevelManager : MonoBehaviour
{
    public static BossLevelManager instance;

    public GameObject bossPlatform;
    private PathFollower bossPlatformMovement;

    public GameObject phaseTwoBoxPigs;
    private BoxPigController[] phaseTwoPigs;

    public GameObject finalDrawbrige;

    public bool IsInPhaseOne { get; set; } = false;
    public bool IsCannonTurn { get; set; } = false;
    public bool IsBoxesTurn { get; set; } = false;
    public bool IsTrapdoorClosed { get; set; } = true;

    [SerializeField]
    private Player player = null;
    [SerializeField]
    private KingPig kingPig = null;    
    [SerializeField]
    private CameraController cameraControl = null;
    public AudioClip bossPhase1Music;
    public AudioClip bossPhase2Music;
    public AudioClip bossPhase3Music;

    private Coroutine phaseOneCoroutine;

    private PlayerManager playerManager;

    private bool playerAnimationControlledByThisClass = false;
    private void ControlPlayerAnimation() { playerAnimationControlledByThisClass = true; }
    private void ReleasePlayerAnimation() { playerAnimationControlledByThisClass = false; }

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

    void Start()
    {
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
        bossPlatformMovement = bossPlatform.GetComponent<PathFollower>();

        phaseTwoPigs = phaseTwoBoxPigs.GetComponentsInChildren<BoxPigController>();
        EnablePhaseTwoPigs(false);

        SoundManager.instance.StopMusic();
        SoundManager.instance.FadeInMusic(bossPhase1Music, 0.3f);
    }

    void Update()
    {
        if (playerAnimationControlledByThisClass)
        {
            bool isGrounded = Physics2D.Linecast(transform.position, player.backGroundCheck.position, 1 << LayerMask.NameToLayer("Ground")) ||
                              Physics2D.Linecast(transform.position, player.frontGroundCheck.position, 1 << LayerMask.NameToLayer("Ground"));

            player.SetIsGrounded(isGrounded);
            player.SetAnimations();
        }
    }

    public void StartBossLevel()
    {
        IsInPhaseOne = true;
        IsCannonTurn = true;
        phaseOneCoroutine = StartCoroutine(ExecutePhaseOneLoop());        
    }

    private IEnumerator ExecutePhaseOneLoop()
    {
        while (IsTrapdoorClosed)
        {
            if (IsTrapdoorClosed)
            {
                IsCannonTurn = true;
                yield return new WaitForSeconds(6f);
            }            
            if (IsTrapdoorClosed && !playerManager.IsEasyMode)
            {
                IsCannonTurn = false;
                player.ShowExclamationBox();
                yield return new WaitForSeconds(1f);                
            }
            if (IsTrapdoorClosed && !playerManager.IsEasyMode)
            {
                IsBoxesTurn = true;
                yield return new WaitForSeconds(6f);
            }
            if (IsTrapdoorClosed && !playerManager.IsEasyMode)
            {
                IsBoxesTurn = false;
                player.ShowExclamationBox();
                yield return new WaitForSeconds(1f);                
            }
        }

        IsCannonTurn = false;
        IsBoxesTurn = false;
    }

    public void DestroyTrapdoor()
    {
        IsTrapdoorClosed = false;                
        IsCannonTurn = false;
        IsBoxesTurn = false;

        BossLevelCannon[] cannons = GameObject.FindObjectsOfType<BossLevelCannon>();
        foreach (BossLevelCannon cannon in cannons)
        {
            cannon.DestroyControllingPig();
        }

        StartCoroutine(JumpToPhaseTwo());
    }

    IEnumerator JumpToPhaseTwo()
    {
        Rigidbody2D rgbd = player.GetComponent<Rigidbody2D>();

        player.DisableInputs();
        this.ControlPlayerAnimation();

        SoundManager.instance.FadeOutMusic(2.0f);

        yield return new WaitForSeconds(1.0f);

        // JUMP
        if (player.IsFacingRight)
        {
            player.Flip();
        }
        player.DisableAllColliders();
        rgbd.AddForce(new Vector2(0f, 800f));
        yield return new WaitForSeconds(0.3f);
        rgbd.velocity = new Vector2(-5f, rgbd.velocity.y);
        player.EnableAllColliders();

        // WAIT UNTIL MOVEMENT IS FINISHED
        yield return new WaitForSeconds(1.0f);
        rgbd.velocity = new Vector2(0f, 0f);
        player.Flip();

        SoundManager.instance.FadeInMusic(bossPhase2Music, 3.0f);

        // MOVING CAMERA
        cameraControl.smoothTimeY = 1.5f;
        cameraControl.SetYOffset(3f);
        yield return new WaitForSeconds(3.5f);
        for (float offset = 1.4f; offset == 0.1; offset -= 0.1f)
        {
            cameraControl.smoothTimeY = offset;
        }
        cameraControl.smoothTimeY = 0.2f;

        // WAIT FOR PIG PLATFORM MOVEMENT        
        bossPlatformMovement.StartMovement();
        yield return new WaitForSeconds(4.0f);
        this.ReleasePlayerAnimation();
        player.EnableInputs();

        // ENABLE PHASE TWO PIGS
        if (!playerManager.IsEasyMode)
        {
            EnablePhaseTwoPigs(true);
        }        

        // SET PHASE TWO
        IsInPhaseOne = false;
        kingPig.StartBossLoop();
    }

    private void EnablePhaseTwoPigs(bool isActive)
    {
        foreach (BoxPigController pig in phaseTwoPigs)
        {
            pig.gameObject.SetActive(isActive);
        }
    }

    public void ExecuteKingPigDeathStepOne()
    {
        SoundManager.instance.FadeOutMusic(1.0f, false);
        if (!playerManager.IsEasyMode)
        {
            foreach (BoxPigController pig in phaseTwoPigs)
            {
                pig.GetComponentInChildren<BoxPig>().DamageEnemy(3);
            }
        }
    }

    public void ExecuteKingPigDeathStepTwo()
    {
        PigTrigger_BossLevel pigTrigger = GameObject.Find("PigTrigger").GetComponent<PigTrigger_BossLevel>();
        pigTrigger.rotation = 0f;
        pigTrigger.RestartRotation();
        SoundManager.instance.FadeInMusic(bossPhase3Music, 2.0f);
    }
}
