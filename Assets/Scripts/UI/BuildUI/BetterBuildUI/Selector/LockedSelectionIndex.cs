using UnityEngine;
using NaughtyAttributes;

namespace DuolBots
{
    [RequireComponent(typeof(Input_DollyTargetCycler))]
    public class LockedSelectionIndex : MonoBehaviour
    {
        [SerializeField][ReadOnly] private int m_lockedSelectionIndex = -1;
        [SerializeField][ReadOnly] private DollyTargetCycler m_dolly = null;

        public int selectionIndex => m_lockedSelectionIndex;

        private void Start()
        {
            m_dolly = GetComponent<Input_DollyTargetCycler>().dollyTargetCycler;
        }

        public void SetSelectionIndex()
        {
            m_lockedSelectionIndex = m_dolly.currentSelectedIndex;
        }

        public void ResetIndex()
        {
            m_lockedSelectionIndex = -1;
        }
    }
}
