using UnityEngine;
// Original Authors - Wyatt Senalik and Aaron Duffey

/// <summary>
/// Its 1:40 AM.
/// </summary>
public class ToggleGameObjectActive : MonoBehaviour
{
    public void ToggleActive()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
