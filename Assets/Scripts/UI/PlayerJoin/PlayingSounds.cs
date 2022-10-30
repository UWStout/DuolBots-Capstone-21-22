using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;

namespace DuolBots
{
    public class PlayingSounds : MonoBehaviour
    {
        [SerializeField] private WwiseEventName m_readyUpSound = null;
        [SerializeField] private WwiseEventName m_moveMenuSound = null;
        [SerializeField] private WwiseEventName m_selectMenuSound = null;

        public void ReadyUpSound()
        {
            AkSoundEngine.PostEvent(m_readyUpSound.wwiseEventName, gameObject);
        }

        public void MoveMenuSound()
        {
            AkSoundEngine.PostEvent(m_moveMenuSound.wwiseEventName, gameObject);
            //Debug.Log("Is moving working??");
        }

        public void SelectSound()
        {
            AkSoundEngine.PostEvent(m_selectMenuSound.wwiseEventName, gameObject);
        }
    }
}
