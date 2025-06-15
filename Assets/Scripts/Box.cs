using System.Collections;
using UnityEngine;

public class Box : MonoBehaviour
{
    public GameObject UpperRightCornerPiece;
    public GameObject UpperLeftCornerPiece;
    public GameObject BottomRightCornerPiece;
    public GameObject BottomLeftCornerPiece;

    public AudioClip breakSound;    

    private SpriteRenderer boxSprite;
    private SpriteRenderer upperRightSprite;
    private SpriteRenderer bottomRightSprite;
    private SpriteRenderer upperLeftSprite;
    private SpriteRenderer bottomLeftSprite;

    private Rigidbody2D upperRightRgbd;
    private Rigidbody2D bottomRightRgbd;
    private Rigidbody2D upperLeftRgbd;
    private Rigidbody2D bottomLeftRgbd;

    // Start is called before the first frame update
    void Start()
    {
        boxSprite = gameObject.GetComponent<SpriteRenderer>();
        upperRightSprite = UpperRightCornerPiece.GetComponent<SpriteRenderer>();
        bottomRightSprite = BottomRightCornerPiece.GetComponent<SpriteRenderer>();
        upperLeftSprite = UpperLeftCornerPiece.GetComponent<SpriteRenderer>();
        bottomLeftSprite = BottomLeftCornerPiece.GetComponent<SpriteRenderer>();

        upperRightRgbd = UpperRightCornerPiece.GetComponent<Rigidbody2D>();
        bottomRightRgbd = BottomRightCornerPiece.GetComponent<Rigidbody2D>();
        upperLeftRgbd = UpperLeftCornerPiece.GetComponent<Rigidbody2D>();
        bottomLeftRgbd = BottomLeftCornerPiece.GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Player player = col.gameObject.GetComponent<Player>();
            player.DamagePlayer();
            StartCoroutine(BoxHitAnimation());
        }
        else if (col.gameObject.CompareTag("Ground"))
        {                
            if (boxSprite.isVisible)
            {
                SoundManager.instance.PlayLowSound(breakSound);                
            }
            StartCoroutine(BoxHitAnimation());            
        }
        else if (col.gameObject.CompareTag("KingPig"))
        {
            KingPig kingPig = col.gameObject.GetComponent<KingPig>();
            kingPig.ReceiveBoxDamage();
            StartCoroutine(BoxHitAnimation());
        }
    }

    IEnumerator BoxHitAnimation()
    {
        boxSprite.enabled = false;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;

        UpperRightCornerPiece.SetActive(true);
        UpperLeftCornerPiece.SetActive(true);
        BottomRightCornerPiece.SetActive(true);
        BottomLeftCornerPiece.SetActive(true);

        upperRightRgbd.AddForce(new Vector2(200f, 200f));
        upperLeftRgbd.AddForce(new Vector2(-200f, 200f));
        bottomRightRgbd.AddForce(new Vector2(100f, 100f));
        bottomLeftRgbd.AddForce(new Vector2(-100f, 100f));

        yield return new WaitForSeconds(2f);

        Destroy(UpperRightCornerPiece);
        Destroy(UpperLeftCornerPiece);
        Destroy(BottomRightCornerPiece);
        Destroy(BottomLeftCornerPiece);
        Destroy(gameObject);
    }
}
