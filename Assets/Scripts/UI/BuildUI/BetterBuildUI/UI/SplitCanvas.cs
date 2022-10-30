using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.RectTransform;
// Original Authors - Eslis Vang and Wyatt Senalik

namespace DuolBots
{
    [RequireComponent(typeof(RectTransform))]
    public class SplitCanvas : MonoBehaviour
    {
        [SerializeField]
        private Vector2Int m_screenDimensions = new Vector2Int(1920, 1080);
        [SerializeField]
        private UnityEvent<byte> m_onSplitCanvasAsPlayer = new UnityEvent<byte>();

        private RectTransform m_rectTransform = null;


        // Called 0th
        // Domestic Initialization
        private void Awake()
        {
            m_rectTransform = GetComponent<RectTransform>();
            CustomDebug.AssertComponentIsNotNull(m_rectTransform, this);
        }


        public void SplitAsPlayerCanvas(byte playerIndex)
        {
            Edge temp_edge = playerIndex == 0 ? Edge.Left : Edge.Right;

            m_rectTransform.SetInsetAndSizeFromParentEdge(temp_edge,
                0, m_screenDimensions.x / 2);

            m_onSplitCanvasAsPlayer.Invoke(playerIndex);
        }
    }
}
