using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public AudioClip explosionSound;

    private Rigidbody2D rgbd;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rgbd = gameObject.GetComponent<Rigidbody2D>();        
        animator = gameObject.GetComponent<Animator>();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            StartCoroutine(BombHitEffect());
        }        
    }

    IEnumerator BombHitEffect()
    {
        animator.SetTrigger("Hit");
        SoundManager.instance.PlayLowSound(explosionSound);        
        yield return new WaitForSeconds(0.8f); ;
        Destroy(gameObject);
    }
}
