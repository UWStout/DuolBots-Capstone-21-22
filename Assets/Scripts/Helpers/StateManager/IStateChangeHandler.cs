// Original Authors - Wyatt Senalik

/// <summary>
/// Interface for <see cref="StateChangeHandler{TEnum}"/>.
/// </summary>
public interface IStateChangeHandler
{
    public void ToggleActive(bool cond);
}
