using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Controls the highlight on a single part.
    /// </summary>
    [RequireComponent(typeof(Outline))]
    public class PartHighlight : MonoBehaviour
    {
        // Colors for when each player has the 
        [SerializeField] private Color[] m_highlightColors = new Color[2]
            {
                new Color(0, 254, 252),
                new Color(240, 0, 240)
            };
        [SerializeField] [Min(0.0f)] private float m_outlineEnabledWidth = 3.0f;

        private Outline m_outline = null;
        private List<byte> m_playersWhoHighlighted = new List<byte>();


        // Domestic Initialization
        private void Awake()
        {
            m_outline = GetComponent<Outline>();

            CustomDebug.AssertComponentIsNotNull(m_outline, this);
        }


        /// <summary>
        /// True: Calls ActivateHighlight.
        /// False: Calls DeactivateHighlight.
        /// </summary>
        public void ToggleHighlightActive(bool cond, byte playerIndex)
        {
            #region Asserts
            CustomDebug.AssertIndexIsInRange(playerIndex, m_highlightColors,
                this);
            #endregion Asserts

            if (cond)
            {
                ActivateHighlight(playerIndex);
            }
            else
            {
                DeactiveHighlight(playerIndex);
            }
        }
        /// <summary>
        /// Turns on the highlight for the given player.
        /// </summary>
        public void ActivateHighlight(byte playerIndex)
        {
            #region Asserts
            CustomDebug.AssertIndexIsInRange(playerIndex, m_highlightColors,
                this);
            #endregion Asserts

            // Already highlighted for this player
            if (m_playersWhoHighlighted.Contains(playerIndex)) { return; }
            // This is a new player highlighting, add them
            m_playersWhoHighlighted.Add(playerIndex);

            UpdateHighlight();
        }
        /// <summary>
        /// Turns off the highlight for the given player.
        /// </summary>
        public void DeactiveHighlight(byte playerIndex)
        {
            #region Asserts
            CustomDebug.AssertIndexIsInRange(playerIndex, m_highlightColors,
                this);
            #endregion Asserts

            // If was not removed, that means this player was not
            // highlighting in the first place, so no need to update
            if (!m_playersWhoHighlighted.Remove(playerIndex)) { return; }

            UpdateHighlight();
        }
        /// <summary>
        /// Forcefully updates the highlight's color and width
        /// to reflect the current player highlight.
        /// </summary>
        public void ForceUpdateHighlight()
        {
            UpdateHighlight();
        }


        /// <summary>
        /// Updates the width of the highlight and the color to
        /// reflect who has highlighted this part (or if no one has).
        /// </summary>
        private void UpdateHighlight()
        {
            // No one has highlighted this part.
            if (m_playersWhoHighlighted.Count <= 0)
            {
                m_outline.OutlineWidth = 0;
                return;
            }
            // At least one person has highlighted this part.
            m_outline.OutlineWidth = m_outlineEnabledWidth;
            m_outline.OutlineColor = DetermineColor();
        }
        /// <summary>
        /// Figures out which color the outline should be
        /// based on the players who have highlighted this part.
        /// </summary>
        private Color DetermineColor()
        {
            // No players have selected, BAD!
            if (m_playersWhoHighlighted.Count == 0)
            {
                Debug.LogError($"Trying to determine player for no players");
                return Color.black;
            }
            // Multiple players have selected
            if (m_playersWhoHighlighted.Count > 1)
            {
                return Color.white;
            }
            
            if (m_playersWhoHighlighted.Count != 1)
            {
                Debug.LogError($"Logical error detected. Expected only one " +
                    $"player to have highlighted.");
                return Color.black;
            }
            // Only one player has selected
            #region Asserts
            CustomDebug.AssertIndexIsInRange(0, m_playersWhoHighlighted,
                this);
            #endregion Asserts
            byte temp_index = m_playersWhoHighlighted[0];
            #region Asserts
            CustomDebug.AssertIndexIsInRange(temp_index, m_highlightColors,
                this);
            #endregion Asserts
            return m_highlightColors[temp_index];
        }
    }
}
