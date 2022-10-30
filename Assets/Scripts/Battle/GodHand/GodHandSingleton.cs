using System.Collections;
using UnityEngine;

namespace DuolBots
{
    public class GodHandSingleton : MonoBehaviour
    {
        private static GodHandSingleton m_instance;

        public static GodHandSingleton Instance { get { return m_instance; } }
        [SerializeField] private GameObject[] m_godHands = null;
        [SerializeField] private BotInLandingZone[] m_safePoints = null;

        private void Awake()
        {
            if (m_instance != null && m_instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                m_instance = this;
            }
        }

        /// <summary>
        /// This spawns a god hand below the given position and plays the animation
        /// </summary>
        /// <param name="botRootObj"></param>
        /// <param name="teamIndex"></param>
        public void SpawnPlayGodHand(GameObject botRootObj, byte teamIndex)
        {
            #region Asserts
            CustomDebug.AssertIndexIsInRange(teamIndex, m_godHands, this);
            #endregion Asserts
            GameObject temp_teamsGodHand = m_godHands[teamIndex];
            temp_teamsGodHand.transform.position = botRootObj.transform.position;
            BotInLandingZone temp_closestLandingZone = m_safePoints[0];
            float temp_maxSqDiff = Mathf.Infinity;
            foreach (BotInLandingZone temp_singSafePoint in m_safePoints)
            {
                if (!temp_singSafePoint.AreBotsInArea())
                {
                    float temp_sqDiff = (temp_singSafePoint.transform.position -
                        botRootObj.transform.position).sqrMagnitude;
                    if (temp_sqDiff < temp_maxSqDiff)
                    {
                        temp_maxSqDiff = temp_sqDiff;
                        temp_closestLandingZone = temp_singSafePoint;
                    }
                }
            }
            BattleFlipAnimation temp_flipAnim = temp_teamsGodHand.
                GetComponentInChildren<BattleFlipAnimation>();
            #region Asserts
            CustomDebug.AssertComponentInChildrenOnOtherIsNotNull(temp_flipAnim,
                temp_teamsGodHand, this);
            #endregion Asserts
            temp_flipAnim.Play();
            StartCoroutine(StartLerping(botRootObj, temp_closestLandingZone.transform));
        }

        private IEnumerator StartLerping(GameObject botRootObj,
            Transform closestLandingZone)
        {
            while (botRootObj.transform.position.y < 50)
            {
                Debug.Log("going up");
                yield return new WaitForEndOfFrame();
            }
            AdjustRotationInAir temp_adjRotInAir = botRootObj.
                GetComponent<AdjustRotationInAir>();
            #region Asserts
            CustomDebug.AssertComponentOnOtherIsNotNull(temp_adjRotInAir,
                botRootObj, this);
            #endregion Asserts
            temp_adjRotInAir.UpdateInfo(closestLandingZone);
        }
    }
}
