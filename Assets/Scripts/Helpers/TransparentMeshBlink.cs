using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
// Original Authors - Wyatt Senalik

public class TransparentMeshBlink : MonoBehaviour
{
    // SerializeField to save the processing of GetComponentInChildren
    [SerializeField]
    private MeshRenderer[] m_meshesToBlink = new MeshRenderer[1];
    // Y axis is the current alpha
    [SerializeField] private BetterCurve m_blinkCurve = new BetterCurve();
    // Cache the curve's end time to avoid repeated division 
    private float m_curveEndTime = 0.0f;
    private float t = 0.0f;


    // Domestic Initialization
    private void Awake()
    {
        m_curveEndTime = m_blinkCurve.GetEndTime();
    }
    // Foreign Initialization
    private void Start()
    {
        // Have all of the materials be clones so we don't edit
        // the actual assets
        foreach (MeshRenderer temp_mr in m_meshesToBlink)
        {
            for (int i = 0; i < temp_mr.materials.Length; ++i)
            {
                Material temp_mat = temp_mr.materials[i];
                temp_mr.materials[i] = new Material(temp_mat);
            }
        }
    }
    // Called once per frame
    private void Update()
    {
        float temp_curAlpha = m_blinkCurve.Evaluate(t);
        ApplyTransparencyToMeshes(temp_curAlpha);

        t += Time.deltaTime;
        // Wrap t
        if (t > m_curveEndTime) { t = 0.0f; }
    }


    public void SetMeshRenderers(MeshRenderer[] meshRenderers)
    {
        m_meshesToBlink = meshRenderers;
    }


    private void ApplyTransparencyToMeshes(float alpha)
    {
        foreach (MeshRenderer temp_mr in m_meshesToBlink)
        {
            if (!temp_mr.enabled) { return; }
            foreach (Material temp_mat in temp_mr.materials)
            {
                Color temp_matColor = temp_mat.color;
                temp_matColor.a = alpha;
                temp_mat.color = temp_matColor;
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TransparentMeshBlink))]
public class TransparentMeshBlinkEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TransparentMeshBlink transparentMeshBlink = (TransparentMeshBlink)target;

        // When pressed, recalculate all the non-runtime properties
        if (GUILayout.Button("Find Renderers In Children"))
        {
            MeshRenderer[] foundRenderers =
                transparentMeshBlink.GetComponentsInChildren<MeshRenderer>(true);
            transparentMeshBlink.SetMeshRenderers(foundRenderers);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
