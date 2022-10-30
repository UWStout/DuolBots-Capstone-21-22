using System;
using UnityEngine;
using UnityEngine.Assertions;
// Original Authors - Eslis Vang and Wyatt Senalik

namespace DuolBots
{
    [RequireComponent(typeof(RectTransform))]
    public class SetUIPositionPerPlayer : MonoBehaviour
    {
        [SerializeField]
        private RectTransformData[] m_playerRectDataList = new RectTransformData[2];

        private RectTransform m_rectTransform = null;


        // Called 0th
        // Domestic Initialization
        private void Awake()
        {
            m_rectTransform = GetComponent<RectTransform>();
            CustomDebug.AssertComponentIsNotNull(m_rectTransform, this);
        }


        public void SetPosition(byte playerIndex)
        {
            Assert.IsTrue(playerIndex < m_playerRectDataList.Length,
                $"Given playerIndex of {playerIndex} is out of bounds for " +
                $"{name}'s {GetType().Name}. Can be a maximum of " +
                $"{m_playerRectDataList.Length - 1}.");

            // Apply position to rect transform.
            m_playerRectDataList[playerIndex].Apply(m_rectTransform);
        }
    }

    [Serializable]
    public class RectTransformData
    {
        [SerializeField]
        private RectTransform.Edge m_edge = RectTransform.Edge.Left;
        [SerializeField] private float m_inset = 0.0f;
        [SerializeField] private float m_size = 100.0f;

        public RectTransform.Edge edge => m_edge;
        public float inset => m_inset;
        public float size => m_size;


        public void Apply(RectTransform rectTransform)
        {
            rectTransform.SetInsetAndSizeFromParentEdge(edge, inset, size);
        }
    }
}
