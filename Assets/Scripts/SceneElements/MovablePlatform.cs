using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovablePlatform : MonoBehaviour
{       
    public Vector3 velocity;    

    private GameObject player;
    private Player playerScript;
    private Collider2D platformCollider;
    private Collider2D playerCollider;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<Player>();

        playerCollider = player.GetComponent<CircleCollider2D>();
        platformCollider = gameObject.GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        // Fiz dessa maneira pq a maneira "convencional" não estava funcionando
        if (platformCollider.IsTouching(playerCollider))
        {
            playerCollider.transform.SetParent(transform);
        }
        else
        {
            if (!playerScript.IsInMovablePlatform)
            {
                playerCollider.transform.SetParent(null);
            }            
        }
    }

    void FixedUpdate()
    {        
        transform.position += (velocity * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlatformDelimiter"))
        {
            if (velocity.x != 0)
            {
                velocity.x *= -1;
            }
            else
            {                
                velocity.y *= -1;                
            }
        }    
    }

    // Normalmente esse approach abaixo funcionaria. Mas como tem esse collider locão no Player,
    // aí não tá rolando. Pelo visto quem tá tocando a plataforma é o circle collider.

    //private void OnCollisionEnter2D(Collision2D other)
    //{
    //    Debug.Log("ENTROU Collision");
    //    if (other.gameObject.CompareTag("Player"))
    //    {
    //        Debug.Log("Player entrou");
    //        other.collider.transform.SetParent(transform);
    //    }
    //}

    //private void OnCollisionExit2D(Collision2D other)
    //{
    //    Debug.Log("SAIU Collision");
    //    if (other.gameObject.CompareTag("Player"))
    //    {
    //        Debug.Log("Player saiu");
    //        other.collider.transform.SetParent(null);
    //    }
    //}
}
