using UnityEngine;
using UnityEngine.InputSystem;

using NaughtyAttributes;
// Original Authors - Eslis Vang and Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Input for navigating the part list.
    /// </summary>
    public class Input_PartList : MonoBehaviour
    {
        public enum eHoldDir { None, Left, Right }

        private const bool IS_DEBUGGING = true;
        private const float MOVE_DEAD_ZONE = 0.1f;

        [SerializeField] [Required] private Animator m_animator = null;
        [SerializeField] [AnimatorParam(nameof(m_animator))]
        private string m_leftTriggerParam = "Left";
        [SerializeField] [AnimatorParam(nameof(m_animator))]
        private string m_rightTriggerParam = "Right";
        [SerializeField] [Min(0.0f)] private float m_inputDelay = 0.25f;
        [SerializeField] [Required]
        private PartSelectPlayerPreview m_partSelPlayerPrev = null;

        // Which direction is currently being held down.
        private eHoldDir m_holdDir = eHoldDir.None;
        private float m_curTimeToWaitUntil = 0.0f;


        // Domestic Initialization
        private void Awake()
        {
            #region Asserts
            CustomDebug.AssertSerializeFieldIsNotNull(m_animator,
                nameof(m_animator), this);
            CustomDebug.AssertSerializeFieldIsNotNull(m_partSelPlayerPrev,
                nameof(m_partSelPlayerPrev), this);
            #endregion Asserts
        }
        // Called once per frame
        private void Update()
        {
            // Still waiting before going next
            if (Time.time < m_curTimeToWaitUntil) { return; }

            switch (m_holdDir)
            {
                case eHoldDir.None: break;
                case eHoldDir.Left: MoveLeft(); break;
                case eHoldDir.Right: MoveRight(); break;
                default: CustomDebug.UnhandledEnum(m_holdDir, this); break;
            }
        }

        private void MoveLeft()
        {
            #region Logs
            CustomDebug.Log(nameof(MoveLeft), IS_DEBUGGING);
            #endregion Logs
            m_animator.SetTrigger(m_leftTriggerParam);
            // Update wait time
            m_curTimeToWaitUntil = Time.time + m_inputDelay;
        }
        private void MoveRight()
        {
            #region Logs
            CustomDebug.Log(nameof(MoveRight), IS_DEBUGGING);
            #endregion Logs
            m_animator.SetTrigger(m_rightTriggerParam);
            // Update wait time
            m_curTimeToWaitUntil = Time.time + m_inputDelay;
        }

        #region PlayerInputMessages
        private void OnMove(InputValue value)
        {
            #region Logs
            CustomDebug.Log(nameof(OnMove), IS_DEBUGGING);
            #endregion Logs

            float temp_moveAxis = value.Get<float>();
            if (temp_moveAxis < -MOVE_DEAD_ZONE)
            {
                m_holdDir = eHoldDir.Left;
                return;
            }
            if (temp_moveAxis > MOVE_DEAD_ZONE)
            {
                m_holdDir = eHoldDir.Right;
                return;
            }

            m_holdDir = eHoldDir.None;
        }
        private void OnAttachPart(InputValue value)
        {
            #region Logs
            CustomDebug.Log(nameof(OnAttachPart), IS_DEBUGGING);
            #endregion Logs

            if (value.isPressed)
            {
                m_partSelPlayerPrev.AttachCurrentPart();
            }
        }
        #endregion PlayerInputMessages
    }
}
