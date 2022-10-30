using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Option for the <see cref="ChassisMoveSelectOnReadyUp"/>.
    /// Groups all the related information for the current
    /// chassis option together.
    /// </summary>
    [Serializable]
    public class SingleChassisMoveOption
    {
        // Bot instantiators for creating the movement bots.
        [SerializeField] [Required]
        private BetterBuildUIBotInstantiator m_instantiator = null;
        // Animators for playing animations per chassis->movement option.
        [SerializeField] [Required]
        private BetterBuildBotAnimatorController m_animCont = null;
        // Starting chassis choices available.
        // Will be destroyed once a chassis is chosen.
        [SerializeField] [Required] private GameObject m_chassisBotRoot = null;
        // The string id for the available chassis.
        [SerializeField] [Required] private StringID m_chassisID = null;
        // The string id for the available movement choices.
        [SerializeField] [Required] private StringID m_movementID = null;

        // Movement bot root once its created.
        // Will be created and set after a chassis is chosen.
        // If no the chosen one, will be destroyed after movement is chosen.
        private GameObject m_movementBotRoot = null;


        public BetterBuildUIBotInstantiator instantiator => m_instantiator;
        public BetterBuildBotAnimatorController animCont => m_animCont;
        public GameObject chassisBotRoot => m_chassisBotRoot;
        public GameObject movementBotRoot
        {
            get => m_movementBotRoot;
            set => m_movementBotRoot = value;
        }
        public string chassisID
        {
            get
            {
                Assert.IsNotNull(m_chassisID, $"{nameof(m_chassisID)} not " +
                    $"specified for {GetType().Name}");
                return m_chassisID.value;
            }
        }
        public string movementID
        {
            get
            {
                Assert.IsNotNull(m_movementID, $"{nameof(m_movementID)} not " +
                    $"specified for {GetType().Name}");
                return m_movementID.value;
            }
        }
    }
}
