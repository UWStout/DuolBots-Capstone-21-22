using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovement_ForceTorque : MonoBehaviour
{
    private void FixedUpdate()
    {
        float speed = GetComponent<Rigidbody>().velocity.magnitude;
        // Left Wheel
        if (Input.GetKey(KeyCode.W))
        {
            GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(0, 20f * (speed + 1), 0), ForceMode.Acceleration);
            GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 0, 12f), ForceMode.Acceleration);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(0, -20f * (speed + 1), 0), ForceMode.Acceleration);
            GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 0, -12f), ForceMode.Acceleration);
        }

        // Right Wheel
        if (Input.GetKey(KeyCode.I))
        {
            GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(0, -20f * (speed + 1), 0), ForceMode.Acceleration);
            GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 0, 12f), ForceMode.Acceleration);
        }
        else if (Input.GetKey(KeyCode.K))
        {
            GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(0, 20f * (speed + 1), 0), ForceMode.Acceleration);
            GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 0, -12f), ForceMode.Acceleration);
        }
    }
}
