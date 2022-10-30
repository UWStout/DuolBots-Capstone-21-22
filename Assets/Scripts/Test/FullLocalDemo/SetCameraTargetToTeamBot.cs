using UnityEngine;
using UnityEngine.Assertions;

using Cinemachine;
using NaughtyAttributes;

using DuolBots.Mirror;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Sets the specified camera's target and look at to be the bot with corresponding
    /// team index as this.
    /// </summary>
    public class SetCameraTargetToTeamBot : MonoBehaviour
    {
        public enum eInitializeCameraTime { OnCreateBotFinished, OnAllBotsCreated, Start }

        private const bool IS_DEBUGGING = false;

        // Camera whose target and lookat we will set to the robot
        [SerializeField] [Required] private CinemachineFreeLook m_freeLookCam = null;

        [SerializeField] [Tag] private string m_robotTag = "Robot";
        [SerializeField] eInitializeCameraTime m_initializeCameraTime
            = eInitializeCameraTime.OnCreateBotFinished;

        private ITeamIndex m_teamIndex = null;


        // Domestic Initialization
        private void Awake()
        {
            Assert.IsNotNull(m_freeLookCam, $"{nameof(SetCameraTargetToTeamBot)} requires " +
                $"that a {nameof(CinemachineFreeLook)} be specified, but none was");

            m_teamIndex = GetComponent<ITeamIndex>();
        }
        // Called when the component is activated.
        private void OnEnable()
        {
            if (m_initializeCameraTime == eInitializeCameraTime.OnCreateBotFinished)
            {
                BattleBotInstantiator.onCreateBotFinished += InitializeCameraOnCreateBotFinished;
            }
            else if (m_initializeCameraTime == eInitializeCameraTime.OnAllBotsCreated)
            {
                NetworkBotInstantiator.ToggleSubscriptionToOnAllBotsInstantiated(
                    InitializeCameraOnAllBotsCreatedServer, true);
            }
        }
        // Called when the component is de-activated.
        private void OnDisable()
        {
            if (m_initializeCameraTime == eInitializeCameraTime.OnCreateBotFinished)
            {
                BattleBotInstantiator.onCreateBotFinished -= InitializeCameraOnCreateBotFinished;
            }
            else if (m_initializeCameraTime == eInitializeCameraTime.OnAllBotsCreated)
            {
                NetworkBotInstantiator.ToggleSubscriptionToOnAllBotsInstantiated(
                    InitializeCameraOnAllBotsCreatedServer, false);
            }
        }
        private void Start()
        {
            if (m_initializeCameraTime == eInitializeCameraTime.Start)
            {
                InitializeCameraOnCreateBotFinished(m_teamIndex.teamIndex);
            }
        }


        /// <summary>
        /// Sets the specified camera to be looking at the robot on my team.
        /// 
        /// Pre Conditions - Assumes FindRobotOnMyTeam's Pre Conditions are met.
        /// Assumes m_freeLookCam is not null.
        /// Post Conditions - Sets the specified camera to be looking at and following
        /// the robot.
        /// </summary>
        private void InitializeCameraOnCreateBotFinished(byte botTeamIndex)
        {
            if (botTeamIndex != m_teamIndex.teamIndex) { return; }

            // Assumes that this executes after the BotInstantiation
            GameObject temp_myTeamRobotRoot = FindRobotOnMyTeam();
            Assert.IsNotNull(temp_myTeamRobotRoot, $"Could not find a robot on my team");
            m_freeLookCam.LookAt = temp_myTeamRobotRoot.transform;
            m_freeLookCam.Follow = temp_myTeamRobotRoot.transform;
        }
        private void InitializeCameraOnAllBotsCreatedServer()
        {
            InitializeCameraOnCreateBotFinished(m_teamIndex.teamIndex);
        }


        /// <summary>
        /// Finds the bot with the team index equal to this object's team index.
        ///
        /// Pre Conditions - Assumes m_teamIndex is not null. Assumes all bots in the scene
        /// have the tag specified in m_robotTag. Assumes there is only one bot in the scene
        /// that shares the same team index as this object.
        /// Pot Conditions - Returns the GameObject of the bot in the scene that has the
        /// same team index as this object. Does not change anything in the scene or
        /// update any variables.
        /// </summary>
        private GameObject FindRobotOnMyTeam()
        {
            GameObject[] temp_robotObjList = GameObject.FindGameObjectsWithTag(m_robotTag);
            CustomDebug.Log($"{nameof(SetCameraTargetToTeamBot)}'s {nameof(FindRobotOnMyTeam)} found " +
                $"{temp_robotObjList.Length} robots in the scene", IS_DEBUGGING);
            foreach (GameObject temp_singleRobotObj in temp_robotObjList)
            {
                ITeamIndex temp_robotTeamIndex = temp_singleRobotObj.GetComponent<ITeamIndex>();
                Assert.IsNotNull(temp_robotTeamIndex, $"Did not have {nameof(ITeamIndex)} attached to" +
                    $" {temp_singleRobotObj.name} and {name}'s {nameof(SetCameraTargetToTeamBot)} requires it");
                if (m_teamIndex.teamIndex == temp_robotTeamIndex.teamIndex)
                {
                    return temp_singleRobotObj;
                }
            }

            Debug.LogError($"No Robot (GameObject with tag={m_robotTag}) " +
                $"was found with teamIndex={m_teamIndex.teamIndex}");
            return null;
        }
    }
}
