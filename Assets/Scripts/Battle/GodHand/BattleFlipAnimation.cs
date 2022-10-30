using UnityEngine;

using NaughtyAttributes;
// Original Authors - Eslis Vang and Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Programmable animation to flip a bot in the
    /// build bot scene.
    /// </summary>
    public class BattleFlipAnimation : ScriptedAnimation
    {
        [SerializeField] [Required] private Animator m_godHandAnimator = null;
        [SerializeField] [AnimatorParam(nameof(m_godHandAnimator))]
        private string m_godHandFlipTriggerAnimParam = "Flip";


        public override void Play()
        {
            m_godHandAnimator.SetTrigger(m_godHandFlipTriggerAnimParam);
        }
    }
}
