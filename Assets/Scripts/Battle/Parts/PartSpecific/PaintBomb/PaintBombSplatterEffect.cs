using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

using NaughtyAttributes;
// Original Authors - Aaron Duffey and Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Randomizes the Images used for the splatter effect.
    /// </summary>
    public class PaintBombSplatterEffect : MonoBehaviour
    {
        // Constants
        private const bool IS_DEBUGGING = false;

        [SerializeField, Required] private Image m_splatterImg = null;
        [SerializeField]
        private List<Sprite> m_splatterSprites = new List<Sprite>();

        // DO NOT CHANGE (Colors specified by Kasey)
        [SerializeField] private List<Color> m_splatterColorOptions =
            new List<Color>() { new Color(255, 0, 0), new Color(255, 137, 0),
            new Color(255, 161, 0), new Color(0, 233, 3),
            new Color(0, 176, 255), new Color(105, 0, 255) };
        [SerializeField] private BetterCurve m_growthCurve = new BetterCurve();

        private void Awake()
        {
            #region Asserts
            CustomDebug.AssertSerializeFieldIsNotNull(m_splatterImg,
                nameof(m_splatterImg), this);
            CustomDebug.AssertIsTrueForComponent(m_splatterSprites.Count != 0,
                $"at least 1 splatter sprite to be specified", this);
            CustomDebug.AssertIsTrueForComponent(m_splatterColorOptions.Count != 0,
                $"at least 1 color opiton to be specified", this);
            #endregion Asserts

            int temp_spriteRandIndex = Random.Range(0, m_splatterSprites.Count - 1);
            int temp_colorRandIndex = Random.Range(0, m_splatterColorOptions.Count - 1);
            #region Asserts
            CustomDebug.AssertIndexIsInRange(temp_spriteRandIndex,
                m_splatterSprites, this);
            CustomDebug.AssertIndexIsInRange(temp_colorRandIndex,
                m_splatterColorOptions, this);
            #endregion Asserts
            // Swaps the sprite in the image with a random effect in the range
            m_splatterImg.sprite = m_splatterSprites[temp_spriteRandIndex];
            // Select random color
            m_splatterImg.color = m_splatterColorOptions[temp_colorRandIndex];
            
            // Start process of growing splatter effect until it reaches its max size
            StartCoroutine(SplatterGrowth());

            #region Logs
            CustomDebug.Log($"{name} is awake, beginning splatter growth.", IS_DEBUGGING);
            #endregion Logs
        }

        private IEnumerator SplatterGrowth()
        {
            float temp_curTime = 0.0f;
            float temp_endTime = m_growthCurve.GetEndTime();
            // Set localScale to the current time
            while (temp_curTime < temp_endTime)
            {
                float temp_curScale = m_growthCurve.Evaluate(temp_curTime);
                transform.localScale = new Vector3(temp_curScale, temp_curScale,
                    temp_curScale);

                yield return null;
                temp_curTime += Time.deltaTime;
            }
            // Snap to the final localScale after finishing time
            float temp_endScale = m_growthCurve.Evaluate(temp_endTime);
            transform.localScale = new Vector3(temp_endScale, temp_endScale,
                temp_endScale);
        }
    }
}
