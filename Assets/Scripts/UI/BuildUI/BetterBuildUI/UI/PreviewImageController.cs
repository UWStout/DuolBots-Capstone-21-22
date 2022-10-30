using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

using NaughtyAttributes;
// Original Authors - Eslis Vang and Wyatt Senalik

namespace DuolBots
{
    public enum eImageState { Chassis, Movement }

    [RequireComponent(typeof(Image))]
    public class PreviewImageController : MonoBehaviour
    {
        private const bool IS_DEBUGGING = true;

        [SerializeField]
        [Required]
        private Input_DollyTargetCycler m_targetCyclerInp = null;
        [SerializeField][Required] private Image m_previewImage = null;
        [SerializeField] private Sprite[] m_chassisSprites = null;
        [SerializeField] private Sprite[] m_movementSprites = null;

        private bool m_isImageLocked = false;
        private int m_curSelIndex => m_targetCyclerInp.dollyTargetCycler.
            currentSelectedIndex;

        private eImageState m_curImageState = eImageState.Chassis;


        private void Awake()
        {
            CustomDebug.AssertComponentIsNotNull(m_previewImage, this);
            CustomDebug.AssertComponentIsNotNull(m_targetCyclerInp, this);
        }
        private void Start()
        {
            m_targetCyclerInp.dollyTargetCycler.onSelectionIndexChange +=
                UpdateImage;
        }
        private void OnDestroy()
        {
            if (m_targetCyclerInp == null) { return; }
            if (m_targetCyclerInp.dollyTargetCycler == null) { return; }
            m_targetCyclerInp.dollyTargetCycler.onSelectionIndexChange -=
                UpdateImage;
        }


        public void ToggleActive(bool cond)
        {
            gameObject.SetActive(cond);
        }
        public void UpdateImageToCurrentSelection()
        {
            UpdateImage(m_curSelIndex);
        }
        public void SetIsSilhouette(bool state)
        {
            m_previewImage.sprite = state ? DetermineSpriteFromCurrentState(
                m_curSelIndex + 3) : DetermineSpriteFromCurrentState(m_curSelIndex);
            //m_previewImage.color = state ? Color.black : Color.white;
        }
        public void LockImage(bool state)
        {
            m_previewImage.sprite = DetermineSpriteFromCurrentState(m_curSelIndex);
            m_isImageLocked = state;
        }
        public void SetImageState(eImageState newState)
        {
            m_curImageState = newState;
        }


        private void UpdateImage(int newIndex)
        {
            if (m_isImageLocked) return;

            int temp_index = newIndex + 3;

            Assert.IsTrue(temp_index < m_chassisSprites.Length
                && temp_index >= 0, $"{temp_index} is out of" +
                $"bounds for {name}'s {GetType().Name}. Must be between 0 and " +
                $"{m_chassisSprites.Length}");

            m_previewImage.sprite = DetermineSpriteFromCurrentState(
                temp_index);
        }
        private Sprite DetermineSpriteFromCurrentState(int index)
        {
            switch (m_curImageState)
            {
                case eImageState.Chassis:
                    return m_chassisSprites[index];
                case eImageState.Movement:
                    return m_movementSprites[index];
                default:
                    Debug.LogError($"Unhandled enum for {typeof(eImageState)} " +
                        $"of value {m_curImageState}");
                    return null;
            }
        }
    }
}
