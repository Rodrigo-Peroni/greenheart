using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KingPig : SimplePig
{
    public GameObject attackPrefab;
    public Transform spawnAttackPoint;
    public Transform spinAttackJumpPoint;
    public Transform groundCheck;
    public float moveSpeed;
    public Image redScreen;

    public AudioClip swooshSound;
    public AudioClip jumpSound;
    public AudioClip attackSound;

    private bool isAttacking = false;
    private bool isSpinning = false;
    private bool isGrounded = true;
    private bool isInvulnerable = false;

    private Vector3 movementInitialPosition;
    private Vector3 movementFinalPosition;
    private Vector3 targetPlayerPosition;
    private CapsuleCollider2D kingCapsuleCollider;

    private float timer;

    // Start is called before the first frame update
    new void Start()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        player = GameObject.Find("Player").GetComponent<Player>();
        kingCapsuleCollider = GetComponent<CapsuleCollider2D>();        

        isFacingRight = false;
        timer = 0f;

        speed = -speed;
    }

    // Update is called once per frame
    new void Update()
    {
        if (!BossLevelManager.instance.IsInPhaseOne && health > 0)
        {

            if (player.transform.position.x > transform.position.x && !isFacingRight)
            {
                Flip();
            }
            else if (player.transform.position.x <= transform.position.x && isFacingRight)
            {
                Flip();
            }            

            if (isSpinning)
            {
                timer += Time.deltaTime * moveSpeed;
                if (movementInitialPosition != movementFinalPosition)
                {
                    transform.position = Vector3.Lerp(movementInitialPosition, movementFinalPosition, timer);
                }

                isGrounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
            }            
            
            if (isGrounded && isAttacking)
            {
                body.gravityScale = 1f;
                isSpinning = false;
                isAttacking = false;
            }

            animator.SetBool("IsSpinning", isSpinning);
        }
    }

    new void FixedUpdate()
    {        

    }

    public void StartBossLoop()
    {
        StartCoroutine(ExecuteBossLoop());
    }

    private IEnumerator ExecuteBossLoop()
    {
        while (health > 0)
        {
            yield return new WaitForSeconds(1.0f);                                   
            LandAttack();
            yield return new WaitForSeconds(1.0f);            
            LandAttack();

            yield return new WaitForSeconds(2.0f);

            JumpToSpinAttack();

            yield return new WaitForSeconds(1.5f);
            targetPlayerPosition = player.transform.position;
            player.ShowExclamationBox();
            yield return new WaitForSeconds(0.75f);
            SpinAttackPlayer();

            yield return new WaitForSeconds(2.0f);
        }        
    }

    private void SpinAttackPlayer()
    {        
        timer = 0f;
        movementInitialPosition = transform.position;
        movementFinalPosition = targetPlayerPosition;
        isAttacking = true;
        SoundManager.instance.PlayLowSound(attackSound);
        kingCapsuleCollider.enabled = true;
        Invoke("ForceIsGrounded", 1.0f);
    }

    private void ForceIsGrounded()
    {
        isSpinning = false;
        isGrounded = true;        
    }

    private void JumpToSpinAttack()
    {
        body.gravityScale = 0f;
        kingCapsuleCollider.enabled = false;
        movementInitialPosition = transform.position;
        movementFinalPosition = spinAttackJumpPoint.position;
        timer = 0f;
        isGrounded = false;
        isSpinning = true;
        SoundManager.instance.PlayLowestSound(jumpSound);
    }

    void LandAttack()
    {
        animator.SetTrigger("Attack");

        Invoke("CreateAttackObject", 0.2f);
    }

    private void CreateAttackObject()
    {
        GameObject attackObject = Instantiate(attackPrefab, spawnAttackPoint.position, spawnAttackPoint.rotation);
        SoundManager.instance.PlayLowestSound(swooshSound);

        if (!this.isFacingRight)
        {
            attackObject.transform.eulerAngles = new Vector3(180, 0, 180);
        }
    }

    public void ReceiveBoxDamage()
    {
        DamageEnemy(3, 2f);
    }

    private IEnumerator ExecuteKingPigDeath()
    {
        float originalPlayerSpeed = player.speed;        

        player.DisableInputs();
        player.speed = 0;       
        
        BossLevelManager.instance.ExecuteKingPigDeathStepOne();

        animator.SetTrigger("KingPigDeath");
        speed *= -1;
        body.AddForce(new Vector2(speed * 100f, 300f));
        yield return new WaitForSeconds(0.1f);

        Time.timeScale = 0.1f;
        StartCoroutine(RedScreenFadeIn());
        yield return new WaitForSeconds(0.4f);
        for (float timeScale = 0.2f; timeScale <= 0.7f; timeScale += 0.1f)
        {
            yield return new WaitForSeconds(0.1f);
            Time.timeScale = timeScale;
        }
        StartCoroutine(RedScreenFadeOut());
        for (float timeScale = 0.8f; timeScale <= 1.0f; timeScale += 0.1f)
        {
            yield return new WaitForSeconds(0.1f);
            Time.timeScale = timeScale;
        }
        Time.timeScale = 1.0f;

        player.EnableInputs();
        player.speed = originalPlayerSpeed;
        
        BossLevelManager.instance.ExecuteKingPigDeathStepTwo();
        Destroy(gameObject);
    }

    protected new void OnTriggerEnter2D(Collider2D other)
    {                
        if (other.CompareTag("Attack"))
        {
            int damage = other.GetComponentInParent<Attack>().attackPower;
            DamageEnemy(damage, 1f);
        }
        else if (other.CompareTag("Player"))
        {
            other.GetComponentInParent<Player>().DamagePlayer();
        }
    }

    private void DamageEnemy(int damage, float invulnerabilityTime)
    {
        if (!isInvulnerable)
        {
            health -= damage;          

            if (health < 1)
            {
                StartCoroutine(ExecuteKingPigDeath());
            }
            else
            {
                isInvulnerable = true;
                StartCoroutine(DamageEffect(invulnerabilityTime));
            }
        }
    }

    protected IEnumerator DamageEffect(float invulnerabilityTime)
    {        
        sprite.color = Color.red;
        body.AddForce(new Vector2(0f, 300f));

        yield return new WaitForSeconds(0.3f);

        sprite.color = Color.white;

        // Invulnerability Loop
        for (float i = 0f; i < invulnerabilityTime; i += 0.1f)
        {
            sprite.enabled = false;
            yield return new WaitForSeconds(0.1f);
            sprite.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }

        isInvulnerable = false;
    }

    private IEnumerator RedScreenFadeIn()
    {
        Color newColor = redScreen.color;        

        while (redScreen.color.a <= 0.2f)
        {
            newColor.a = newColor.a + 0.01f;
            redScreen.color = newColor;
            yield return new WaitForSecondsRealtime(0.025f);            
        }
    }

    private IEnumerator RedScreenFadeOut()
    {     
        Color newColor = redScreen.color;
        
        while (redScreen.color.a >= 0.0f)
        {
            newColor.a = newColor.a - 0.01f;
            redScreen.color = newColor;
            yield return new WaitForSecondsRealtime(0.025f);            
        }
    }
}