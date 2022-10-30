using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPostionsForParts : MonoBehaviour
{
    public Dictionary<string, Vector3> cameraTransformPosition = new Dictionary<string, Vector3>();
    public Dictionary<string, Vector3> cameraTransformRotation = new Dictionary<string, Vector3>();

    void Awake()
    {
        cameraTransformPosition.Add("Axe", new Vector3(6f, 0f, 2.5f));
        cameraTransformRotation.Add("Axe", new Vector3(0f, -90f, 0f));

        cameraTransformPosition.Add("Bomber", new Vector3(4f, 0f, 1.5f));
        cameraTransformRotation.Add("Bomber", new Vector3(0f, -90f, 0f));

        cameraTransformPosition.Add("BubbleBlaster", new Vector3(5f, 0f, 1.5f));
        cameraTransformRotation.Add("BubbleBlaster", new Vector3(0f, -90f, 0f));

        cameraTransformPosition.Add("BurstLaser", new Vector3(5f, 0f, 1.5f));
        cameraTransformRotation.Add("BurstLaser", new Vector3(0f, -90f, 0f));

        cameraTransformPosition.Add("Drill", new Vector3(6f, 0f, 1.5f));
        cameraTransformRotation.Add("Drill", new Vector3(0f, -90, 0f));

        cameraTransformPosition.Add("Fan_Finished", new Vector3(4f, 0f, 1.5f));
        cameraTransformRotation.Add("Fan_Finished", new Vector3(0f, -90f, 0f));

        cameraTransformPosition.Add("HandShield", new Vector3(-0.2f, 1.2f, 8f));
        cameraTransformRotation.Add("HandShield", new Vector3(0f, 180f, 0f));

        cameraTransformPosition.Add("Horn", new Vector3(5f, 0f, 2f));
        cameraTransformRotation.Add("Horn", new Vector3(0f, -90f, 0f));

        cameraTransformPosition.Add("PaintBlaster", new Vector3(7f, 0.5f, 2.5f));
        cameraTransformRotation.Add("PaintBlaster", new Vector3(0f, -90f, 0f));

        cameraTransformPosition.Add("PipeOilSlick", new Vector3(-0.6f, 0f, 6f));
        cameraTransformRotation.Add("PipeOilSlick", new Vector3(0f, 180f, 0f));

        cameraTransformPosition.Add("Turret 2 Axis", new Vector3(4f, 0f, 1f));
        cameraTransformRotation.Add("Turret 2 Axis", new Vector3(0f, -90f, 0f));
    }
}
