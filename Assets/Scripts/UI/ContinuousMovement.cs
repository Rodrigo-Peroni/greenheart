using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContinuousMovement : MonoBehaviour
{
    public float speed;
    public RawImage image;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Rect rect = image.uvRect;
        rect.x += speed;
        image.uvRect = rect;        
    }
}
