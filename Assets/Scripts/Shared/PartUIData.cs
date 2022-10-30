using System;
using UnityEngine;
// Original Author - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Holds data for UI associated with a part.
    /// </summary>
    [Serializable]
    public class PartUIData
    {
        // Sprite for when the part is available for the player to use
        [SerializeField] private Texture2D m_unlockedSprite = null;
        // Sprite for when the part is not unlocked yet
        [SerializeField] private Texture2D m_lockedSprite = null;
        // Animation clip of the sprite spinning
        [SerializeField] private AnimationClip m_animationClip = null;

        public Texture2D unlockedSprite => m_unlockedSprite;
        public Texture2D lockedSprite => m_lockedSprite;
        public AnimationClip animationClip => m_animationClip;

        public void Initialize(Texture2D unlocked,Texture2D locked, AnimationClip animation)
        {
            m_unlockedSprite = unlocked;
            m_lockedSprite = locked;
            m_animationClip = animation;
        }
    }
}
