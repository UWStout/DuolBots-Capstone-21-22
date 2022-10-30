using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik and Eslis Vang

namespace DuolBots
{
    public class AdvanceFromPartSelect : MonoBehaviour
    {
        // Constants
        private const bool IS_DEBUGGING = true;

        [SerializeField][Scene] private string m_nextSceneName = "HostJoinPlayer";
        [SerializeField] private Text m_requirementWarning = null;

        private BetterBuildSceneStateManager m_stateMan = null;
        private ReadyUpManager m_readyUpMan = null;
        private SceneLoader m_sceneLoader = null;
        private ChosenPartsManager_PartSelect m_partsMan = null;
        private PartDatabase m_partDatabase = null;

        private BetterBuildSceneStateChangeHandler m_partHandler = null;
        private BetterBuildSceneStateChangeHandler m_endHandler = null;

        private Input_ReadyUpPartSelect[] m_readyUpPartSel = null;
        private bool m_isWarning = false;

        // Foreign Initialization
        private void Start()
        {
            m_stateMan = BetterBuildSceneStateManager.instance;
            m_readyUpMan = ReadyUpManager.instance;
            m_sceneLoader = SceneLoader.instance;
            m_partsMan = ChosenPartsManager_PartSelect.instance;
            m_partDatabase = PartDatabase.instance;
            m_readyUpPartSel = FindObjectsOfType<Input_ReadyUpPartSelect>();
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_stateMan, this);
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_readyUpMan, this);
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_sceneLoader, this);
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_partsMan, this);
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_partDatabase, this);
            #endregion Asserts

            m_partHandler = new BetterBuildSceneStateChangeHandler(m_stateMan,
                BeginPartHandler, EndPartHandler, eBetterBuildSceneState.Part);
            m_endHandler = new BetterBuildSceneStateChangeHandler(m_stateMan,
                BeginEndHandler, EndEndHandler, eBetterBuildSceneState.End);
        }
        private void OnDestroy()
        {
            m_partHandler.ToggleActive(false);
            m_endHandler.ToggleActive(false);
        }


        #region PartState
        private void BeginPartHandler()
        {
            #region Logs
            CustomDebug.LogForComponent(nameof(BeginPartHandler), this,
                IS_DEBUGGING);
            #endregion Logs
            m_readyUpMan.onAllPlayersReady += CheckIfBotMeetsRequirements;
        }
        private void EndPartHandler()
        {
            #region Logs
            CustomDebug.LogForComponent(nameof(EndPartHandler), this,
                IS_DEBUGGING);
            #endregion Logs
            if (m_readyUpMan != null && m_stateMan != null)
            {
                m_readyUpMan.onAllPlayersReady -= CheckIfBotMeetsRequirements;
            }
        }
        #endregion PartState

        #region EndState
        private void BeginEndHandler()
        {
            #region Logs
            CustomDebug.LogForComponent(nameof(BeginEndHandler), this,
                IS_DEBUGGING);
            #endregion Logs
            m_sceneLoader.LoadScene(m_nextSceneName);
        }
        private void EndEndHandler() { }
        #endregion EndState

        private void CheckIfBotMeetsRequirements()
        {
            if (!HasAtLeastOneWeaponPart())
            {
                CustomDebug.Log($"Bot requires at least one weapon part.", IS_DEBUGGING);
                if (!m_isWarning) StartCoroutine(PartRequirementWarning());
                foreach (Input_ReadyUpPartSelect readyPartSel in m_readyUpPartSel)
                {
                    readyPartSel.SetIsReady(false);
                }
                return;
            }
            m_stateMan.AdvanceState();
        }

        private bool HasAtLeastOneWeaponPart()
        {
            foreach (PartInSlot part in m_partsMan.slottedParts)
            {
                PartScriptableObject temp_partSO = m_partDatabase.
                    GetPartScriptableObject(part.partID);
                if (temp_partSO.partType == ePartType.Weapon)
                {
                    return true;
                }
            }

            return false;
        }

        private IEnumerator PartRequirementWarning()
        {
            m_isWarning = true;

            m_requirementWarning.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            m_requirementWarning.gameObject.SetActive(false);

            m_isWarning = false;

            yield return null;
        }
    }
}
