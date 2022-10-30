using System;
using UnityEngine;

using NaughtyAttributes;
// Original Authors - Eslis Vang and Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// On the bot root for the build bot scene.
    /// Controls which animation to play for the bot root.
    /// </summary>
    public class BetterBuildBotAnimatorController : MonoBehaviour
    {
        [SerializeField] [Required] private ScriptedAnimation m_flipAnim = null;
        [SerializeField] [Required] private ScriptedAnimation m_crushAnim = null;

        public int selectionIndex { get; set; }

        public event Action<int> onFlipCrushEnd;


        // Called 0th
        // Domestic Initialization
        private void Awake()
        {
            CustomDebug.AssertComponentIsNotNull(m_flipAnim, this);
            CustomDebug.AssertComponentIsNotNull(m_crushAnim, this);
        }
        private void OnEnable()
        {
            m_flipAnim.onEnd += InvokeOnFlipCrushEnd;
            m_crushAnim.onEnd += InvokeOnFlipCrushEnd;
        }
        private void OnDisable()
        {
            if (m_flipAnim != null)
            {
                m_flipAnim.onEnd -= InvokeOnFlipCrushEnd;
            }
            if (m_crushAnim != null)
            {
                m_crushAnim.onEnd -= InvokeOnFlipCrushEnd;
            }
        }


        public void PlayCrushAnimation()
        {
            m_crushAnim.Play();
        }
        public void PlayFlipAnimation()
        {
            m_flipAnim.Play();
        }


        private void InvokeOnFlipCrushEnd()
        {
            onFlipCrushEnd?.Invoke(selectionIndex);
        }
    }
}
