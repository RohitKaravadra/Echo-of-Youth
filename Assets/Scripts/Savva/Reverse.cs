using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TransformSnapshot
{
    public Vector2 position;
    public Quaternion rotation;
}


public class Reverse : MonoBehaviour
{
    public int maxHistory = 1000;
    public float recordInterval = 0.05f;
    public Rigidbody2D rb;

    private bool isReversing = false;
    private float timeSinceLastRecord = 0f;
    private float gravityScale;


    private List<TransformSnapshot> positionHistory = new List<TransformSnapshot>();

    

    // TODO: move this to FixedUpdate? Not sure.
    void Update()
    {
        timeSinceLastRecord += Time.deltaTime;
        if (timeSinceLastRecord > recordInterval)
        {
            timeSinceLastRecord = 0f;

            if (isReversing)
            {
                ReversePosition();
            }
            else
            {
                RecordPosition();
            }
        }
    }

    private void RecordPosition()
    {
        // Remove oldest position.
        if (positionHistory.Count >= maxHistory)
        {
            positionHistory.RemoveAt(0);
        }

        // Save new position snapshot.
        TransformSnapshot newElement = new TransformSnapshot();
        newElement.position = transform.position;
        newElement.rotation = transform.rotation;

        if (positionHistory.Count == 0 ||
            (!newElement.Equals(positionHistory[^1]) || rb.Equals(DragAll.GetDraggingRB())))
        {
            positionHistory.Add(newElement);
        }
    }

    private void ReversePosition()
    {
        if (positionHistory.Count > 0)
        {
            // Reverse position.
            transform.position = positionHistory[^1].position;
            transform.rotation = positionHistory[^1].rotation;

            // Remove last element.
            positionHistory.RemoveAt(positionHistory.Count - 1);
        }
        else
        {
            isReversing = false;
        }
    }

    public void StartReversing()
    {
        gravityScale = rb.gravityScale;
        rb.gravityScale = 0;
        isReversing = true;
    }

    public void StopReversing()
    {
        rb.gravityScale = gravityScale;
        isReversing = false;
    }
}
