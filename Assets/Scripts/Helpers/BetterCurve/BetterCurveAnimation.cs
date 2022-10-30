using System.Collections;
using UnityEngine;
// Original Authors - Wyatt Senalik and Aaron Duffey

public abstract class BetterCurveAnimation : MonoBehaviour
{
    [SerializeField] private BetterCurve m_curve = null;
    [SerializeField] private bool m_playOnStart = false;

    private bool m_isCoroutineRunning = false;
    private Coroutine m_animCorout = null;

    private void Start()
    {
        if (m_playOnStart) { StartAnimation(); }
    }

    public void StartAnimation(bool shouldInterrupt = false)
    {
        StartDoAnimationCoroutine(shouldInterrupt);
    }

    protected abstract void TakeCurveAction(float curveValue);


    private void StartDoAnimationCoroutine(bool shouldInterrupt)
    {
        // Coroutine is already running
        if (m_isCoroutineRunning)
        {
            // Don't interrupt the currently running coroutine
            if (!shouldInterrupt) { return; }
            // Do interrupt
            StopCoroutine(m_animCorout);
        }
        // Start new routine.
        m_animCorout = StartCoroutine(DoAnimationCoroutine());
    }
    private IEnumerator DoAnimationCoroutine()
    {
        m_isCoroutineRunning = true;

        float temp_curTime = 0.0f;
        float temp_endTime = m_curve.GetEndTime();
        // Set localScale to the current time
        while (temp_curTime < temp_endTime)
        {
            float temp_curveValue = m_curve.Evaluate(temp_curTime);
            TakeCurveAction(temp_curveValue);

            yield return null;
            temp_curTime += Time.deltaTime;
        }
        // Snap to the final localScale after finishing time
        float temp_endScale = m_curve.Evaluate(temp_endTime);
        TakeCurveAction(temp_endScale);

        m_isCoroutineRunning = false;
    }
}
