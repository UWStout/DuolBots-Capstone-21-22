using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageSceneScript : MonoBehaviour
{

    [SerializeField]
    public List<GameObject> Slots = new List<GameObject>();

    public Color[] colors = { new Color(.1f,.1f,.4f), Color.cyan, Color.green, Color.magenta, Color.red, Color.yellow};

    public int colorindex = -1;

    public void TakePicture()
    {
        ///StartCoroutine(PictureTime());
        ///StartCoroutine(FullBotPic());
        StartCoroutine(PartGray());
    }

    public IEnumerator FullBotPic()
    {
        Material temp_White = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        temp_White.color = new Color(.8f, .8f, .8f);
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            Material[] mats = new Material[r.materials.Length];
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = temp_White;
            }
            r.materials = mats;
        }
        yield return new WaitForEndOfFrame();

        foreach (GameObject go in Slots)
        {
            Material temp_Other = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            temp_Other.color = colors[colorindex];
            foreach (Renderer r in go.GetComponentsInChildren<Renderer>())
            {
                Material[] mats = new Material[r.materials.Length];
                for (int i = 0; i < mats.Length; i++)
                {
                    mats[i] = temp_Other;
                }
                r.materials = mats;
            }
        }


        yield return new WaitForEndOfFrame();
        Helper($"{Random.Range(100000, 999999)}");
    }

    public IEnumerator PartGray()
    {
        Material temp_White = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        temp_White.color = new Color(.8f, .8f, .8f);
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            //Material[] mats = new Material[r.materials.Length];
            //for (int i = 0; i < mats.Length; i++)
            //{
            //    mats[i] = temp_White;
            //}
            r.materials[0] = temp_White;
        }
        yield return new WaitForEndOfFrame();
        Helper($"{Random.Range(100000, 999999)}");
    }

    public IEnumerator PictureTime()
    {
        Helper(transform.GetChild(0).name);
        //make a new black material for the locked image

        yield return new WaitForEndOfFrame();

        Material temp_Black = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        temp_Black.color = Color.black;
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            Material[] mats = new Material[r.materials.Length];
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = temp_Black;
            }
            r.materials = mats;
        }
        Helper($"{transform.GetChild(0).name}Locked");
    }



    private void Helper(string t)
    {
        Camera m_Camera = Camera.main;
        // Prep for camera for image
        RenderTexture temp_texture = new RenderTexture(1024, 1024, 24, RenderTextureFormat.ARGB32);
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
        string s = "Image";
        System.IO.File.WriteAllBytes(FilePaths.GETPARTSTILLPATH(s + t), bytes);

        // Wrap Up function
        RenderTexture.active = null;

        FilePaths.REFRESHASSETDATABASE();
    }
}
