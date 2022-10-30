using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] float rotate = 1f;

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(-Vector3.up * rotate * Time.deltaTime);
    }
}
