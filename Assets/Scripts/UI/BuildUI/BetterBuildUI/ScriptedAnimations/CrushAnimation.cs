using UnityEngine;

using NaughtyAttributes;
// Original Authors - Eslis Vang and Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Programmable animation to crush the bots in the
    /// build bot scene.
    /// </summary>
    public class CrushAnimation : ScriptedAnimation
    {
        [SerializeField] private Animator m_animator = null;
        [SerializeField] [AnimatorParam(nameof(m_animator))]
        private string m_crushTriggerAnimParam = "Crush";


        public override void Play()
        {
            m_animator.SetTrigger(m_crushTriggerAnimParam);
        }
    }
}
