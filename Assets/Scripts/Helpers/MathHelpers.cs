using UnityEngine;
// Original Authors - Wyatt Senalik and Eslis Vang

/// <summary>
/// Collection of math helpers functions.
/// </summary>
public static class MathHelpers
{
    /// <summary>
    /// Finds the minimum value from the given values,
    /// ignoring the sign. 
    /// </summary>
    public static float MinIgnoreSign(params float[] values)
    {
        if (values.Length == 0)
        {
            Debug.LogError("Array with length 0 passed into function.");
            return 0f;
        }

        int temp_minValueIndex = 0;
        float temp_minValue = Mathf.Abs(values[temp_minValueIndex]);
        for (int i = 1; i < values.Length; i++)
        {
            float temp_curVal = values[i];
            float temp_curAbsVal = Mathf.Abs(temp_curVal);
            if (temp_curAbsVal < temp_minValue)
            {
                temp_minValueIndex = i;
                temp_minValue = temp_curVal;
            }
        }

        return values[temp_minValueIndex];
    }
}
