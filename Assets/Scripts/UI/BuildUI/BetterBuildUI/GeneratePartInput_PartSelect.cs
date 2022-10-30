using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik and Eslis Vang

namespace DuolBots
{
    [RequireComponent(typeof(GenerateInputBindings))]
    public class GeneratePartInput_PartSelect : MonoBehaviour
    {
        private BetterBuildSceneStateManager m_stateMan = null;
        private GenerateInputBindings m_genInpBindings = null;

        private BetterBuildSceneStateChangeHandler m_assignHandler = null;


        // Domestic Initialization
        private void Awake()
        {
            m_genInpBindings = GetComponent<GenerateInputBindings>();
            #region Asserts
            CustomDebug.AssertComponentIsNotNull(m_genInpBindings, this);
            #endregion Asserts
        }
        // Foreign Initialization
        private void Start()
        {
            m_stateMan = BetterBuildSceneStateManager.instance;
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_stateMan, this);
            #endregion Asserts
            m_assignHandler = new BetterBuildSceneStateChangeHandler(m_stateMan,
                BeginAssignHandler, EndAssignHandler,
                eBetterBuildSceneState.Assign);
        }
        private void OnDestroy()
        {
            m_assignHandler.ToggleActive(false);
        }


        #region Assign
        private void BeginAssignHandler()
        {
            BuiltBotData temp_botData = BuildSceneBotData.GetBotData(0);
            List<CustomInputBinding> temp_bindings = m_genInpBindings.
                GenerateInputs(temp_botData);
            BuildSceneInputData.SetData(0, temp_bindings);

            m_stateMan.AdvanceState();
        }
        private void EndAssignHandler() { }
        #endregion Assign
    }
}
