using Cinemachine;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using DuolBots;
// Original Author(s) - Eslis Vang


public class ChassisSelectionCinemachineCameraTarget : MonoBehaviour
{
    private const bool IS_DEBUGGING = false;

    private PartSelectorManager m_partSelector = null;
    private bool m_firstTimeLoop = true;
    [SerializeField] [ReadOnly] private CinemachineFreeLook m_freeLookCamera = null;
    [SerializeField] [ReadOnly] private CinemachineVirtualCamera m_virtalCamera = null;
    [SerializeField] [ReadOnly] private Transform[] m_cameraFollowTargets = null;
    [SerializeField] [ReadOnly] private Transform[] m_cameraLookAtTargets = null;
    [SerializeField] [ReadOnly] private int[] activeColumnIndex = null;
    [SerializeField] private GenerateViewportBot m_viewportBot = null;
    [SerializeField] [ReadOnly] private SlotPlacementManager m_slotManager = null;
    [SerializeField] [ReadOnly] private bool m_searchForSlotManager = false;
    [SerializeField] private Transform m_firstIndexFollowPointVirtualCamera = null;
    [SerializeField] private Transform m_firstIndexLookAtPointVirtualCamera = null;
    [SerializeField] private bool m_FreelookSettings = false;
    [SerializeField] [ShowIf("m_FreelookSettings")] private Transform m_firstIndexFollowPoint = null;
    [SerializeField] [ShowIf("m_FreelookSettings")] private Transform m_firstIndexLookAtPoint = null;
    [SerializeField] [ShowIf("m_FreelookSettings")] private GameObject m_plugObject = null;
    [SerializeField] [ShowIf("m_FreelookSettings")] [ReadOnly] private GameObject[] m_plugObjects = null;

    private void OnEnable()
    {
        m_searchForSlotManager = true;
    }

    private void Awake()
    {
        activeColumnIndex = new int[2];
    }
    // Start is called before the first frame update
    void Start()
    {
        if (TryGetComponent<CinemachineFreeLook>(out CinemachineFreeLook temp_freeLookCamera))
        {
            m_freeLookCamera = temp_freeLookCamera;
        }
        if (TryGetComponent<CinemachineVirtualCamera>(out CinemachineVirtualCamera temp_virtalCamera))
        {
            m_virtalCamera = temp_virtalCamera;
        }
        m_partSelector = PartSelectorManager.instance;
        Assert.IsNotNull(m_partSelector, $"{this.name}: Part Selector Manager is null.");

        activeColumnIndex[0] = m_partSelector.GetActiveColumn(0);
        activeColumnIndex[1] = m_partSelector.GetActiveColumn(1);
    }

    private void Update()
    {
        activeColumnIndex[0] = m_partSelector.GetActiveColumn(0);
        activeColumnIndex[1] = m_partSelector.GetActiveColumn(1);
    }

    private void FixedUpdate()
    {
        if (!m_partSelector.isChassisSelected) { return; }
        if (m_searchForSlotManager)
        {
            m_slotManager = m_viewportBot.botObject.GetComponentInChildren<SlotPlacementManager>();
        }

        if (m_freeLookCamera == null) { return; }
        if (m_slotManager == null) { return; }

        // Only run if the slot manager is null and when the chassis has been selected.
        if (m_firstTimeLoop)
        {
            m_cameraFollowTargets = new Transform[m_slotManager.GetSlotAmount() + 1];
            m_cameraLookAtTargets = new Transform[m_slotManager.GetSlotAmount() + 1];
            m_plugObjects = new GameObject[m_slotManager.GetSlotAmount() + 1];

            m_cameraFollowTargets[0] = m_firstIndexFollowPoint;
            m_cameraLookAtTargets[0] = m_firstIndexLookAtPoint;

            m_firstTimeLoop = false;
        }

        for (int i = 0; i < m_slotManager.GetSlotAmount(); i++)
        {
            Transform temp_slotTransform = m_slotManager.GetSlotTransform(i);
            if (!GameObject.Find($"Slot_{m_slotManager.GetSlotAmount() - 1}PlugObject"))
            {
                m_plugObjects[i + 1] = Instantiate(m_plugObject);
                m_plugObjects[i + 1].transform.SetPositionAndRotation(temp_slotTransform.position, temp_slotTransform.rotation);
                m_plugObjects[i + 1].name = $"{temp_slotTransform.name}PlugObject";
                m_plugObjects[i + 1].transform.parent = m_viewportBot.botObject.transform;
            }
            m_cameraFollowTargets[i + 1] = temp_slotTransform;
            m_cameraLookAtTargets[i + 1] = temp_slotTransform;
        }
    }

    public void SetNewCameraTarget(int index, int playerIndex)
    {
        if (index < 0) { return; }
        if (index >= m_cameraFollowTargets.Length) { return; }
        if (m_freeLookCamera != null && m_virtalCamera != null) { return; }

        if (m_freeLookCamera != null)
        {
            m_freeLookCamera.m_Follow = m_cameraFollowTargets[index];
            m_freeLookCamera.m_LookAt = m_cameraLookAtTargets[index];
            m_freeLookCamera.m_XAxis.Value = 180f;
            m_freeLookCamera.m_YAxis.Value = 0.5f;

            foreach (GameObject plug in m_plugObjects)
            {
                if (plug != null)
                {
                    if (plug == m_plugObjects[index])
                    {
                        CustomDebug.Log($"{plug.name}: Enabling", IS_DEBUGGING);
                        plug.GetComponentInChildren<PartSelectionOutlinePlugObject>().EnableOutline();
                    }
                    else
                    {
                        CustomDebug.Log($"{plug.name}: Disabling", IS_DEBUGGING);
                        plug.GetComponentInChildren<PartSelectionOutlinePlugObject>().DisableOutline();
                    }
                }
            }
        }
        else if (m_virtalCamera != null)
        {
            m_virtalCamera.m_Follow = m_cameraFollowTargets[m_partSelector.GetActiveColumn(playerIndex)];
            m_virtalCamera.m_LookAt = m_cameraLookAtTargets[m_partSelector.GetActiveColumn(playerIndex)];
        }
    }

    public void ResetVirtualCamera()
    {
        m_virtalCamera.m_Follow = m_firstIndexFollowPointVirtualCamera;
        m_virtalCamera.m_LookAt = m_firstIndexLookAtPointVirtualCamera;
    }

    public void UpdateAxisValue(InputValue value)
    {
        Vector2 temp_rotateVector = value.Get<Vector2>();
        if (m_freeLookCamera != null)
        {
            m_freeLookCamera.m_YAxis.m_InputAxisValue = temp_rotateVector.y;
            m_freeLookCamera.m_XAxis.m_InputAxisValue = temp_rotateVector.x;
        }
    }

    public bool CheckIfValidVirtualCam()
    {
        if (m_virtalCamera.m_Follow == m_firstIndexFollowPointVirtualCamera) { return false; }
        if (m_virtalCamera.m_LookAt == m_firstIndexLookAtPointVirtualCamera) { return false; }
        return true;
    }
}
