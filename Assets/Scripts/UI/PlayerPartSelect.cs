using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPartSelect : MonoBehaviour
{
    [SerializeField] private GameObject m_parts = null;


    // Start is called before the first frame update
    void Start()
    {
        int temp_Index = 0;
        string path = Application.dataPath + "/GeneratedTextures";
        foreach (Transform temp_Part_Transform in m_parts.transform)
        {
            GameObject UI_Part = new GameObject();
            UI_Part.name = temp_Part_Transform.gameObject.name;
            UI_Part.transform.SetParent(transform);
            string fullPath = path + "/" + UI_Part.name + ".png";
            UI_Part.AddComponent<RawImage>();
            Vector2 position = new Vector2(temp_Index * 100 + 100, -100);
            UI_Part.transform.localPosition = position;
            if (System.IO.File.Exists(fullPath))
            {
                byte[] image = System.IO.File.ReadAllBytes(fullPath);
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(image);
                tex.name = UI_Part.name;
                UI_Part.GetComponent<RawImage>().texture = tex;
            }
            temp_Index++;
        }
    }
}
