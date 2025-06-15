using System.Collections;
using UnityEngine;

public class BoxPig : MonoBehaviour
{
    public Transform pickUpPoint;
    public Transform dropPoint;
    public Transform boxSpawnPoint;
    public GameObject boxPrefab;

    public float speed;
    public int health;
    public bool destroyOnEasyMode = false;

    public AudioClip throwSound;

    private SpriteRenderer sprite;
    protected Rigidbody2D rgbd;
    protected Animator animator;
    private Collider2D capsuleCollider;

    private LootTable lootTable;
    private LootDropper lootDropper;

    protected float directionFactor; // Should be 1 or -1;
    private bool hasBox = true;
    private float currentSpeed;
    private bool isThrowingOrPickingUpBox = false;
    private bool isInverted;
    protected bool isLookingRight;

    private PlayerManager playerManager;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();

        if (playerManager.IsEasyMode && destroyOnEasyMode)
        {
            Destroy(gameObject);
        }

        sprite = gameObject.GetComponent<SpriteRenderer>();
        rgbd = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();

        lootTable = gameObject.GetComponent<LootTable>();
        lootDropper = gameObject.GetComponent<LootDropper>();

        BoxPigController boxPigController = gameObject.GetComponentInParent<BoxPigController>();
        bool startsLookingToTheRight = boxPigController.startsLookingToTheRight;
        isInverted = boxPigController.isInverted;

        if (startsLookingToTheRight)
        {
            directionFactor = -1;
            isLookingRight = true;
        }
        else
        {
            directionFactor = 1;
            isLookingRight = false;
        }
        currentSpeed = speed * -directionFactor;
    }

    // Update is called once per frame
    protected void Update()
    {
        animator.SetBool("IsWalking", rgbd.velocity.x != 0f);
        animator.SetBool("HasBox", hasBox);        

        if (!isThrowingOrPickingUpBox)
        {
            if (!hasBox)
            {
                currentSpeed = speed * directionFactor;
            }
            else
            {
                currentSpeed = speed * -directionFactor;
            }
        }
        else
        {
            currentSpeed = 0f;
        }
    }

    protected void FixedUpdate()
    {
        rgbd.velocity = new Vector2(currentSpeed, rgbd.velocity.y);
    }

    // Função nativa da Unity. Só executa caso haja uma colisão onde quem tá colidindo é um trigger (checkbox "IsTrigger" marcado)
    protected void OnTriggerEnter2D(Collider2D other)
    {        
        if (other.CompareTag("DropPoint"))
        {
            StartCoroutine(ThrowBox());
        }
        else if (other.CompareTag("PickUpPoint"))
        {
            StartCoroutine(PickUpBox());
        }
        else if (other.CompareTag("Player"))
        {
            if (!playerManager.IsEasyMode)
            {
                other.GetComponentInParent<Player>().DamagePlayer();
            }
            else
            {
                StartCoroutine(DealDelayedDamage(other));
            }
        }
        else if (other.CompareTag("Attack"))
        {
            int damage = other.GetComponentInParent<Attack>().attackPower;
            DamageEnemy(damage);
        }
    }

    private IEnumerator DealDelayedDamage(Collider2D collider)
    {
        yield return new WaitForSeconds(0.6f);
        if (collider.IsTouching(this.capsuleCollider))
        {
            collider.GetComponentInParent<Player>().DamagePlayer();
        }
    }

    IEnumerator ThrowBox()
    {
        isThrowingOrPickingUpBox = true;

        animator.SetTrigger("ThrowBox");
        hasBox = false;
        yield return new WaitForSeconds(0.45f);
        GameObject box = Instantiate(boxPrefab, boxSpawnPoint.position, boxSpawnPoint.rotation);
        if (sprite.isVisible)
        {
            SoundManager.instance.PlayLowestSound(throwSound);
        }
        if (isInverted)
        {
            box.transform.localScale = new Vector3(-1f, -1f, box.transform.localScale.z);
            box.GetComponent<Rigidbody2D>().gravityScale *= -1f;
        }

        isThrowingOrPickingUpBox = false;
        Flip();
    }

    IEnumerator PickUpBox()
    {
        isThrowingOrPickingUpBox = true;

        animator.SetTrigger("PickUpBox");
        hasBox = true;
        yield return new WaitForSeconds(0.25f);

        isThrowingOrPickingUpBox = false;
        Flip();
    }

    protected void Flip()
    {
        isLookingRight = !isLookingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    public void DamageEnemy(int damage)
    {
        health -= damage;

        StartCoroutine(DamageEffect());

        if (health < 1)
        {
            lootDropper.DropLoot(lootTable.GetLootFromLootTable());
            Destroy(gameObject);
        }
    }

    IEnumerator DamageEffect()
    {
        sprite.color = Color.red;        

        yield return new WaitForSeconds(0.3f);        

        sprite.color = Color.white;
    }
}
