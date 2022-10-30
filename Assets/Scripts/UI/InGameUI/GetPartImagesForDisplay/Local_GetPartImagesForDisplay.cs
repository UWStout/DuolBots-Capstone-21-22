using UnityEngine;
using UnityEngine.Assertions;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    [RequireComponent(typeof(Shared_GetPartImagesForDisplay))]
    public class Local_GetPartImagesForDisplay : MonoBehaviour
    {
        private Shared_GetPartImagesForDisplay m_sharedController = null;


        // Called 0th
        // Domestic Intitialization
        private void Awake()
        {
            m_sharedController = GetComponent<Shared_GetPartImagesForDisplay>();
            Assert.IsNotNull(m_sharedController, $"{name}'s {GetType().Name} " +
                $"requires {nameof(Shared_GetPartImagesForDisplay)} but none " +
                $"was found");
        }
        // Called 1st
        private void Start()
        {
            // In the local variant, we just call this on start for team 0
            m_sharedController.InitializePartImagesForDisplay(0);
        }
    }
}
