using UnityEngine;
// Original Authors - Wyatt Senalik

/// <summary>
/// Data held in a transform (Position, Rotation, and Scale).
/// </summary>
public class TransformData
{
    private readonly Vector3 m_position = Vector3.zero;
    private readonly Quaternion m_rotation = Quaternion.identity;
    private readonly Vector3 m_scale = Vector3.one;

    public Vector3 position => m_position;
    public Quaternion rotation => m_rotation;
    public Vector3 scale => m_scale;


    public TransformData()
    {
        m_position = Vector3.zero;
        m_rotation = Quaternion.identity;
        m_scale = Vector3.one;
    }
    public TransformData(Vector3 pos, Quaternion rot, Vector3 size)
    {
        m_position = pos;
        m_rotation = rot;
        m_scale = size;
    }


    /// <summary>
    /// Creates <see cref="TransformData"/> holding position, rotation,
    /// and localScale.
    /// </summary>
    /// <param name="transform">Transform to create the data from.</param>
    public static TransformData CreateGlobalTransformData(Transform transform)
    {
        return new TransformData(transform.position, transform.rotation,
            transform.localScale);
    }
    /// <summary>
    /// Creates <see cref="TransformData"/> holding localPosition,
    /// localRotation, and localScale.
    /// </summary>
    /// <param name="transform">Transform to create the data from.</param>
    public static TransformData CreateLocalTransformData(Transform transform)
    {
        return new TransformData(transform.localPosition,
            transform.localRotation, transform.localScale);
    }


    /// <summary>
    /// Applies the <see cref="TransformData"/>'s position, rotation,
    /// and scale to the given <see cref="Transform"/>'s position, rotation,
    /// and localScale respectively.
    /// </summary>
    /// <param name="transform">Transform to apply
    /// the <see cref="TransformData"/> to.</param>
    public void ApplyGlobal(Transform transform)
    {
        transform.position = position;
        transform.rotation = rotation;
        transform.localScale = scale;
    }
    /// <summary>
    /// Applies the <see cref="TransformData"/>'s position, rotation,
    /// and scale to the given <see cref="Transform"/>'s localPosition,
    /// localRotation, and localScale respectively.
    /// </summary>
    /// <param name="transform">Transform to apply
    /// the <see cref="TransformData"/> to.</param>
    public void ApplyLocal(Transform transform)
    {
        transform.localPosition = position;
        transform.localRotation = rotation;
        transform.localScale = scale;
    }
    /// <summary>
    /// Compares the given <see cref="TransformData"/> to this
    /// <see cref="TransformData"/> and returns false if any changes
    /// are outside the specified tolerance.
    /// If all changes fall within the tolerance, returns true.
    /// </summary>
    /// <param name="data">Other <see cref="TransformData"/>
    /// to compare this against.</param>
    /// <param name="positionTolerance">Amount of changes to allow without
    /// returning true. Checks this against x, y, and z separately.</param>
    /// <param name="rotationTolerance">Amount of changes to allow without
    /// returning true. Checks this against x, y, and z separately.</param>
    /// <param name="scaleTolerance">Amount of changes to allow without
    /// returning true. Checks this against x, y, and z separately.</param>
    /// <returns>If the <see cref="TransformData"/>s are close enough
    /// to one another to be considered the same.</returns>
    public bool Compare(TransformData data, float positionTolerance,
        float rotationTolerance, float scaleTolerance)
    {
        // Position
        float xPosDiff = Mathf.Abs(position.x - data.position.x);
        if (xPosDiff > positionTolerance) { return false; }
        float yPosDiff = Mathf.Abs(position.y - data.position.y);
        if (yPosDiff > positionTolerance) { return false; }
        float zPosDiff = Mathf.Abs(position.z - data.position.z);
        if (zPosDiff > positionTolerance) { return false; }
        // Rotation
        float xRotDiff = Mathf.Abs(rotation.x - data.rotation.x);
        if (xRotDiff > rotationTolerance) { return false; }
        float yRotDiff = Mathf.Abs(rotation.y - data.rotation.y);
        if (yRotDiff > rotationTolerance) { return false; }
        float zRotDiff = Mathf.Abs(rotation.z - data.rotation.z);
        if (zRotDiff > rotationTolerance) { return false; }
        float wRotDiff = Mathf.Abs(rotation.w - data.rotation.w);
        if (wRotDiff > rotationTolerance) { return false; }
        // Scale
        float xScaleDiff = Mathf.Abs(scale.x - data.scale.x);
        if (xScaleDiff > scaleTolerance) { return false; }
        float yScaleDiff = Mathf.Abs(scale.y - data.scale.y);
        if (yScaleDiff > scaleTolerance) { return false; }
        float zScaleDiff = Mathf.Abs(scale.z - data.scale.z);
        if (zScaleDiff > scaleTolerance) { return false; }
        // Everything is within the tolerance
        return true;
    }

    public override string ToString()
    {
        return $"{GetType().Name} ({nameof(position)}={position}) " +
            $"({nameof(rotation)}={rotation}) ({nameof(scale)}={scale})";
    }
}
