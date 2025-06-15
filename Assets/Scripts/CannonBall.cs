using System.Collections;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public float speed;

    private Rigidbody2D rgbd;
    private Animator animator;
    private SpriteRenderer sprite;
    public bool IsFacingRight { get; set; }

    public AudioClip explosionSound;

    private bool canMove = false;

    // Start is called before the first frame update
    void Start()
    {
        rgbd = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
    }
    
    public void UpdateDirection()
    {
        if (!IsFacingRight)
        {
            speed = -speed;
        }
        else
        {            
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

        canMove = true;
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            rgbd.velocity = new Vector2(speed, rgbd.velocity.y);
        }        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponentInParent<Player>().DamagePlayer();
            StartCoroutine(CannonBallHitPlayer(other.gameObject.transform));
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {        
        StartCoroutine(CannonBallHitWall());
    }

    IEnumerator CannonBallHitPlayer(Transform playerTransform)
    {        
        speed = 0;
        transform.position = playerTransform.position;
        animator.SetTrigger("Hit");
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);       
    }

    IEnumerator CannonBallHitWall()
    {        
        animator.SetTrigger("Hit");
        speed = 0;
        if (sprite.isVisible)
        {
            SoundManager.instance.PlayLowSound(explosionSound);
        }
        gameObject.GetComponent<CircleCollider2D>().enabled = false;
        yield return new WaitForSeconds(1f); ;
        Destroy(gameObject);
    }
}
