using UnityEngine;
using UnityEngine.Assertions;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Expects to be spawned with authority on a team's client.
    /// NOT USED.
    /// </summary>
    [RequireComponent(typeof(TeamIndex))]
    public class ClientAuthorityTeamManager : MonoBehaviour
    {
        [SerializeField] [Tag] private string m_robotTag = "Robot";

        // References
        private TeamIndex m_teamIndex = null;


        /// <summary>Index for the team this is managing.</summary>
        public byte teamIndex => m_teamIndex.teamIndex;
        /// <summary>Reference to the GameObject of this team's bot.</summary>
        public GameObject botObject { get; private set; }


        // Called 0th
        // Domestic Intialization
        private void Awake()
        {
            m_teamIndex = GetComponent<TeamIndex>();
            Assert.IsNotNull(m_teamIndex, $"{name}'s {GetType().Name} requires " +
                $"{nameof(TeamIndex)} but none was found.");
        }


        /// <summary>
        /// Initializes this ClientAuthorityTeamManager with the team index.
        ///
        /// Pre Condtions - Assumes m_teamIndex is not null.
        /// Post Conditions - Sets the m_teamIndex's teamIndex to be
        /// <paramref name="indexOfTeam"/>. Searches the scene and caches a reference
        /// to this team's bot.
        /// </summary>
        /// <param name="indexOfTeam">Index of the team this manager should
        /// manage.</param>
        public void Initialize(byte indexOfTeam)
        {
            m_teamIndex.teamIndex = indexOfTeam;

            CacheReferenceToMyTeamRobot();
        }


        /// <summary>
        /// Finds the robot in the scene with this team's team index and caches
        /// a reference to it in the botObject property.
        /// 
        /// Pre Conditions - Assumes m_teamIndex is not null. Assumes that there is
        /// one and only one robot in the scene with this team's team index.
        /// Post Conditions - Sets botObject to be a reference to
        /// this team's bot's GameObject.
        /// </summary>
        private void CacheReferenceToMyTeamRobot()
        {
            // Get all the robots in the scene.
            GameObject[] temp_robotObjArr =
                GameObject.FindGameObjectsWithTag(m_robotTag);
            Assert.IsTrue(temp_robotObjArr.Length > 0, $"There are no objects " +
                $"with tag={m_robotTag} in the scene for this ({name})'s " +
                $"{GetType().Name}");

            // Check all the bots to search for this team's.
            foreach (GameObject temp_singleBot in temp_robotObjArr)
            {
                ITeamIndex temp_curTeamIndex = temp_singleBot.
                    GetComponent<ITeamIndex>();

                if (temp_curTeamIndex.teamIndex == teamIndex)
                {
                    Assert.IsNull(botObject, $"{nameof(botObject)} was not null " +
                        $"when searching for a robot with team index {teamIndex}");
                    // Found our bot.
                    botObject = temp_singleBot;
                }
            }

            Debug.LogError($"{name}'s {GetType().Name} found no robot " +
                $"with {teamIndex} in the scene");
        }
    }
}
