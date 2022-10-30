using UnityEngine;
using UnityEngine.UI;

using NaughtyAttributes;
// Original Authors - Eslis Vang and Wyatt Senalik

namespace DuolBots
{
    [RequireComponent(typeof(PartSelectPlayerSelection))]
    public class PartSelectUIImageController : MonoBehaviour
    {
        [SerializeField] [Required] private RawImage m_selectedImage = null;
        // Image lists start from the selected and go outward.
        // Left images tend rightward and right images tend leftward.
        [SerializeField] private RawImage[] m_leftImgs = new RawImage[3];
        [SerializeField] private RawImage[] m_rightImgs = new RawImage[3];

        private BetterBuildSceneStateManager m_stateMan = null;
        private PartSelectPlayerSelection m_playerSel = null;

        private BetterBuildSceneStateChangeHandler m_partHandler = null;


        // Domestic Initialization
        private void Awake()
        {
            m_playerSel = GetComponent<PartSelectPlayerSelection>();
            #region Asserts
            CustomDebug.AssertSerializeFieldIsNotNull(m_selectedImage,
                nameof(m_selectedImage), this);
            CustomDebug.AssertComponentIsNotNull(m_playerSel, this);
            #endregion Asserts
        }
        // Foreign Initialization
        private void Start()
        {
            m_stateMan = BetterBuildSceneStateManager.instance;
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_stateMan, this);
            #endregion Asserts
            m_partHandler = new BetterBuildSceneStateChangeHandler(m_stateMan,
                BeginPartHandler, null, eBetterBuildSceneState.Part);
        }
        // Subscribe
        private void OnEnable()
        {
            m_playerSel.onSelectedIndexChanged += UpdateImages;
        }
        // Unsubscribe
        private void OnDisable()
        {
            if (m_playerSel != null)
            {
                m_playerSel.onSelectedIndexChanged -= UpdateImages;
            }
        }
        private void OnDestroy()
        {
            m_partHandler.ToggleActive(false);
        }


        private void BeginPartHandler()
        {
            UpdateImages(0);
        }
        /// <summary>
        /// Updates the RawImages to display the part images with
        /// the middle being the selected.
        /// </summary>
        private void UpdateImages(int index)
        {
            // Selected image
            PartScriptableObject temp_selPartSO
                = m_playerSel.GetCurrentlySelectedPartSO();
            Texture temp_selPartTex = temp_selPartSO.partUIData.unlockedSprite;
            m_selectedImage.texture = temp_selPartTex;
            // Left images
            for (int i = 0; i < m_leftImgs.Length; ++i)
            {
                UpdateSingleImage(i, ref m_leftImgs, -(i + 1));
            }
            // Right images
            for (int i = 0; i < m_rightImgs.Length; ++i)
            {
                UpdateSingleImage(i, ref m_rightImgs, i + 1);
            }
        }
        private void UpdateSingleImage(int imgIndex, ref RawImage[] imageList,
            int awayIndex)
        {
            #region Asserts
            CustomDebug.AssertIndexIsInRange(imgIndex, imageList, this);
            #endregion Asserts
            RawImage temp_img = imageList[imgIndex];
            PartScriptableObject temp_partSO
                = m_playerSel.GetPartSOAwayFromCurrentlySelected(awayIndex);
            Texture temp_partTex = temp_partSO.partUIData.unlockedSprite;
            temp_img.texture = temp_partTex;
        }
    }
}
