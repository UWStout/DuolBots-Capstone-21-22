using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BetterCurveUnityEventAnimation : BetterCurveAnimation
{
    [SerializeField] private UnityEvent<float> m_curveActions =
        new UnityEvent<float>();

    protected override void TakeCurveAction(float curveValue)
    {
        m_curveActions.Invoke(curveValue);
    }
}
