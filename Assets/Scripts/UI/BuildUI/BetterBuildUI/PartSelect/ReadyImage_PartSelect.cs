using UnityEngine;

using NaughtyAttributes;
// Original Authors - Eslis Vang and Wyatt Senalik

namespace DuolBots
{
    public class ReadyImage_PartSelect : MonoBehaviour
    {
        [SerializeField] [Required] private GameObject m_readyObj = null;


        // Domestic Initialization
        private void Awake()
        {
            ToggleReady(false);
        }


        public void ToggleReady(bool cond)
        {
            m_readyObj.SetActive(cond);
        }
    }
}
