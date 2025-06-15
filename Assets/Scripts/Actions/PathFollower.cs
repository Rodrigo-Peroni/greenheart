using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    public GameObject pathObject;
    public GameObject objectToMove;
    public float moveSpeed;

    private float timer;
    private PathNode[] pathNode;
    private int currentNode = 0;
    private Vector3 currentPositionHolder;
    private Vector3 startPosition;
    private bool isMovementStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        ResetNodeTree();
    }

    private void CheckNode()
    {
        timer = 0;
        startPosition = objectToMove.transform.position;
        currentPositionHolder = pathNode[currentNode].transform.position;      
    }

    public void StartMovement()
    {
        startPosition = objectToMove.transform.position;
        isMovementStarted = true;        
    }

    public void ResetNodeTree()
    {
        pathNode = pathObject.GetComponentsInChildren<PathNode>();
        CheckNode();
    }

    public void StopMovement()
    {
        isMovementStarted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMovementStarted)
        {
            timer += Time.deltaTime * moveSpeed;
            if (objectToMove.transform.position != currentPositionHolder)
            {
                objectToMove.transform.position = Vector3.Lerp(startPosition, currentPositionHolder, timer);
            }
            else
            {
                if (currentNode < pathNode.Length - 1)
                {
                    currentNode++;                    
                }
                else
                {
                    isMovementStarted = false;
                    currentNode = 0;
                }
                CheckNode();
            }
        }
    }
}
