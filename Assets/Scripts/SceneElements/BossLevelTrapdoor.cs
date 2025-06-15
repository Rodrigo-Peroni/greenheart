using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLevelTrapdoor : MonoBehaviour
{
    public Vector3 finalPosition;
    public GameObject FallRouteLeft;
    public GameObject FallRouteRight;
    public BombPig bombPig;

    private SpriteRenderer sprite;
    private PathFollower pathFollower;
    private Player player;

    public int health;
    private bool needsToFlip;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        pathFollower = GetComponent<PathFollower>();
        player = GameObject.Find("Player").GetComponent<Player>();

        pathFollower.pathObject = FallRouteLeft;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DamageTrapdoor(int damage)
    {        
        health -= damage;

        if (health <= 0)
        {
            BossLevelManager.instance.DestroyTrapdoor();
            //bombPig.DropHeart();
            Destroy(bombPig.gameObject);
            Destroy(gameObject);
        }
        else
        {
            player.DisableInputs();
            SetFallDirection();                      
            StartCoroutine(DamageEffect());
            StartCoroutine(PigGetHitAndThrowBomb());
            StartCoroutine(PlayerFall());          
        }        
    }

    private void SetFallDirection()
    {
        float distanceToLeft = Vector3.Distance(player.transform.position, FallRouteLeft.transform.position);
        float distanceToRight = Vector3.Distance(player.transform.position, FallRouteRight.transform.position);

        if (distanceToLeft < distanceToRight)
        {
            pathFollower.pathObject = FallRouteLeft;
            needsToFlip = player.IsFacingRight? false : true;
        }
        else
        {
            pathFollower.pathObject = FallRouteRight;
            needsToFlip = player.IsFacingRight ? true : false;
        }
        pathFollower.ResetNodeTree();
    }

    IEnumerator PigGetHitAndThrowBomb()
    {
        bombPig.PlayDamageEffect();
        yield return new WaitForSeconds(0.1f);
        bombPig.ThrowBomb();
    }

    IEnumerator PlayerFall()
    {
        yield return new WaitForSeconds(0.1f);
        player.GoToIdleState();
        yield return new WaitForSeconds(0.6f);
        if (needsToFlip)
        {
            player.Flip();
        }
        StartCoroutine(player.HitAndFall());
        pathFollower.StartMovement();
    }

    IEnumerator DamageEffect()
    {
        sprite.color = Color.red;

        yield return new WaitForSeconds(0.2f);

        sprite.color = Color.white;
    }
}
