using UnityEngine;
using UnityEngine.Assertions;
// Original Authors - Wyatt Senalik and Ben Lussman

namespace DuolBots
{
    [RequireComponent(typeof(Shared_IconManager))]
    public class Local_IconManager : MonoBehaviour
    {
        private Shared_IconManager m_sharedController = null;


        private void Awake()
        {
            m_sharedController = GetComponent<Shared_IconManager>();
            Assert.IsNotNull(m_sharedController, $"{name}'s {GetType().Name} " +
                $"requires {typeof(Shared_IconManager)} be attached but none was found.");
        }
        private void Start()
        {
            m_sharedController.SetupBotIcons();
        }
    }
}
