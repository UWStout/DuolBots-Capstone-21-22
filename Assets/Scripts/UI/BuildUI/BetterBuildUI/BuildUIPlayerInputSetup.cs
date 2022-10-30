using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

using NaughtyAttributes;
// Original Authors - Eslis Vang and Wyatt Senalik

namespace DuolBots
{
    public class BuildUIPlayerInputSetup : MonoBehaviour
    {
        // The player input prefab to spawn
        [SerializeField] [Required] private GameObject m_playerPrefab = null;
        [SerializeField]
        private DollyTargetCycler[] m_playerCyclers = new DollyTargetCycler[0];

        private IReadOnlyList<PlayerInput> m_playerInputs = null;


        private void Awake()
        {
            // A BUNCH OF STUFF EXPECTS THIS TO BE IN AWAKE
            m_playerInputs = CurrentPlayerInputDevices.
                SpawnPlayerInputForEachDevice(m_playerPrefab);
            #region Asserts
            Assert.AreEqual(m_playerCyclers.Length, m_playerInputs.Count,
                $"Amount of specified cyclers {m_playerCyclers.Length} does not " +
                $"match the amount of spawned players " +
                $"{m_playerInputs.Count}.");
            #endregion Asserts

            InitializeDollyTargetCyclers();
        }
        // Foreign Initialization
        private void Start()
        {
            SplitPlayerCanvases();
        }


        private void InitializeDollyTargetCyclers()
        {
            for (int i = 0; i < m_playerInputs.Count; ++i)
            {
                PlayerInput temp_curPlayerInp = m_playerInputs[i];

                // Assign the dolly cyclers to the corresponding player input
                Input_DollyTargetCycler temp_inpForCycler =
                    temp_curPlayerInp.GetComponent<Input_DollyTargetCycler>();
                CustomDebug.AssertComponentIsNotNull(temp_inpForCycler, this,
                    temp_curPlayerInp.gameObject);
                DollyTargetCycler temp_corresCycler = m_playerCyclers[i];
                temp_inpForCycler.dollyTargetCycler = temp_corresCycler;
            }
        }
        private void SplitPlayerCanvases()
        {
            for (int i = 0; i < m_playerInputs.Count; ++i)
            {
                PlayerInput temp_curPlayerInp = m_playerInputs[i];

                // Initialize the canvas to be split right or left
                SplitCanvas temp_playerSplitCanvas = temp_curPlayerInp.
                    GetComponentInChildren<SplitCanvas>();
                CustomDebug.AssertComponentIsNotNull(temp_playerSplitCanvas, this,
                    temp_curPlayerInp.gameObject);

                PlayerIndex temp_playerIndex = temp_curPlayerInp.GetComponent<PlayerIndex>();
                CustomDebug.AssertComponentIsNotNull(temp_playerIndex, this,
                    temp_curPlayerInp.gameObject);

                temp_playerSplitCanvas.SplitAsPlayerCanvas(temp_playerIndex.playerIndex);
            }
        }
    }
}
