using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PartSelectPlayerInputManager : MonoBehaviour
{
    [SerializeField] PartSelectionRow[] m_partSelection = new PartSelectionRow[2];

    private void Start()
    {
        m_partSelection[0] = GameObject.Find("P1ScrollViewManager").GetComponent<PartSelectionRow>();
        m_partSelection[1] = GameObject.Find("P2ScrollViewManager").GetComponent<PartSelectionRow>();
    }

    public void OnPlayerJoined(PlayerInput player)
    {
        if (GameObject.Find("Player 1"))
        {
            player.name = "Player 2";
            m_partSelection[1].UpdateActiveBox();
            //m_partSelection[1].UpdateCellHighlight();
        }
        else
        {
            player.name = "Player 1";
            m_partSelection[0].UpdateActiveBox();
            //m_partSelection[0].UpdateCellHighlight();
        }
    }

}
