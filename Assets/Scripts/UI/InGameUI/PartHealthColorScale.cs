using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
// Original Authors - Shelby Vian
// Tweaked a little by Wyatt Senalik (for networking purposes)

namespace DuolBots
{
    public class PartHealthColorScale : MonoBehaviour
    {
        private const bool IS_DEBUGGING = false;

        [SerializeField] private PartHealth m_health = null;
        public float m_startHealth, m_curHealth;
        [SerializeField] GameObject partImage, inactiveOverlay;
        [SerializeField] [Tag] private string m_partTag = "Part";
        [SerializeField] private eMultiplayerOption m_networkOption = eMultiplayerOption.Local;
        private PlayerIndex m_playerIndex = null;
        private SetImageTextures m_setImgTex = null;


        // Domestic Initialization
        private void Awake()
        {
            m_playerIndex = GetComponent<PlayerIndex>();
            m_setImgTex = GetComponentInChildren<SetImageTextures>();

            #region Asserts
            CustomDebug.AssertComponentIsNotNull(m_playerIndex, this);
            CustomDebug.AssertComponentInChildrenIsNotNull(m_setImgTex, this);
            #endregion Asserts

        }
        // Foreign Initialization
        private void Start()
        {
            PlayerIndex temp_parentPlayerIndex
                = transform.parent.GetComponent<PlayerIndex>();

            #region Asserts
            CustomDebug.AssertComponentOnOtherIsNotNull(temp_parentPlayerIndex,
                transform.parent.gameObject, this);
            #endregion Asserts

            m_playerIndex.playerIndex = temp_parentPlayerIndex.playerIndex;

            // Find my bot root
            RobotHelpersSingleton temp_botHelpers = RobotHelpersSingleton.instance;
            GameObject temp_myBotRoot;
            switch (m_networkOption)
            {
                case eMultiplayerOption.Local:
                    temp_myBotRoot = temp_botHelpers.FindBotRoot(0);
                    break;
                case eMultiplayerOption.Networked:
                    temp_myBotRoot = temp_botHelpers.FindOwnedBotRootNetwork();
                    break;
                default:
                    CustomDebug.UnhandledEnum(m_networkOption, this);
                    temp_myBotRoot = null;
                    break;
            }

            SlotIndex[] temp_mySlots
                = temp_myBotRoot.GetComponentsInChildren<SlotIndex>();

            #region Debugs
            CustomDebug.Log($"{name}'s {GetType().Name}'s team index is " +
                $"{temp_myBotRoot.GetComponent<ITeamIndex>().teamIndex}",
                IS_DEBUGGING);
            CustomDebug.Log($"{name}'s {GetType().Name} found " +
                $"{temp_mySlots.Length} slots that belong to them", IS_DEBUGGING);
            CustomDebug.Log($"Searching for a part with index " +
                    $"{m_setImgTex.partSlot}", IS_DEBUGGING);
            #endregion Debugs
            foreach (SlotIndex temp_slotIndex in temp_mySlots)
            {
                if (temp_slotIndex.slotIndex == m_setImgTex.partSlot)
                {
                    m_health = temp_slotIndex.GetComponentInChildren<PartHealth>();

                    #region Asserts
                    CustomDebug.AssertComponentInChildrenOnOtherIsNotNull(m_health,
                        temp_slotIndex.gameObject, this);
                    #endregion Asserts
                }
            }

            m_startHealth = m_health.GetMaxHealth();
            m_curHealth = m_startHealth;
            partImage.GetComponent<Image>().color = Color.green;
        }

        void Update()//change to parthealth event
        {
            m_curHealth = m_health.GetCurrentHealth();

            partImage.GetComponent<Image>().fillAmount = m_curHealth / m_startHealth;

            float percHealthLeft = (m_curHealth / m_startHealth) * 10;

            if (percHealthLeft <= 0)
            {
                partImage.GetComponent<Image>().color = Color.grey;
                SetInactiveOverlay(true);
            }
            else
            {

                if (percHealthLeft >= 5)
                {
                    //for every 10% of damage taken, (x, 1, 0, 1) increases by 0.2
                    partImage.GetComponent<Image>().color = new Color(0.2f * (10 - percHealthLeft), 1, 0, 1);
                } else
                {
                    //for every 10% of damage taken, (1, x, 0, 1) decreases by 0.2
                    partImage.GetComponent<Image>().color = new Color(1, 0.2f * percHealthLeft, 0, 1);
                }
            }
        }

       public void SetInactiveOverlay(bool state)
        {
            if (inactiveOverlay.activeSelf != state)
                inactiveOverlay.SetActive(state);
        }

    }
}
