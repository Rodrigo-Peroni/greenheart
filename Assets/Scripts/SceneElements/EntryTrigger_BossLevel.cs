using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryTrigger_BossLevel : MonoBehaviour
{
    public GameObject drawbridge;
    public CameraController mainCamera;
    public Transform playerWaitPoint;

    private Quaternion startRotation;
    private Quaternion endRotation;
    private float rotationProgress = -1;
    private Player player;
    private PlayerManager playerManager;
    private PathFollower cameraPathFollower;

    private void Start()
    {
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
        player = GameObject.Find("Player").GetComponent<Player>();
        cameraPathFollower = GetComponent<PathFollower>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!playerManager.HasAlreadySeenBossCamera)
            {
                StartCoroutine(MoveCamera());

                playerManager.HasAlreadySeenBossCamera = true;
            }

            StartRotatingDrawbridge(-90f);
            BossLevelManager.instance.StartBossLevel();
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
        if (rotationProgress < 1 && rotationProgress >= 0)
        {
            rotationProgress += Time.deltaTime * 4;

            // Here we assign the interpolated rotation to transform.rotation
            // It will range from startRotation (rotationProgress == 0) to endRotation (rotationProgress >= 1)
            drawbridge.transform.rotation = Quaternion.Lerp(startRotation, endRotation, rotationProgress);
        }
    }

    private IEnumerator MoveCamera()
    {
        yield return new WaitForSeconds(0.5f);
        player.speed = 0f;
        player.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
        player.GoToIdleState();
        yield return null;
        player.DisableInputs();

        yield return new WaitForSeconds(1.5f);
        mainCamera.IsControlledExternally = true;
        cameraPathFollower.StartMovement();        
        yield return new WaitForSeconds(10.0f);
        
        mainCamera.IsControlledExternally = false;
        mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, -10f);
        player.speed = 5f;
        player.EnableInputs();
    }
}
