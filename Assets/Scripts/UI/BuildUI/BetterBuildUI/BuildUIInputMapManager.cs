using UnityEngine;
// Original Authors - Eslis Vang and Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Swaps between input maps for each state of the better build ui.
    /// </summary>
    [RequireComponent(typeof(InputMapStack))]
    public class BuildUIInputMapManager : MonoBehaviour
    {
        [SerializeField]
        private string m_chasissMoveInputMapName = "ChassisAndMovement";
        [SerializeField] private string m_partInputMapName = "PartSelect";

        private BetterBuildSceneStateManager m_stateMan = null;
        private InputMapStack m_inputStack = null;

        private BetterBuildSceneStateChangeHandler m_chassisMoveHandler = null;
        private BetterBuildSceneStateChangeHandler m_partHandler = null;


        // Domestic Initialization
        private void Awake()
        {
            m_inputStack = GetComponent<InputMapStack>();
            #region Asserts
            CustomDebug.AssertComponentIsNotNull(m_inputStack, this);
            #endregion Asserts
        }
        // Foreign Initialization
        private void Start()
        {
            m_stateMan = BetterBuildSceneStateManager.instance;
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_stateMan, this);
            #endregion Asserts

            m_chassisMoveHandler = new BetterBuildSceneStateChangeHandler(
                m_stateMan, BeginChassisMovementHandler, EndChassisMovementHandler,
                eBetterBuildSceneState.Chassis, eBetterBuildSceneState.Movement);
            m_partHandler = new BetterBuildSceneStateChangeHandler(
                m_stateMan, BeginPartHandler, EndPartHandler,
                eBetterBuildSceneState.Part);
        }
        private void OnDestroy()
        {
            m_chassisMoveHandler.ToggleActive(false);
            m_partHandler.ToggleActive(false);
        }


        #region ChassisMovement
        private void BeginChassisMovementHandler()
        {
            m_inputStack.SwitchInputMap(m_chasissMoveInputMapName);
        }
        private void EndChassisMovementHandler()
        {
            m_inputStack.PopInputMap(m_chasissMoveInputMapName);
        }
        #endregion ChassisMovement

        #region Part
        private void BeginPartHandler()
        {
            m_inputStack.SwitchInputMap(m_partInputMapName);
        }
        private void EndPartHandler()
        {
            m_inputStack.PopInputMap(m_partInputMapName);
        }
        #endregion Part
    }
}
