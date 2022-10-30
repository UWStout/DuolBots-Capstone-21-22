using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// MovementModelAnimController that just uses Animators with
    /// a speed float variable on it.
    /// </summary>
    public class SimpleAnimatorMovementModelAnimController : MonoBehaviour, IMovementModelAnimController
    {
        // Constants
        public const string MOVE_ANIM_FLOAT_VAR_NAME = "speed";

        // Animators to update the values of when UpdateMoveValues is called
        [SerializeField] private List<Animator> m_modelAnimators = new List<Animator>();


        /// <summary>
        /// Updates the variables in the Animators to reflect the given values.
        /// 
        /// Pre Conditions - The given amount of floats must be equal to the
        /// specified amount of animators.
        /// Post Conditions - The variables in the animators are updated to reflect
        /// the passed in values where the first value is put in the first animator, the
        /// 2nd value in the 2nd value, and etc.
        /// </summary>
        /// <param name="moveValueArr">Values to update the animators to.</param>
        public void UpdateMoveValues(params float[] moveValueArr)
        {
            Assert.AreEqual(moveValueArr.Length, m_modelAnimators.Count,
                $"{name}'s {GetType().Name}'s {nameof(UpdateMoveValues)} " +
                $"function's given {nameof(moveValueArr)}'s Length was {moveValueArr.Length}, which " +
                $"did not match {nameof(m_modelAnimators)}'s Count of {m_modelAnimators.Count}");

            for (int i = 0; i < moveValueArr.Length; ++i)
            {
                Animator temp_curAnim = m_modelAnimators[i];
                float temp_curVal = moveValueArr[i];

                temp_curAnim.SetFloat(MOVE_ANIM_FLOAT_VAR_NAME, temp_curVal);
            }
        }
    }
}
