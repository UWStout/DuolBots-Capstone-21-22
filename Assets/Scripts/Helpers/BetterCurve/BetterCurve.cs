using System;
using UnityEngine;
using UnityEngine.Assertions;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik and Aaron Duffey

[Serializable]
public class BetterCurve
{
    // Constants
    private const bool IS_DEBUGGING = false;

    [SerializeField] [CurveRange(0, 0, 1, 1)]
    private AnimationCurve m_rawCurve = null;
    [SerializeField] private float m_minValue = 0.0f;
    [SerializeField] private float m_maxValue = 1.0f;
    [SerializeField] [Min(0.0001f)] private float m_timeScale = 1.0f;

    public float minValue => m_minValue;
    public float maxValue => m_maxValue;
    public float timeScale => m_timeScale;

    public BetterCurve()
    {
        m_rawCurve = new AnimationCurve();
        m_minValue = 0.0f;
        m_maxValue = 1.0f;
        m_timeScale = 1.0f;
    }
    public BetterCurve(AnimationCurve animCurve, float min, float max, float timeScale)
    {
        m_rawCurve = animCurve;
        m_minValue = min;
        m_maxValue = max;
        m_timeScale = timeScale;
    }
    public float Evaluate(float time)
    {
        float temp_scaledTime = time * m_timeScale;
        #region Logs
        CustomDebug.LogForObject($"Given time {time} scaled to " +
            $"{temp_scaledTime}", this, IS_DEBUGGING);
        #endregion Logs
        #region Asserts
        Assert.IsTrue(temp_scaledTime >= 0 && temp_scaledTime <= 1,
            $"{GetType().Name}'s {nameof(Evaluate)} expected given time " +
            $"({time}) to be in the range 0-1 once scaled with {m_timeScale}");
        #endregion Asserts
        float temp_rawTime = m_rawCurve.Evaluate(temp_scaledTime);
        return temp_rawTime * (m_maxValue - m_minValue) + m_minValue;
    }
    public float GetEndTime()
    {
        if (m_rawCurve.length <= 0)
        {
            Debug.LogError($"{GetType().Name}'s curve has no points");
            return 0.0f;
        }

        float temp_rawTime = m_rawCurve.keys[m_rawCurve.length - 1].time;
        float temp_scaledTime = temp_rawTime / m_timeScale;
        #region Logs
        CustomDebug.LogForObject($"End time calcualted to be {temp_scaledTime}.",
            this, IS_DEBUGGING);
        #endregion Logs
        return temp_scaledTime;
    }
}
