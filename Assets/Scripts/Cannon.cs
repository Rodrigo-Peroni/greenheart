using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    public float fireRate;
    public CannonBall cannonBall;
    public GameObject controllingPig;
    public Transform spawnPoint;

    public AudioClip shootSound;

    private bool isFacingRight;

    private Animator animator;
    private SpriteRenderer sprite;

    private float nextShoot = 0;
    private bool hasControllingPig = true;

    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
        if (transform.localScale.x < 0)
        {
            isFacingRight = true;
        }
        else
        {
            isFacingRight = false;
        }
    }

    // Update is called once per frame
    protected void Update()
    {
        if (controllingPig == null)
        {
            hasControllingPig = false;
        }

        if (Time.time > nextShoot && hasControllingPig)
        {
            nextShoot = Time.time + fireRate;
            animator.SetTrigger("Shoot");
            CannonBall newCannonBall = Instantiate(cannonBall, spawnPoint.position, spawnPoint.rotation);
            newCannonBall.IsFacingRight = this.isFacingRight;
            newCannonBall.UpdateDirection();
            if (sprite.isVisible)
            {
                SoundManager.instance.PlayLowestSound(shootSound);
            }
        }
    }
}
