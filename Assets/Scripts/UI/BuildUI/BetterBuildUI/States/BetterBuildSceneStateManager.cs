using UnityEngine;
using System.Collections;
// Original Authors - Eslis Vang and Wyatt Senalik

namespace DuolBots
{
    public enum eBetterBuildSceneState { Chassis, PostChassis, Movement, PostMovement, Part, Assign, End }

    /// <summary>
    /// State manager for the better build scene.
    /// </summary>
    public class BetterBuildSceneStateManager : BaseStateManager<
        eBetterBuildSceneState, BetterBuildSceneStateManager>
    {
        BetterBuildSceneStateChangeHandler m_postChassisHandler = null;
        BetterBuildSceneStateChangeHandler m_postMovementHandler = null;

        //Maybe find a better place for this.
        [SerializeField, NaughtyAttributes.Tag] private string m_playerTag = "Player";
        private GameObject[] m_playerObjs = new GameObject[2];

        // Foreign Initialization
        private void Start()
        {
            CatchupEventResetter temp_eventResetter = CatchupEventResetter.instance;
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(temp_eventResetter,
                this);
            #endregion Asserts

            temp_eventResetter.AddCatchupEventForReset(resetOnInitialStateSet);
            temp_eventResetter.AddCatchupEventForReset(resetOnStateChange);

            m_postChassisHandler = BetterBuildSceneStateChangeHandler.
                CreateNew(RunCoroutine, null, eBetterBuildSceneState.PostChassis);
            m_postMovementHandler = BetterBuildSceneStateChangeHandler.
                CreateNew(RunCoroutine, null, eBetterBuildSceneState.PostMovement);

            m_playerObjs = GameObject.FindGameObjectsWithTag(m_playerTag);
        }

        private void OnDestroy()
        {
            m_postChassisHandler.ToggleActive(false);
            m_postMovementHandler.ToggleActive(false);
        }

        private void RunCoroutine()
        {
            StartCoroutine(WaitBeforeAdvanceCoroutine());
        }

        private IEnumerator WaitBeforeAdvanceCoroutine()
        {
            // Gross hack to fix destroy needing a frame to actually destroy things
            yield return null;
            yield return null;

            foreach (GameObject obj in m_playerObjs)
            {
                PopupStatController temp_statCont =
                    obj.GetComponentInChildren<PopupStatController>(true);
                if (!temp_statCont.gameObject.activeSelf) { continue; }
                temp_statCont.gameObject.SetActive(false);
                obj.GetComponent<InputMapStack>().PopInputMap("UI");
            }
            AdvanceState();
        }
    }
}
