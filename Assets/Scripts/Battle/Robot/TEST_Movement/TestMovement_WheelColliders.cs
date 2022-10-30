using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovement_WheelColliders : MonoBehaviour
{
    public KeyCode forward;
    public KeyCode backward;

    private void FixedUpdate()
    {
        // Store last "held" angular velocity
        //if (GetComponent<Rigidbody>().angularVelocity)
        if (Input.GetKey(forward))
        {
            Debug.Log("Forward pressed");
            GetComponent<WheelCollider>().motorTorque = 15;
        }
        if (Input.GetKeyUp(forward))
        {
            Debug.Log("Forward released");
            GetComponent<WheelCollider>().motorTorque = 0;
        }

        if (Input.GetKey(backward))
        {
            Debug.Log("Backward pressed");
            GetComponent<WheelCollider>().motorTorque = -15;
        }
        if (Input.GetKeyUp(backward))
        {
            Debug.Log("Backward released");
            GetComponent<WheelCollider>().motorTorque = 0;
        }
    }
}
