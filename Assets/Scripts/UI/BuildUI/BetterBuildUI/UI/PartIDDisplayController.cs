using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using NaughtyAttributes;

namespace DuolBots
{
    public class PartIDDisplayController : MonoBehaviour
    {
        [SerializeField][NaughtyAttributes.Tag] private string m_playerTag = "";
        [SerializeField] private TextMeshProUGUI[] m_nameDisplays =
            new TextMeshProUGUI[2];
        [SerializeField] private DollyTargetCycler[] m_chassisMovePlayerCyclers =
            new DollyTargetCycler[2];
        [SerializeField] private ChassisMoveSelectOnReadyUp m_chassisMoveReadyUp = null;
        [SerializeField, ReadOnly] private GameObject[] m_playerObjects =
            new GameObject[2];

        private PartSelectPlayerSelection[] m_partSel = new PartSelectPlayerSelection[2];

        private BetterBuildSceneStateChangeHandler m_displayHandler = null;
        private BetterBuildSceneStateManager m_stateMan = null;
        private PartDatabase m_partDatabase = null;

        // Start is called before the first frame update
        void Start()
        {
            m_playerObjects = GameObject.FindGameObjectsWithTag(m_playerTag);

            m_stateMan = BetterBuildSceneStateManager.instance;
            m_partDatabase = PartDatabase.instance;
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_stateMan, this);
            CustomDebug.AssertDynamicSingletonMonoBehaviourPersistantIsNotNull(m_partDatabase, this);
            #endregion

            m_displayHandler = BetterBuildSceneStateChangeHandler.CreateNew(
                BeginDisplayHandler, EndDisplayHandler, eBetterBuildSceneState.Chassis,
                eBetterBuildSceneState.Movement, eBetterBuildSceneState.Part);
        }

        private void OnEnable()
        {
            foreach (DollyTargetCycler cycler in m_chassisMovePlayerCyclers)
            {
                cycler.onSelectionIndexChange += UpdateNameDisplay;
            }
        }

        private void OnDestroy()
        {
            m_displayHandler.ToggleActive(false);
            foreach (DollyTargetCycler cycler in m_chassisMovePlayerCyclers)
            {
                cycler.onSelectionIndexChange -= UpdateNameDisplay;
            }
        }

        private void BeginDisplayHandler()
        {
            if (m_stateMan.curState == eBetterBuildSceneState.Part)
            {
                for (int i = 0; i < m_playerObjects.Length; i++)
                {
                    m_partSel[i] = m_playerObjects[i].
                        GetComponentInChildren<PartSelectPlayerSelection>();
                    m_partSel[i].onSelectedIndexChanged += UpdateNameDisplay;
                }
            }

            StartNameDisplay();
        }

        private void EndDisplayHandler()
        {
            ClearNameDisplay();
        }
        private void StartNameDisplay()
        {
            foreach (TextMeshProUGUI text in m_nameDisplays)
            {
                byte temp_playerIndex = text.GetComponentInParent<PlayerIndex>().
                    playerIndex;
                string temp_partID = "";

                if (m_stateMan.curState == eBetterBuildSceneState.Chassis)
                {
                    temp_partID = m_partDatabase.GetPartScriptableObject(
                        m_chassisMoveReadyUp.optionList[m_chassisMovePlayerCyclers[
                        temp_playerIndex].currentSelectedIndex].chassisID).partName;
                }
                if (m_stateMan.curState == eBetterBuildSceneState.Movement)
                {
                    temp_partID = m_partDatabase.GetPartScriptableObject(
                        m_chassisMoveReadyUp.optionList[m_chassisMovePlayerCyclers[
                            temp_playerIndex].currentSelectedIndex].movementID).partName;
                }
                if (m_stateMan.curState == eBetterBuildSceneState.Part)
                {
                    temp_partID = m_partSel[temp_playerIndex].GetCurrentlySelectedPartSO().partName;
                }

                text.text = temp_partID;
            }
        }

        private void UpdateNameDisplay(int index)
        {
            StartNameDisplay();
        }

        private void ClearNameDisplay()
        {
            m_nameDisplays[0].text = "";
            m_nameDisplays[1].text = "";
        }
    }
}
