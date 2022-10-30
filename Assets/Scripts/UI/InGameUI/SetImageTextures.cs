using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Original Author - Shelby Vian

/// <summary>
/// Sets textures of the icon to the correct part image
/// </summary>
public class SetImageTextures : MonoBehaviour
{
    public Texture2D texturePart;
    [SerializeField] private GameObject partImage;
    public byte partSlot;
    
    public void SetTexture(Texture2D newTexture, byte slot)
    {
        partImage.GetComponent<RawImage>().texture = newTexture;
        partSlot = slot;
    }
}
