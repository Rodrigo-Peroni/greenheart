using Assets.Utils.Items;
using Assets.Utils.Skills;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float speed;
    public int jumpForce;
    public int health;
    public Transform backGroundCheck;
    public Transform frontGroundCheck;
    public bool enableShakeCamera;

    private bool isInvulnerable = false;
    private bool isGrounded = false;
    private bool isJumping = false;    
    private bool canReceiveInputs = true;
    private bool canDoubleJump = false;
    private bool isDoubleJumping = false;
    //private bool isTouchingDoor = false;
    private bool wasGroundedLastFrame = false;
    private bool isInCoyoteTimer = false;

    private int attackPower;
    private int heartDropRate;
    private int accumulatedDiamonds = 0;

    public bool IsFacingRight { get; set; }
    public bool IsInMovablePlatform { get; set; }    

    private SpriteRenderer sprite;
    private Rigidbody2D body;
    private Animator animator;
    private Collider2D playerCollider;
    // private Transform trans;

    public float attackRate;
    public float actionRate;
    public Transform spawnAttack;    
    public GameObject attackPrefab;
    public LootDropper lootDropper;
    public GameObject exclamationBoxPrefab;
    public Transform exclamationBoxSpawnPoint;

    private float nextAttack = 0;
    private float nextAction = 0;

    private CameraController cameraController;
    public PlayerManager playerManager;    

    public AudioClip sfxHurt;
    public AudioClip sfxJump;
    public AudioClip sfxAttack;

    public int DiamondsCount { get; set; }    

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<CircleCollider2D>();
        lootDropper = GetComponent<LootDropper>();
        // trans = GetComponent<Transform>(); Não precisa pq já existe um objeto com o nome de "transform" nativo

        IsFacingRight = true;
        IsInMovablePlatform = false;

        cameraController = GameObject.Find("Main Camera").GetComponent<CameraController>();

        // Estou tratando o Player Manager inteiramente aqui no código.
        // Uma outra opção que tinha, depois que adicionei o controle estático na criação dele,
        // era de apenas colocar um PlayerManager em cada nível, igual acontece com o SoundManager e Hud.
        // Optei por deixar aqui, porque gosto da ideia de que o PlayerManager é responsabilidade do Player e 
        // não do Nível.
        // 
        // Se fosse seguir esse conceito 100%, eu acredito que a criação do Hud deveria ser responsabilidade do
        // Player também. Mas isso é food for thought pro futuro (tem que levar em consideração que o Hud tem elementos
        // visuais que não dá pra criar direto no código, enquanto o PlayerManager é só um script). Provavelmente teria
        // que ter um Prefab e ir instanciando a partir dele...
        //
        // Havia também a possibilidade do PlayerManager ser só uma classe estática, que não herdasse do MonoBehaviour,
        // mas aqui estou optando por usar a abordagem de tê-lo como um GameObject.
        //try
        //{
        //    playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();            
        //}
        //catch (Exception)
        //{
        //    GameObject newGameObject = new GameObject("PlayerManager");
        //    newGameObject.AddComponent<PlayerManager>();
        //    playerManager = newGameObject.GetComponent<PlayerManager>();
        //}

        // Mudei o código acima porque eu precisava do PlayerManager com a Skill List (e é foda criar ela na mão no código,
        // não foi feita pra isso). Agora, eu chupei o que disse acima e coloquei um PlayerManager em cada nível.
        // Conceitualmente, até o momento, eu não acho que isso é um problema (vide os outros Managers, o conceito é o mesmo).
        // Eu ainda to pegando ele com um Find porque não curti muito o approach de usar uma instancia estática dele dentro dele
        // (igual foi feito no Hud e Sound Manager). MAs no futuro, vale analisar se isso é interessante.
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();

        health = playerManager.playerHealth;
        Hud.instance.RefreshLife(health);

        DiamondsCount = playerManager.diamondsAmount;
        Hud.instance.RefreshDiamonds(DiamondsCount);

        LoadJumpSkill();
        LoadAttackSkill();
        LoadHealSkill();
    }

    #region Skill Loading Methods

    private void LoadJumpSkill()
    {
        Level jumpLevel = playerManager.SkillsList.GetLevelOfSkill("Jump");
        switch (jumpLevel)
        {
            case Level.L0:
                jumpForce = 630;
                canDoubleJump = false;                
                break;
            case Level.L1:
                jumpForce = 700;
                canDoubleJump = false;                
                break;
            case Level.L2:
                jumpForce = 630;
                canDoubleJump = true;                
                break;
        }
    }

    private void LoadAttackSkill()
    {
        Level hammerLevel = playerManager.SkillsList.GetLevelOfSkill("Attack");
        this.attackPower = (int)hammerLevel + 1;        
    }

    private void LoadHealSkill()
    {
        Level heartLevel = playerManager.SkillsList.GetLevelOfSkill("Heal");
        this.heartDropRate = 30 - ((int)heartLevel * 10);
    }

    #endregion

    // Update is called once per frame
    void Update()
    {
        if (canReceiveInputs)
        {
            // Aqui eu to alterando só as variáveis de controle (as de controle de animação tb). Operações no RigidBody estão no FixedUpdate

            // Faz um cast da posição inicial até a final (cria uma "linha") e vê se ele cruza com o o layer indicado

            IsInMovablePlatform = Physics2D.Linecast(transform.position, backGroundCheck.position, 1 << LayerMask.NameToLayer("MovablePlatform")) ||
                Physics2D.Linecast(transform.position, frontGroundCheck.position, 1 << LayerMask.NameToLayer("MovablePlatform"));

            isGrounded = IsInMovablePlatform ||
                Physics2D.Linecast(transform.position, backGroundCheck.position, 1 << LayerMask.NameToLayer("Ground")) ||
                Physics2D.Linecast(transform.position, frontGroundCheck.position, 1 << LayerMask.NameToLayer("Ground"));

            if (isGrounded)
            {
                isDoubleJumping = false;
            }
                
            RaycastHit2D touchedDoor = Physics2D.Linecast(transform.position, spawnAttack.position, 1 << LayerMask.NameToLayer("Door"));            

            if (!isGrounded && wasGroundedLastFrame)
            {
                StartCoroutine(CoyoteTimer());
            }
            wasGroundedLastFrame = isGrounded;

            if (Input.GetButtonDown("Jump") && (isGrounded || (canDoubleJump && !isDoubleJumping)) && !isInCoyoteTimer)
            {
                if (!isGrounded)
                {
                    isDoubleJumping = true;
                }
                isJumping = true;
                SoundManager.instance.PlaySound(sfxJump);
            }

            SetAnimations();

            if (Input.GetButton("Fire1") && Time.time > nextAttack)
            {
                Attack();
                SoundManager.instance.PlayLowestSound(sfxAttack);
            }
            
            if (Input.GetButton("Fire2") && touchedDoor && Time.time > nextAction)
            {
                InteractWithDoor(touchedDoor);                
            }

            if (Input.GetButtonDown("Cancel") && !PauseMenu.isGamePaused)
            {
                Hud.instance.PauseGame();
            }

            #region Debug Methods

            if (Input.GetKeyDown(KeyCode.T))
            {
                if (playerManager.SkillsList.Skills["Jump"].IsNotInMaximumLevel)
                {
                    playerManager.SkillsList.Skills["Jump"].UpgradeSkill();
                }
                else
                {
                    playerManager.SkillsList.Skills["Jump"].ResetSkill();
                }
                LoadJumpSkill();
                Debug.Log("Jump Skill Level: " + playerManager.SkillsList.Skills["Jump"].SkillCurrentLevel);
            }

            //if (Input.GetKeyDown(KeyCode.G))
            //{
            //    if (playerManager.SkillsList.Skills["Attack"].IsNotInMaximumLevel)
            //    {
            //        playerManager.SkillsList.Skills["Attack"].UpgradeSkill();
            //    }
            //    else
            //    {
            //        playerManager.SkillsList.Skills["Attack"].ResetSkill();
            //    }                
            //    LoadAttackSkill();
            //    Debug.Log("Attack Skill Level: " + playerManager.SkillsList.Skills["Attack"].SkillCurrentLevel);
            //}

            //if (Input.GetKeyDown(KeyCode.V))
            //{
            //    if (playerManager.SkillsList.Skills["Heal"].IsNotInMaximumLevel)
            //    {
            //        playerManager.SkillsList.Skills["Heal"].UpgradeSkill();
            //    }
            //    else
            //    {
            //        playerManager.SkillsList.Skills["Heal"].ResetSkill();
            //    }                
            //    LoadHealSkill();
            //    Debug.Log("Heal Skill Level: " + playerManager.SkillsList.Skills["Heal"].SkillCurrentLevel);
            //}

            #endregion
        }
    }

    private IEnumerator CoyoteTimer()
    {
        isInCoyoteTimer = true;
        float startTime = Time.time;        

        while (Time.time <= startTime + 0.1f)
        {
            if (Input.GetButtonDown("Jump") && !isJumping && body.velocity.y < 0)
            {
                Debug.Log("Coyote Jump");
                isJumping = true;
                SoundManager.instance.PlaySound(sfxJump);
            }
            yield return null;
        }

        isInCoyoteTimer = false;
    }

    // Usado pra updates que serão feitos no RigidBody
    void FixedUpdate()
    {
        if (canReceiveInputs)
        {
            float move = Input.GetAxis("Horizontal");            
            body.velocity = new Vector2(move * speed, body.velocity.y);

            if ((move < 0f && IsFacingRight) || (move > 0f && !IsFacingRight))
            {
                Flip();
            }

            if (isJumping)
            {
                if (isDoubleJumping)
                {
                    body.velocity = new Vector2(body.velocity.x, 0f);
                }
                body.AddForce(new Vector2(0f, jumpForce));
                // Posso usar AddForce por exemplo com Vector3.up, pega a direção do vetor pra cima. Vc pode multiplicar com um valor.
                // Tipo, jogar uma força pra cima com 500 de Força: body.AddForce(Vector3.up * 500);
                isJumping = false;
            }
        }
    }

    public void Flip()
    {
        IsFacingRight = !IsFacingRight;        
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);        
    }

    public void SetAnimations()
    {
        animator.SetFloat("VelocityY", body.velocity.y);
        animator.SetBool("IsJumpingOrFalling", !isGrounded);
        animator.SetBool("IsWalking", (body.velocity.x != 0f) && isGrounded);        
    }

    void Attack()
    {
        animator.SetTrigger("Attack");
        nextAttack = Time.time + attackRate;

        GameObject attackObject = Instantiate(attackPrefab, spawnAttack.position, spawnAttack.rotation);
        attackObject.GetComponent<Attack>().attackPower = this.attackPower;

        if (!IsFacingRight)
        {
            attackObject.transform.eulerAngles = new Vector3(180, 0, 180);
        }
    }

    private void InteractWithDoor(RaycastHit2D touchedDoor)
    {
        nextAction = Time.time + actionRate;
        Door door = touchedDoor.transform.gameObject.GetComponent<Door>(); //Magic ACCESS Line

        if (!door.isOpened)
        {
            canReceiveInputs = false;
            body.velocity = new Vector2(0f, 0f);
            transform.position = door.GetComponentInChildren<Transform>().position;
            animator.SetTrigger("DoorIn");
        }

        door.ActivateDoor();        
    }

    public void ExecuteDoorOut()
    {
        canReceiveInputs = true;
        animator.SetTrigger("DoorOut");
    }

    public IEnumerator HitAndFall()
    {
        DisableInputs();
        animator.SetTrigger("HitAndFall");
        isInvulnerable = true;
        yield return new WaitForSeconds(1.7f);
        animator.SetTrigger("PlayerDeath");
        yield return new WaitForSeconds(1.0f);
        GoToIdleState();
        EnableInputs();
        isInvulnerable = false;
    }

    public void DisableInputs()
    {
        canReceiveInputs = false;
    }

    public void EnableInputs()
    {
        canReceiveInputs = true;
    }

    public void GoToIdleState()
    {
        animator.SetBool("IsJumpingOrFalling", false);
        animator.SetTrigger("GoToIdle");
    }

    public void AddDiamonds(int amountToAdd)
    {
        DiamondsCount += amountToAdd;
        accumulatedDiamonds += amountToAdd;
        if (accumulatedDiamonds >= heartDropRate)
        {
            accumulatedDiamonds -= heartDropRate;
            lootDropper.DropLoot(new LootItem(ItemCode.Heart, 1));
        }
    }

    public void PickUpHeart()
    {
        if (health < 3)
        {
            health++;
        }
        else
        {
            AddDiamonds(5);
            Hud.instance.RefreshDiamonds(DiamondsCount);
        }
    }

    IEnumerator DamageEffect()
    {
        if (enableShakeCamera)
        {
            cameraController.ShakeCamera(0.3f, 0.05f);
        }        

        for(float i = 0f ; i<1f ; i += 0.1f)
        {
            sprite.enabled = false;
            yield return new WaitForSeconds(0.1f);
            sprite.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }

        isInvulnerable = false;
    }

    public void DamagePlayer()
    {
        if (!isInvulnerable && health > 0)
        {            
            health -= 1;
            if (health <= 0)
            {
                Hud.instance.RefreshLife(0);
            }
            else
            {
                Hud.instance.RefreshLife(health);
            }                
            SoundManager.instance.PlaySound(sfxHurt);

            if (health < 1)
            {
                canReceiveInputs = false;
                animator.SetTrigger("PlayerDeath");
                body.velocity = new Vector2(0f, 0f);                
                // Espera 3 segundos pra executar o método
                playerManager.playerHealth = 3;
                playerManager.diamondsAmount = 0;
                Invoke("ReloadLevel", 3f);                
            }
            else
            {
                isInvulnerable = true;
                StartCoroutine(DamageEffect());
            }
        }

    }

    private void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public void ShowExclamationBox()
    {
        StartCoroutine(CallExclamationBoxAnimation());
    }

    private IEnumerator CallExclamationBoxAnimation()
    {
        GameObject boxInstance = Instantiate(exclamationBoxPrefab, exclamationBoxSpawnPoint);
        yield return new WaitForSeconds(0.8f);
        Destroy(boxInstance);
    }

    public void DisableAllColliders()
    {
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }
    }

    public void EnableAllColliders()
    {
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = true;
        }
    }

    public void SetIsGrounded(bool isGrounded)
    {
        this.isGrounded = isGrounded;
    }
}
