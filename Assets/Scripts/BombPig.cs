using Assets.Utils.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombPig : MonoBehaviour
{
    public Transform bombSpawnPoint;
    public GameObject bombPrefab;

    private SpriteRenderer sprite;
    private Animator animator;
    private LootDropper lootDropper;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        lootDropper = GetComponent<LootDropper>();
    }

    public void ThrowBomb()
    {
        animator.SetTrigger("ThrowBomb");
        Invoke("InstantiateBomb", 0.3f);
    }

    private void InstantiateBomb()
    {
        Instantiate(bombPrefab, bombSpawnPoint);
    }

    public void PlayDamageEffect()
    {
        StartCoroutine(DamageEffect());
    }

    IEnumerator DamageEffect()
    {
        sprite.color = Color.red;

        yield return new WaitForSeconds(0.2f);

        sprite.color = Color.white;
    }

    public void DropHeart()
    {
        lootDropper.DropLoot(new LootItem(ItemCode.Heart, 1));
    }
}
