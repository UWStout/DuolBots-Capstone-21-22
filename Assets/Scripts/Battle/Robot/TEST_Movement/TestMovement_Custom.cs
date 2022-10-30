using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovement_Custom : MonoBehaviour
{
    public float topSpeed;
    public float acceleration;
    public float topRotVel;
    public float rotAccel;

    private int leftMove = 0;
    private int rightMove = 0;

    public GameObject leftWheel;
    public GameObject rightWheel;
    private Rigidbody body;

    private float dVel = 0.0f;
    private float dRot = 0.0f;
    private Vector3 rotateAround;

    private void Start()
    {
        body = GetComponent<Rigidbody>();
        leftMove = 0;
        rightMove = 0;
    }

    private void Update()
    {
        CaptureWheelMovement();
        
        // Determine rotation position
        float maxMove = Mathf.Max(Mathf.Abs(leftMove), Mathf.Abs(rightMove));
        Vector3 maxWheelPos = (maxMove == Mathf.Abs(leftMove)) ? leftWheel.transform.position : rightWheel.transform.position;
        float minMove = Mathf.Min(Mathf.Abs(leftMove), Mathf.Abs(rightMove));
        Vector3 minWheelPos = (minMove == Mathf.Abs(leftMove)) ? leftWheel.transform.position : rightWheel.transform.position;
        Vector3 center = Vector3.Lerp(leftWheel.transform.position, rightWheel.transform.position, 0.5f);

        if (Mathf.Abs(leftMove) == Mathf.Abs(rightMove))
        {
            rotateAround = center;
        }
        else
        {
            rotateAround = Vector3.Lerp(center, minWheelPos, (maxMove + minMove) / maxMove);
        }

        Vector3 forwardMovement = Vector3.Project(GetComponentInParent<Rigidbody>().velocity, transform.forward);
        Vector3 backwardMovement = -Vector3.Project(GetComponentInParent<Rigidbody>().velocity, transform.forward);
        // Moving forward
        if (leftMove > 0 && rightMove > 0)
        {
            //if (forwardMovement.magnitude < topSpeed)
            //{
                dVel = Mathf.Min(leftMove, rightMove) * acceleration;
            //}
            dRot = (leftMove - rightMove) * rotAccel;
        }
        else if (leftMove < 0 && rightMove < 0)
        {
            //if (backwardMovement.magnitude < topSpeed)
            //{
                dVel = Mathf.Max(leftMove, rightMove) * acceleration;
            //}
            dRot = (leftMove - rightMove) * rotAccel;
        }
        else
        {
            dRot = (leftMove - rightMove) * rotAccel;
        }
    }

    private void FixedUpdate()
    {
        body.transform.RotateAround(rotateAround, Vector3.up, dRot);
        body.velocity = Quaternion.Euler(0, dRot, 0) * body.velocity;
        body.velocity += transform.forward * dVel;
        dVel = 0.0f;
        dRot = 0.0f;
    }

    private void CaptureWheelMovement()
    {
        // Left Wheel
        if (Input.GetKeyDown(KeyCode.W))
        {
            leftMove += 1;
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            leftMove -= 1;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            leftMove -= 1;
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            leftMove += 1;
        }

        // Right Wheel
        if (Input.GetKeyDown(KeyCode.I))
        {
            rightMove += 1;
        }
        else if (Input.GetKeyUp(KeyCode.I))
        {
            rightMove -= 1;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            rightMove -= 1;
        }
        else if (Input.GetKeyUp(KeyCode.K))
        {
            rightMove += 1;
        }
        Debug.Log("Left: " + leftMove + ", Right: " + rightMove);
        // TODO: Once movement input has been switched to axes, round input to the nearest 0.05 (0.1) to reduce unwanted turning
    }
}
