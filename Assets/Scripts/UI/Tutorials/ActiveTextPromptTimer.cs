using System;
using UnityEngine;
using UnityEngine.UI;

using NaughtyAttributes;
// Original Authors - ?

namespace DuolBots
{
    public class ActiveTextPromptTimer : MonoBehaviour
    {
        public static event Action onActiveNextButton;
        public static event Action onActiveBackButton;

        [SerializeField, Required] private BattleStateManager m_stateMan = null;
        //manages when the continue and back are active
        [SerializeField, Required] private Image m_backFill, m_nextFill;
        private float m_cooldownLength = 2.5f, m_timeValue = 0.0f;
        private bool m_cooldownActive = false;
        private DialogueIndex m_dialogueIndex;

        private BattleStateChangeHandler m_battleHandler = null;
        private Input_AdvanceText[] m_playerAdvanceTextInp
            = new Input_AdvanceText[0];


        // Domestic Initialization
        private void Awake()
        {
            m_dialogueIndex = GetComponent<DialogueIndex>();
            #region Asserts
            CustomDebug.AssertSerializeFieldIsNotNull(m_backFill,
                nameof(m_backFill), this);
            CustomDebug.AssertSerializeFieldIsNotNull(m_nextFill,
                nameof(m_nextFill), this);
            CustomDebug.AssertComponentIsNotNull(m_dialogueIndex, this);
            #endregion Asserts

            m_cooldownActive = true;
        }
        // Foreign Initialization
        private void Start()
        {
            m_battleHandler = new BattleStateChangeHandler(m_stateMan,
                BeginBattleHandler, EndBattleHandler, eBattleState.Battle);
        }
        private void OnDestroy()
        {
            m_battleHandler.ToggleActive(false);
        }
        private void Update()
        {
            bool temp_backActive = m_dialogueIndex.dialogueIndex > 0 &&
                m_dialogueIndex.dialogueIndex < 4;
            bool temp_nextActive = m_dialogueIndex.dialogueIndex < 2;
            m_backFill.gameObject.SetActive(temp_backActive);
            m_nextFill.gameObject.SetActive(temp_nextActive);

            if (m_cooldownActive)
            {
                m_timeValue += Time.deltaTime;
                m_backFill.fillAmount = m_timeValue / m_cooldownLength;
                m_nextFill.fillAmount = m_timeValue / m_cooldownLength;

                if (m_timeValue >= m_cooldownLength)
                {
                    m_cooldownActive = false;
                }
            }
        }


        private void BeginBattleHandler()
        {
            // Find player inputs for advance text
            m_playerAdvanceTextInp = FindObjectsOfType<Input_AdvanceText>();
            #region Asserts
            CustomDebug.AssertIsTrueForComponent(m_playerAdvanceTextInp.Length >= 2,
                $"to find at least 2 {nameof(Input_AdvanceText)} in the scene",
                this);
            #endregion Asserts
            // Sub to advance and back text
            foreach (Input_AdvanceText temp_textInp in m_playerAdvanceTextInp)
            {
                temp_textInp.onAdvanceTextPrompt += OnAdvanceTextPrompt;
                temp_textInp.onBackTextPrompt += OnBackTextPrompt;
            }
        }
        private void EndBattleHandler()
        {
            // Unsub
            foreach (Input_AdvanceText temp_textInp in m_playerAdvanceTextInp)
            {
                if (temp_textInp == null) { continue; }
                temp_textInp.onAdvanceTextPrompt -= OnAdvanceTextPrompt;
                temp_textInp.onBackTextPrompt -= OnBackTextPrompt;
            }
        }
        private void OnAdvanceTextPrompt()
        {
            if (m_cooldownActive) { return; }
            onActiveNextButton?.Invoke();
            ResetCooldown();
        }
        private void OnBackTextPrompt()
        {
            if (m_cooldownActive) { return; }
            onActiveBackButton?.Invoke();
            ResetCooldown();
        }
        private void ResetCooldown()
        {
            m_timeValue = 0.0f;
            m_cooldownActive = true;
        }
    }
}
