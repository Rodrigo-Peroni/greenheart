using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigTrigger_BossLevel : MonoBehaviour
{
    public GameObject drawbridge;
    public float rotation;

    private Quaternion startRotation;
    private Quaternion endRotation;
    private float rotationProgress = -1;

    private int destroyedPigs = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            destroyedPigs += 1;
        }
    }

    // Call this to start the rotation
    void StartRotatingDrawbridge(float zPosition)
    {
        // Here we cache the starting and target rotations
        startRotation = drawbridge.transform.rotation;
        endRotation = Quaternion.Euler(drawbridge.transform.rotation.eulerAngles.x, drawbridge.transform.rotation.eulerAngles.y, zPosition);

        // This starts the rotation, but you can use a boolean flag if it's clearer for you
        rotationProgress = 0;
    }

    void Update()
    {
        if (destroyedPigs >= 5)
        {
            StartRotatingDrawbridge(rotation);
        }

        if (rotationProgress < 1 && rotationProgress >= 0)
        {
            rotationProgress += Time.deltaTime * 4;

            // Here we assign the interpolated rotation to transform.rotation
            // It will range from startRotation (rotationProgress == 0) to endRotation (rotationProgress >= 1)
            drawbridge.transform.rotation = Quaternion.Lerp(startRotation, endRotation, rotationProgress);
        }
    }

    public void RestartRotation()
    {
        rotationProgress = 0;
    }
}
