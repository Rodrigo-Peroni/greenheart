using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    private Vector2 velocity;
    private Transform playerTransform;

    public bool IsControlledExternally { get; set; } = false;

    public float smoothTimeX;
    public float smoothTimeY;

    private float shakeTimer;
    private float shakeAmount;

    private float yOffset = 0f;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();        
    }

    // Update is called once per frame
    void Update()
    {
        if (shakeTimer >= 0f)
        {
            Vector2 shakePosition = Random.insideUnitCircle * shakeAmount;
            transform.position = new Vector3(
                transform.position.x + shakePosition.x,
                transform.position.y + shakePosition.y,
                transform.position.z);

            shakeTimer -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        // Verificando se o Player não é Null pq ele podeser null caso dermos um destroy nele na hora da morte
        if (playerTransform != null && !IsControlledExternally)
        {
            float posX = Mathf.SmoothDamp(transform.position.x, playerTransform.position.x, ref velocity.x, smoothTimeX);
            float posY = Mathf.SmoothDamp(transform.position.y, playerTransform.position.y + yOffset, ref velocity.y, smoothTimeY);

            transform.position = new Vector3(posX, posY, transform.position.z);
        }
    }

    public void ShakeCamera(float time, float shakeStrength)
    {
        shakeTimer = time;
        shakeAmount = shakeStrength;
    }

    public void SetYOffset(float yOffset)
    {
        this.yOffset = yOffset;
    }
}
