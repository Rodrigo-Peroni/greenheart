using System.Collections;
using UnityEngine;

public class SimplePig : MonoBehaviour
{
    public float speed;
    public int health;
    public Transform wallCheck;
    public bool canFlip = true;
    public bool destroyOnEasyMode = false;

    protected bool isFacingRight = true;
    private bool hasTouchedWall = false;

    protected SpriteRenderer sprite;
    protected Rigidbody2D body;
    protected Animator animator;
    private Collider2D capsuleCollider;

    protected Player player;
    private LootTable lootTable;
    private LootDropper lootDropper;

    private PlayerManager playerManager;

    // Start is called before the first frame update
    protected void Start()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        player = GameObject.Find("Player").GetComponent<Player>();
        lootTable = gameObject.GetComponent<LootTable>();
        lootDropper = gameObject.GetComponent<LootDropper>();
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();

        if (playerManager.IsEasyMode && destroyOnEasyMode)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    protected void Update()
    {
        hasTouchedWall = Physics2D.Linecast(transform.position, wallCheck.position, 1 << LayerMask.NameToLayer("EnemyRouteCollider")); // Posso tb criar uma nova layer "Wall"

        if (hasTouchedWall)
        {
            Flip();
        }

        animator.SetBool("IsWalking", body.velocity.x != 0f);
    }

    protected void FixedUpdate()
    {
        body.velocity = new Vector2(speed, body.velocity.y);
    }

    // Função nativa da Unity. Só executa caso haja uma colisão onde quem tá colidindo é um trigger (checkbox "IsTrigger" marcado)
    protected void OnTriggerEnter2D(Collider2D other)
    {
        // Estou verificando todo o dano (recebido e dado pelo inimigo) nesse método, comparando os diferentes valores de Tag.
        if (other.CompareTag("Attack"))
        {
            int damage = other.GetComponentInParent<Attack>().attackPower;
            DamageEnemy(damage);
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
    }

    private IEnumerator DealDelayedDamage(Collider2D collider)
    {
        yield return new WaitForSeconds(0.6f);
        if (collider.IsTouching(this.capsuleCollider))
        {
            collider.GetComponentInParent<Player>().DamagePlayer();
        }
    }

    protected void Flip()
    {
        if (canFlip)
        {
            isFacingRight = !isFacingRight;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            speed *= -1;
        }
    }

    // Esse método vai ser rodado em paralelo a partir do método DamageEnemy.
    // Criando dessa forma com o IEnumerator, vai permitir que ele tenha manipulação de tempo sem interromper os outros processos
    // Pelo menos foi isso que entendi
    protected IEnumerator DamageEffect(bool needToFlip)
    {
        if (needToFlip)
        {
            Flip();
        }

        float currentSpeed = speed;
        speed *= -1;

        sprite.color = Color.red;
        body.AddForce(new Vector2(0f, 300f));

        yield return new WaitForSeconds(0.3f);

        speed = currentSpeed;

        sprite.color = Color.white;
    }

    protected void DamageEnemy(int damage)
    {
        health -= damage;
        if (player.IsFacingRight == this.isFacingRight)
        {
            StartCoroutine(DamageEffect(true));
        }
        else
        {
            StartCoroutine(DamageEffect(false));
        }        

        if (health < 1)
        {
            if (lootDropper != null)
            {
                lootDropper.DropLoot(lootTable.GetLootFromLootTable());
            }            
            Destroy(gameObject);
        }
    }

}
