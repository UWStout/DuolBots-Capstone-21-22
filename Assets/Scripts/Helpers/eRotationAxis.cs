using UnityEngine;
// Original Authors - Wyatt Senalik

public enum eRotationAxis { x, y, z }


/// <summary>
/// Extensions for eRotationAxis.
/// </summary>
public static class eRotationAxisExtensions
{
    /// <summary>
    /// Gives the corresponding Vector3 direction vector.
    /// x => Vector3.right;
    /// y => Vector3.up;
    /// z => Vector3.forward;
    /// </summary>
    public static Vector3 GetCorrespondingVector(this eRotationAxis curRotAxis)
    {
        switch (curRotAxis)
        {
            case eRotationAxis.x:
                return Vector3.right;
            case eRotationAxis.y:
                return Vector3.up;
            case eRotationAxis.z:
                return Vector3.forward;
            default:
                Debug.LogError($"Unspecified {nameof(eRotationAxis)} " +
                    $"of value {curRotAxis}");
                return Vector3.zero;
        }
    }
    public static float GetValueFromEulerAngles(this eRotationAxis rotAxis,
        Vector3 eulerAngles)
    {
        switch (rotAxis)
        {
            case eRotationAxis.x:
                return eulerAngles.x;
            case eRotationAxis.y:
                return eulerAngles.y;
            case eRotationAxis.z:
                return eulerAngles.z;
            default:
                Debug.LogError($"Unspecified {nameof(eRotationAxis)} " +
                    $"of value {rotAxis}");
                return Mathf.NegativeInfinity;
        }
    }

    public static Vector3 ReplaceValueInEulerAngles(this eRotationAxis rotAxis,
        Vector3 eulerAngles, float value)
    {
        switch (rotAxis)
        {
            case eRotationAxis.x:
                eulerAngles.x = value;
                break;
            case eRotationAxis.y:
                eulerAngles.y = value;
                break;
            case eRotationAxis.z:
                eulerAngles.z = value;
                break;
            default:
                Debug.LogError($"Unspecified {nameof(eRotationAxis)} " +
                    $"of value {rotAxis}");
                break;
        }

        return eulerAngles;
    }
}
