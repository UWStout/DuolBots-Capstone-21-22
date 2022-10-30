using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DuolBots;
using Cinemachine;
using UnityEngine.InputSystem;

public class AssignCamera : MonoBehaviour
{
    private Transform[] m_slotTransforms;
    private SlotPlacementManager m_slot;
    [SerializeField] private Transform m_zeroIndex;
    public CinemachineFreeLook freeLook => m_freeLook;
    private CinemachineFreeLook m_freeLook;

    void Start()
    {
        m_freeLook = GetComponent<CinemachineFreeLook>();
        Invoke("LateStart", 0.05f);
    }

    private void LateStart()
    {
        m_slot = FindObjectOfType<SlotPlacementManager>();
        m_slotTransforms = new Transform[m_slot.slotTransforms.Length + 1];
        IReadOnlyDictionary<int, GameObject> temp_slotDictionary = m_slot.GetSlottedParts();

        m_slotTransforms[0] = m_zeroIndex;
        int counter = 1;

        foreach (KeyValuePair<int, GameObject> temp in temp_slotDictionary)
        {
            //Debug.Log($"{temp.Value.name}");
            if (temp.Value.transform.childCount > 0)
                m_slotTransforms[counter++] = temp.Value.transform;
        }
    }

    public void SetCameraTarget(int index)
    {
        m_freeLook.m_Follow = m_slotTransforms[index];
        m_freeLook.m_LookAt = m_slotTransforms[index];
    }

    public void UpdateAxisValue(InputValue value)
    {
        Vector2 temp_rotateVector = value.Get<Vector2>();
        if (m_freeLook != null)
        {
            m_freeLook.m_YAxis.m_InputAxisValue = temp_rotateVector.y;
            m_freeLook.m_XAxis.m_InputAxisValue = temp_rotateVector.x;
        }
    }
}
