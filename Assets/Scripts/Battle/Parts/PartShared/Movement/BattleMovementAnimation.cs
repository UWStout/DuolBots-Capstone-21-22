using UnityEngine;
using UnityEngine.Assertions;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Updates the MovementModelAnimController based on movement data
    /// from the SharedController_Movement.
    /// </summary>
    [RequireComponent(typeof(SharedController_Movement))]
    public class BattleMovementAnimation : MonoBehaviour
    {
        // References
        private IMovementModelAnimController m_moveAnimController = null;
        private SharedController_Movement m_sharedMoveController = null;


        // Called 0th
        // Domestic Initialization
        private void Awake()
        {
            m_moveAnimController = GetComponentInParent<IMovementModelAnimController>();
            Assert.IsNotNull(m_moveAnimController, $"{name}'s {GetType().Name} requires " +
                $"a {m_moveAnimController.GetType().Name} to be attached to a parent, but none was.");

            m_sharedMoveController = GetComponent<SharedController_Movement>();
            Assert.IsNotNull(m_sharedMoveController, $"{GetType().Name} requires " +
                $"a {m_sharedMoveController.GetType().Name} to be attached to {name}, but none was.");
        }
        // Called once every frame
        private void Update()
        {
            UpdateMovementAnimValues();
        }


        /// <summary>
        /// Updates the animvalues for the MovementAnimationController.
        /// 
        /// Pre Conditions - SharedController_Movement and MovementAnimationController are not null.
        /// That MovementAnimationController has 4 models specified on it in this order:
        /// left, left, right, right.
        /// Post Conditions - Applies the current left power to both left animators and the
        /// current right power to both right animators.
        /// </summary>
        private void UpdateMovementAnimValues()
        {
            float temp_curLeftMove = m_sharedMoveController.LeftPowerVisual;
            float temp_curRightMove = m_sharedMoveController.RightPowerVisual;

            // Assumes that there are 2 left movement models and 2 right movement models
            m_moveAnimController.UpdateMoveValues(temp_curLeftMove, temp_curLeftMove,
                temp_curRightMove, temp_curRightMove);
        }
    }
}
