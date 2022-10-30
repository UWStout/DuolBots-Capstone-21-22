using UnityEngine;
using UnityEngine.Assertions;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik and Eslis Vang

namespace DuolBots
{
    public class ChassisMoveSelection_ISelection_DollyTargetCycler : MonoBehaviour,
        ISelection_DollyTargetCycler
    {
        private const bool IS_DEBUGGING = true;


        // Instance of a confirmation popup (not prefab)
        [SerializeField] [Required]
        private PopupController m_confirmationPopup = null;


        // Called 0th
        // Domestic Initialization
        private void Awake()
        {
            Assert.IsNotNull(m_confirmationPopup, $"{nameof(m_confirmationPopup)}" +
                $" is required but not specified.");
        }


        #region ISelection_DollyTargetCycler
        public void ConfirmSelection()
        {
            // Opens confirmation window
            m_confirmationPopup.Activate();
        }
        #endregion ISelection_DollyTargetCycler
    }
}
