using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DuolBots
{
    public class IconData : MonoBehaviour
    {
        [SerializeField]
        private eInputType m_InputType;
        [SerializeField]
        private byte m_playerIndex;
        [SerializeField]
        private byte m_teamIndex;
        [SerializeField]
        private byte m_slotIndex;
        [SerializeField]
        private bool m_hasCooldown;

        public void SetInputType(eInputType _inputType)
        {
            m_InputType = _inputType;
        }
        public eInputType GetInputType()
        {
            return m_InputType;
        }
        public void SetPlayerIndex(byte _playerIndex)
        {
            m_playerIndex = _playerIndex;
        }
        public byte GetPlayerIndex()
        {
            return m_playerIndex;
        }
        public void SetSlotIndex(byte _slotIndex)
        {
            m_slotIndex = _slotIndex;
        }
        public byte GetSlotIndex()
        {
            return m_slotIndex;
        }
        public void SetTeamIndex(byte _teamIndex)
        {
            m_teamIndex = _teamIndex;
        }
        public byte GetTeamIndex()
        {
            return m_teamIndex;
        }
        public void SetHasCooldown(bool _hasCooldown)
        {
            m_hasCooldown = _hasCooldown;
        }
        public bool GetHasCooldown()
        {
            return m_hasCooldown;
        }


    }

}
