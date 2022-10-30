// Original Authors - Wyatt Senalik

public enum ePartType
{
    Chassis,
    Movement,
    Weapon,
    Utility,
    Other
}

public static class ePartTypeExtensions
{
    /// <summary>
    /// If the type is the type for a slotted part.
    /// </summary>
    public static bool IsSlottedPart(this ePartType partType)
    {
        if (partType == ePartType.Weapon) { return true; }
        if (partType == ePartType.Utility) { return true; }
        return false;
    }
}
