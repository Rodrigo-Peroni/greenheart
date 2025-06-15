using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diamonds : MonoBehaviour
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
            rgbd.constraints = RigidbodyConstraints2D.FreezeRotation;// | RigidbodyConstraints2D.FreezePositionX;
        }
        else
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
