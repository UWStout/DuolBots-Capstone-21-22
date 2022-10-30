using System;
// Original Author - Aaron Duffey

/// <summary>
/// Interface for weapons that have a charge.
/// </summary>
public interface IChargeWeapon
{
    // Events describing whether the weapon is currently charging
    public event Action onStartedCharging;
    public event Action onFinishedCharging;
    // Events for when the weapon reaches and breaks out of being fully charged (firing the weapon after fully charging)
    public event Action onFullyChargedStart;
    public event Action onFullyChargedEnd;
}
