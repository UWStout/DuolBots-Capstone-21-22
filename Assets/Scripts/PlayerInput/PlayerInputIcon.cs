using System;
using UnityEngine;
using UnityEngine.Assertions;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// UNUSED
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class PlayerInputIcon : MonoBehaviour
    {
        [SerializeField] private Bounds2D m_movementBounds = new Bounds2D();

        private RectTransform m_rectTransform = null;


        // Called 0th
        // Domestic Initialization
        private void Awake()
        {
            m_rectTransform = GetComponent<RectTransform>();
            Assert.IsNotNull(m_rectTransform, $"{name}'s {GetType().Name} " +
                $"requires {nameof(RectTransform)} but none was found.");
        }


        public void Move(Vector2 moveAmount)
        {
            Vector2 temp_curPos = m_rectTransform.anchoredPosition;
            
            Vector2 temp_desiredPos = temp_curPos + moveAmount;
            Vector2 temp_clampedPos = m_movementBounds.
                ClampToFitBounds(temp_desiredPos);

            m_rectTransform.position = temp_clampedPos;

            Debug.Log($"CurPos={temp_curPos}. DesiredPos={temp_desiredPos}." +
                $"");
        }
    }


    [Serializable]
    public class Bounds2D
    {
        [SerializeField] [MinMaxSlider(0.0f, 1.0f)]
        private Vector2 m_horizontalBound = new Vector2(0.0f, 1.0f);
        [SerializeField] [MinMaxSlider(0.0f, 1.0f)]
        private Vector2 m_verticalBound = new Vector2(0.0f, 1.0f);


        public Vector2 ClampToFitBounds(Vector2 desiredPosition)
        {
            float x = Mathf.Clamp(desiredPosition.x,
                m_horizontalBound.x, m_horizontalBound.y);
            float y = Mathf.Clamp(desiredPosition.y,
                m_verticalBound.x, m_verticalBound.y);

            return new Vector2(x, y);
        }
    }
}
