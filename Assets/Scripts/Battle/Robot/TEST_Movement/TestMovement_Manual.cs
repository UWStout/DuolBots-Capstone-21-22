using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovement_Manual : MonoBehaviour
{
    public float TopSpeed = 15;
    public float Acceleration = 1.5f;
    public float RotAccel = 0.5f;

    private int isLeftMoving = 0;
    private int isRightMoving = 0;

    public GameObject LeftWheel;
    public GameObject RightWheel;

    private void Update()
    {
        // Left Wheel
        if (Input.GetKeyDown(KeyCode.W))
        {
            isLeftMoving += 1;
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            isLeftMoving -= 1;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            isLeftMoving -= 1;
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            isLeftMoving += 1;
        }

        // Right Wheel
        if (Input.GetKeyDown(KeyCode.I))
        {
            isRightMoving += 1;
        }
        else if (Input.GetKeyUp(KeyCode.I))
        {
            isRightMoving -= 1;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            isRightMoving -= 1;
        }
        else if (Input.GetKeyUp(KeyCode.K))
        {
            isRightMoving += 1;
        }

        // Interpret movement
        Vector3 directionalMovement = Vector3.Project(GetComponentInParent<Rigidbody>().velocity, transform.forward);

        // Moving forward
        if (isLeftMoving == 1 && isRightMoving == 1)
        {
            if (directionalMovement.magnitude < TopSpeed)
            {
                GetComponentInParent<Rigidbody>().AddRelativeForce(new Vector3(0, 0, Acceleration), ForceMode.Acceleration);
            }
        }
        // Moving backward
        if (isLeftMoving == -1 && isRightMoving == -1)
        {
            if (directionalMovement.magnitude < TopSpeed)
            {
                GetComponentInParent<Rigidbody>().AddRelativeForce(new Vector3(0, 0, -Acceleration), ForceMode.Acceleration);
            }
        }

        /*
        float dRot = 0;
        // Rotation
        if (isLeftMoving == 1 && isRightMoving == 0)
        {
            dRot += RotAccel;
        }
        else if (isLeftMoving == -1)
        {
            dRot -= RotAccel;
        }

        if (isRightMoving == 1)
        {
            dRot -= RotAccel;
        }
        else if (isRightMoving == -1)
        {
            dRot += RotAccel;
        }

        GetComponentInParent<Transform>().Rotate(Vector3.up, dRot);*/

        if (isLeftMoving == 1 && isRightMoving == 0)
        {
            GetComponentInParent<Transform>().RotateAround(RightWheel.transform.position, Vector3.up, RotAccel);
        }
        else if (isLeftMoving == 1 && isRightMoving == -1)
        {
            GetComponentInParent<Transform>().Rotate(Vector3.up, RotAccel);
        }
        else if (isLeftMoving == -1 && isRightMoving == 0)
        {
            GetComponentInParent<Transform>().RotateAround(RightWheel.transform.position, Vector3.up, -RotAccel);
        }
        else if (isRightMoving == 1 && isLeftMoving == 0)
        {
            GetComponentInParent<Transform>().RotateAround(LeftWheel.transform.position, Vector3.up, -RotAccel);
        }
        else if (isRightMoving == 1 && isLeftMoving == -1)
        {
            GetComponentInParent<Transform>().Rotate(Vector3.up, -RotAccel);
        }
        else if (isRightMoving == -1 && isLeftMoving == 0)
        {
            GetComponentInParent<Transform>().RotateAround(LeftWheel.transform.position, Vector3.up, RotAccel);
        }
    }
}
