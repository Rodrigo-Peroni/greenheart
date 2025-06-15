using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLevelTrapdoorTrigger : MonoBehaviour
{
    private BossLevelTrapdoor trapdoor;

    // Start is called before the first frame update
    void Start()
    {
        trapdoor = GetComponentInParent<BossLevelTrapdoor>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Attack"))
        {
            int damage = other.GetComponentInParent<Attack>().attackPower;
            trapdoor.DamageTrapdoor(damage);
        }
    }    
}
