using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenLevelGreenHeart : MonoBehaviour
{
    public float verticalSpeed;

    private SpriteRenderer sprite;
    private Rigidbody2D rgbd;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        rgbd = GetComponent<Rigidbody2D>();

        StartCoroutine(SpriteFadeIn());
    }

    private void FixedUpdate()
    {
        rgbd.velocity = new Vector2(rgbd.velocity.x, verticalSpeed);
    }

    private IEnumerator SpriteFadeIn()
    {
        Color newColor = sprite.color;

        while (sprite.color.a <= 0.8f)
        {
            newColor.a = newColor.a + 0.01f;
            sprite.color = newColor;
            yield return new WaitForSecondsRealtime(0.035f);
        }
    }
}
