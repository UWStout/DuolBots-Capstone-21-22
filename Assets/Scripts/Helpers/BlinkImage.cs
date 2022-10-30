using System.Collections;
using UnityEngine;
using UnityEngine.UI;
// Original Authors - Wyatt Senalik

[RequireComponent(typeof(Image))]
public class BlinkImage : MonoBehaviour
{
    // Y axis is alpha values.
    [SerializeField] private BetterCurve m_blinkCurve = new BetterCurve();
    private Image m_image = null;

    private float m_endTime = 0;
    private bool m_isBlinkCoroutActive = false;
    private Coroutine m_blinkCorout = null;


    // Domestic Initialization
    private void Awake()
    {
        m_image = GetComponent<Image>();
        #region Asserts
        CustomDebug.AssertComponentIsNotNull(m_image, this);
        #endregion Asserts

        m_endTime = m_blinkCurve.GetEndTime();
    }


    /// <summary>
    /// Makes the image blink based on the blink (alpha) curve.
    /// </summary>
    /// <param name="shouldInterupt">If the ongoing blink should be
    /// interupted.</param>
    /// <returns>If a new Blink has been started. Always true if interupt is true.
    /// True if a blink is not already occurring.</returns>
    public bool Blink(bool shouldInterupt=false)
    {
        if (!enabled) { return false; }

        if (shouldInterupt)
        {
            StopCoroutine(m_blinkCorout);
            m_blinkCorout = StartCoroutine(BlinkCoroutine());
            return true;
        }
        if (m_isBlinkCoroutActive) { return false; }
        m_blinkCorout = StartCoroutine(BlinkCoroutine());
        return true;
    }


    private IEnumerator BlinkCoroutine()
    {
        m_isBlinkCoroutActive = true;

        float t = 0.0f;
        while (t < m_endTime)
        {
            float alpha = m_blinkCurve.Evaluate(t);
            UpdateAlpha(alpha);

            yield return null;
            t += Time.deltaTime;
        }
        UpdateAlpha(m_blinkCurve.Evaluate(m_endTime));

        m_isBlinkCoroutActive = false;
    }
    private void UpdateAlpha(float alpha)
    {
        Color col = m_image.color;
        col.a = alpha;
        m_image.color = col;
    }
}
