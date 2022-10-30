using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(MeshRenderer))]
public class PartSelectionOutlinePlugObject : MonoBehaviour
{
    [SerializeField] private Material m_outlineMaterial;
    [SerializeField] private float m_outlineScaleFactor;
    [SerializeField] private Color m_outlineColor;
    [SerializeField] [ReadOnly] private Renderer m_renderer = null;

    void Start()
    {
        m_renderer = CreateOutline(m_outlineMaterial, m_outlineScaleFactor, m_outlineColor);
    }
    Renderer CreateOutline(Material outlineMat, float scaleFactor, Color color)
    {
        GameObject outlineObject = Instantiate(this.gameObject, transform.position, transform.rotation, transform);
        outlineObject.transform.localScale = Vector3.one;
        Renderer rend = outlineObject.GetComponent<Renderer>();
        rend.material = outlineMat;
        rend.material.SetColor("_OutlineColor", color);
        rend.material.SetFloat("_ScaleFactor", scaleFactor);
        rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        Destroy(outlineObject.GetComponent<PartSelectionOutlinePlugObject>());
        rend.enabled = false;

        return rend;
    }

    public void EnableOutline()
    {
        m_renderer.enabled = true;
    }

    public void DisableOutline()
    {
        m_renderer.enabled = false;
    }
}
