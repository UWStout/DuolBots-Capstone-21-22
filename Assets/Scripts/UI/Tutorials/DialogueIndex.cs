
using UnityEngine;

namespace DuolBots
{
    public class DialogueIndex : MonoBehaviour
    {
        [SerializeField] private int m_dialogueIndex = -1;
        public int dialogueIndex
        {
            get => m_dialogueIndex;
            set => m_dialogueIndex = value;
        }
    }
}

