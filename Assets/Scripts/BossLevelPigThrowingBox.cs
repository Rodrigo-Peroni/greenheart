using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLevelPigThrowingBox : BoxPig
{
    private new void Update()
    {
        if (BossLevelManager.instance.IsTrapdoorClosed)
        {
            if (BossLevelManager.instance.IsBoxesTurn)
            {
                base.Update();
            }
            else
            {
                animator.SetTrigger("Stop");
            }
        }
    }    

    private new void FixedUpdate()
    {
        if (BossLevelManager.instance.IsTrapdoorClosed)
        {
            if (BossLevelManager.instance.IsBoxesTurn)
            {
                base.FixedUpdate();
            }
        }
        else
        {
            if ((directionFactor == -1 && isLookingRight) || (directionFactor == 1 && !isLookingRight))
            {
                Flip();
            }
            rgbd.velocity = new Vector2(3.0f * directionFactor, rgbd.velocity.y);
        }
    }

    private new void OnTriggerEnter2D(Collider2D other)
    {
        if (BossLevelManager.instance.IsTrapdoorClosed)
        {
            base.OnTriggerEnter2D(other);
        }
        else
        {
            if (other.CompareTag("LevelTriggers"))
            {
                Destroy(gameObject);
            }
        }
    }
}
