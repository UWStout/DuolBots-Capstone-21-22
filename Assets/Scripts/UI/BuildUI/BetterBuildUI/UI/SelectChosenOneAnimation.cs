using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using NaughtyAttributes;

namespace DuolBots
{
    public class SelectChosenOneAnimation : MonoBehaviour
    {
        private const bool IS_DEBUGGING = true;

        [SerializeField] private int m_flickerCount = 0;
        [SerializeField] [Min(0.01f)] private float m_timeBeforeEachFlicker = 0.1f;
        [SerializeField] [Min(0.01f)] private float m_finishedWaitTime = 1.0f;
        [SerializeField] private Color m_baseColor = Color.gray;
        [SerializeField] private Color m_colorGreen = Color.green;

        private PreviewImageController[] m_previewImageCont = null;
        private Image[] m_previewImageBackrounds = null;

        private void Start()
        {
            m_previewImageCont = FindObjectsOfType<PreviewImageController>();
            m_previewImageBackrounds = new Image[2];
            for (int i = 0; i < m_previewImageCont.Length; ++i)
            {
                m_previewImageBackrounds[i] = m_previewImageCont[i].
                    GetComponent<Image>();
            }
        }

        public void Play(int winningPlayerIndex)
        {
            CustomDebug.Log($"<color=9cdcfeff>Winning selection's index</color>: " +
                $"{winningPlayerIndex}", IS_DEBUGGING);
            StartCoroutine(CoroutinePreviewImageFlicker(winningPlayerIndex));
        }

        public void ResetPreviewFlicker()
        {
            m_previewImageBackrounds[0].color = m_baseColor;
            m_previewImageBackrounds[1].color = m_baseColor;
        }

        private IEnumerator CoroutinePreviewImageFlicker(int winningPlayerIndex)
        {
            int temp_flickerIndex = 0;
            int temp_flickerCount = m_flickerCount;
            while (temp_flickerCount > 0)
            {
                m_previewImageBackrounds[0].color = temp_flickerIndex == 0
                    ? m_colorGreen : m_baseColor;
                m_previewImageBackrounds[1].color = temp_flickerIndex == 1
                    ? m_colorGreen : m_baseColor;
                temp_flickerIndex = (temp_flickerIndex + 1) % 2;
                --temp_flickerCount;
                // Ensure that the flicker ends on the correct player's selection
                if (temp_flickerCount == 0 && temp_flickerIndex != winningPlayerIndex)
                {
                    temp_flickerCount = 1;
                }
                // Flicker normally
                if (temp_flickerCount > 0)
                {
                    yield return new WaitForSeconds(m_timeBeforeEachFlicker);
                }
                // Pause on the winning selection
                else
                {
                    yield return new WaitForSeconds(m_finishedWaitTime);
                }
            }

            CustomDebug.LogWarning($"<color=9cdcfeff>Flicker Count</color>: " +
                $"{temp_flickerCount}");

            if (temp_flickerCount <= 0)
            {
                ResetPreviewFlicker();
            }
            yield return null;
        }
    }
}
