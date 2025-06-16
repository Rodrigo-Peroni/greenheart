using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script to define different behaviors of an item in case it is dropping from an enemy
/// or in case it's a static item that can be found in the game level.
/// </summary>
public class DroppableItem : MonoBehaviour
{
    public bool isDroppable = false;

    // Start is called before the first frame update
    void Start()
    {
        SetIsDroppable(isDroppable);
    }

    public void SetIsDroppable(bool isDroppable)
    {
        this.isDroppable = isDroppable;

        if (isDroppable)
        {
            Rigidbody2D rgbd = gameObject.GetComponent<Rigidbody2D>();
            rgbd.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        else
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
