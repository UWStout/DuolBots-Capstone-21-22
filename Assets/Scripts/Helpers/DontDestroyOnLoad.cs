using UnityEngine;
// Original Author - Wyatt Senalik

/// <summary>
/// Simple script to make an object persist through scenes.
/// </summary>
public class DontDestroyOnLoad : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
