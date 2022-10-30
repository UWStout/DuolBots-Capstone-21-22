using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 0f;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate((Vector3.right + Vector3.one).normalized * Time.deltaTime * rotationSpeed);
    }


}
