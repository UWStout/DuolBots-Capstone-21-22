using DuolBots;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class PartSelectionPlayerNavigationManager : MonoBehaviour
{
    // References to UI that will be controlled
    // Root UI object (Canvas)
    [SerializeField] private GameObject m_uiRoot = null;
    // Which UI element will be the default selection
    [SerializeField] private GameObject[] m_uiFirstSelected = null;

    [SerializeField] [ReadOnly] private List<GameObject> m_playerObjects = null;

    [SerializeField] [ReadOnly] private int m_GameState = -2;

    private PartSelectorManager m_partSelector = null;

    private void Awake()
    {
    }
    // Start is called before the first frame update
    void Start()
    {
        m_partSelector = PartSelectorManager.instance;
        Assert.IsNotNull(m_partSelector, $"{this.name}: PartSelectorManager is null.");
    }

    // Update is called once per frame
    void Update()
    {
        if (m_playerObjects.Count <= 0)
        {
            m_playerObjects = GetPlayerGameObjects();
            m_GameState = -1;
        }
        if (m_GameState == -1)
        {
            m_GameState = 0;
            foreach (GameObject temp_player in m_playerObjects)
            {
                temp_player.GetComponentInChildren<MultiplayerEventSystem>().playerRoot = m_uiRoot;
                if (m_uiFirstSelected.Length > 0)
                {
                    temp_player.GetComponentInChildren<MultiplayerEventSystem>().firstSelectedGameObject = m_uiFirstSelected[m_GameState].transform.GetChild(0).gameObject;
                    SetSelectedUIComponent(temp_player.GetComponentInChildren<MultiplayerEventSystem>(), temp_player.GetComponentInChildren<MultiplayerEventSystem>().firstSelectedGameObject);
                    GivePlayerControl(0, 0);
                }
            }
        }
    }

    public void GiveNextPlayerControl(int newGameState)
    {
        MultiplayerEventSystem temp_activeEventSystem = m_playerObjects[m_partSelector.playerTurnIndex].GetComponentInChildren<MultiplayerEventSystem>();
        m_GameState = newGameState;

        temp_activeEventSystem.playerRoot = m_uiRoot;
        temp_activeEventSystem.firstSelectedGameObject = m_uiFirstSelected[m_GameState].transform.GetChild(0).gameObject;
        temp_activeEventSystem.SetSelectedGameObject(m_uiFirstSelected[m_GameState]);
    }

    public void GivePlayerControl(int index, int newGameState)
    {
        MultiplayerEventSystem temp_activeEventSystem = m_playerObjects[index].GetComponentInChildren<MultiplayerEventSystem>();
        m_GameState = newGameState;

        temp_activeEventSystem.playerRoot = m_uiRoot;
        temp_activeEventSystem.firstSelectedGameObject = m_uiFirstSelected[m_GameState].transform.GetChild(0).gameObject;
        temp_activeEventSystem.SetSelectedGameObject(m_uiFirstSelected[m_GameState]);
    }

    /// <summary>
    /// Called from buttons.
    /// </summary>
    /// <param name="uiToSelect"></param>
    public void SetSelectedUIComponent(GameObject uiToSelect)
    {
        m_playerObjects[m_partSelector.playerTurnIndex].GetComponent<MultiplayerEventSystem>().SetSelectedGameObject(uiToSelect);
    }
    /// <summary>
    /// Called from buttons.
    /// </summary>
    /// <param name="uiToSelect"></param>
    public void SetSelectedUIComponent(MultiplayerEventSystem activeEventSystem, GameObject uiToSelect)
    {
        activeEventSystem.SetSelectedGameObject(uiToSelect);
    }

    private List<GameObject> GetPlayerGameObjects()
    {
        IReadOnlyList<PlayerInput> temp_playerIndex = FindObjectsOfType<PlayerInput>();
        List<GameObject> temp_playerObjectList = new List<GameObject>();
        foreach (PlayerInput temp_player in temp_playerIndex)
        {
            if (temp_player.user.index == 2)
            {
                temp_playerObjectList.Insert(0, temp_player.gameObject);
            }
            else if (temp_player.user.index == 3)
            {
                temp_playerObjectList.Add(temp_player.gameObject);
            }
        }
        return temp_playerObjectList;
    }
}
