using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DuolBots;
// Original Author - Ben Lussman

/// <summary>
/// PartImages is used to take images of parts of our robots which we use throughout our UI systems
/// Pre Conditions: Need a list of parts that we need images of, and a camera to take those iamges without disturbing the main camera
/// </summary>
public static class PartImages
{
    private static Vector3 m_offset = new Vector3(0,0, -5);

    private static Camera m_Camera = null;


    public static void MakeAnimation(ref GameObject temp_Part,int frames)
    {
        /*
        m_Camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        Reset = m_Camera;
        PositionCamera(ref temp_Part);
        //m_Camera.orthographic = true;
        int temp_partLayer = temp_Part.layer;
        Quaternion temp_rotation = temp_Part.transform.rotation;

        temp_Part.layer = 5;
        FilePaths.MAKEDIRECTORY(FilePaths.TEMPORARYFOLDER);
        for(int i =0; i < frames; i++)
        {
            temp_Part.transform.rotation = Quaternion.Euler(0, (360/frames)*i, 0);
            SingleImage(ref temp_Part, FilePaths.GETPNGFORTEMPORARYFOLDER(temp_Part.name.Substring(0, temp_Part.name.Length - 7) + i));
        }

        //FilePaths.DELETETEMPORARYFOLDER();
        FilePaths.REFRESHASSETDATABASE();
        */
    }

    public static void TakeImages(ref GameObject temp_Part)
    {
        //get main camera and handle reseting the camera
        GameObject o = new GameObject();
        o.AddComponent<Camera>();
        m_Camera = o.GetComponent<Camera>();
        PositionCamera(ref temp_Part);
        //m_Camera.orthographic = true;
        temp_Part.layer = 5;
        //make a new black material for the locked image
        Material temp_Black = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        temp_Black.color = Color.black;

        //take an image
        SingleImage(ref temp_Part, FilePaths.GETPARTSTILLPATH(temp_Part.name.Substring(0,temp_Part.name.Length-7)));

        // for each material on the object set it to the black material 
        List<Material> temp_mats = new List<Material>();
        foreach(Renderer r in temp_Part.GetComponentsInChildren<Renderer>())
        {
            temp_mats.Add(r.sharedMaterial);
            r.material = temp_Black;
        }

        //take an image
        SingleImage(ref temp_Part, FilePaths.GETPARTSTILLPATH(temp_Part.name.Substring(0, temp_Part.name.Length - 7) + "Locked"));

        Object.DestroyImmediate(o);
        //refresh database
        FilePaths.REFRESHASSETDATABASE();
    }
    // Late Update instead of Update since this must be accomplished after rendering 
    private static void SingleImage(ref GameObject temp_Part, string ImagePath)
    {
        // Cycle through each part in the list to see if we are missing an image of it
        //foreach (GameObject temp_Part in m_parts)
        if (temp_Part!=null && !System.IO.File.Exists(ImagePath))
        {

            // Prep for camera for image
            RenderTexture temp_texture = new RenderTexture(256, 256, 24, RenderTextureFormat.ARGB32);
            temp_texture.Create();
            m_Camera.targetTexture = temp_texture;
            m_Camera.clearFlags = CameraClearFlags.SolidColor;
            m_Camera.backgroundColor = Color.clear;
            m_Camera.Render();
            RenderTexture.active = m_Camera.targetTexture;

            // Convert RenderTexture to Texture2D in preperation for export
            Texture2D temp_to_PNG = new Texture2D(m_Camera.targetTexture.width, m_Camera.targetTexture.height, TextureFormat.ARGB32, false);
            temp_to_PNG.ReadPixels(new Rect(0, 0, m_Camera.targetTexture.width, m_Camera.targetTexture.height), 0, 0);
            temp_to_PNG.Apply();

            //Encode to a PNG then delete the textures to prevent memory leaks
            byte[] bytes = temp_to_PNG.EncodeToPNG();

            //Write out the PNG
            System.IO.File.WriteAllBytes(ImagePath, bytes);

            // Wrap Up function
            RenderTexture.active = null;
        }
    }

    /// <summary>
    /// Get the max bounds of a model
    /// </summary>
    /// <param name="g"></param>
    /// <returns>Bounds</returns>
    private static Bounds GetMaxBounds(ref GameObject g)
    {
        Bounds b = new Bounds(g.transform.position, Vector3.zero);
        foreach (Renderer r in g.GetComponentsInChildren<Renderer>())
        {
            b.Encapsulate(r.bounds);
        }
        return b;
    }

    /// <summary>
    /// Moves the camera to a position relative to the bounding box of a referenced game object
    /// </summary>
    /// <param name="temp_Part"></param>
    private static void PositionCamera(ref GameObject temp_Part)
    {
        Bounds temp_bounds;

        temp_bounds = Encap(temp_Part.transform, temp_Part.GetComponentInChildren<Renderer>().bounds);


        temp_Part.transform.position -= temp_bounds.center;
        float modBy = 256/Mathf.Max(temp_bounds.size.x, temp_bounds.size.y);
        temp_Part.transform.localScale = new Vector3(modBy,modBy,modBy);

        m_Camera.transform.position = temp_bounds.center * modBy - new Vector3(0, 0, m_Camera.farClipPlane * .3333f);
           
    }

    private static Bounds Encap(Transform parent, Bounds blocker)
    {
        foreach (Transform child in parent)
        {
            Renderer renderer = child.GetComponent<Renderer>();
            if (renderer != null)
            {
                blocker.Encapsulate(renderer.bounds);
            }
            blocker = Encap(child, blocker);
        }
        return blocker;
    }

    /// <summary>
    /// Given a Image path it loads the image as a Texture2D
    /// </summary>
    /// <param name="ImagePath"></param>
    /// <returns>Texture2D</returns>
    public static Texture2D LoadTexture(string ImagePath)
    {
        return AssetDatabase.LoadAssetAtPath<Texture2D>(ImagePath.Substring(ImagePath.LastIndexOf("Assets")));
    }
}

