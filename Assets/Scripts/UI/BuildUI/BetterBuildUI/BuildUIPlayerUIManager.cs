using UnityEngine;

using NaughtyAttributes;
// Original Authors - Eslis Vang and Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Individual UI Manager controlled by <see cref="BuildUIUIManager"/>
    /// for each individual player.
    /// </summary>
    public class BuildUIPlayerUIManager : MonoBehaviour
    {
        [SerializeField] [Required]
        private PreviewImageController m_prevImgCont = null;
        [SerializeField] private GameObject[] m_partStateObjects = null;


        public void ChangeToChassisMoveUI()
        {
            m_prevImgCont.ToggleActive(true);

            ToggleObjects(ref m_partStateObjects, false);
        }
        public void ChangeToPartUI()
        {
            ToggleObjects(ref m_partStateObjects, true);

            m_prevImgCont.ToggleActive(false);
        }


        private void ToggleObjects(ref GameObject[] objs, bool cond)
        {
            foreach (GameObject temp_o in objs)
            {
                temp_o.SetActive(cond);
            }
        }
    }
}
