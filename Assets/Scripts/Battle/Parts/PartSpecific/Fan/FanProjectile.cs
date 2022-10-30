using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;
// Original Author - Aaron Duffey

namespace DuolBots
{
    /// <summary>
    /// Script that handles behavior for the Fan weapon's projectile
    /// (is toggled and NOT instantiated by FanProjectileFireController).
    /// </summary>
    public class FanProjectile : MonoBehaviour
    {
        // Constants
        private const bool IS_DEBUGGING = false;

        [SerializeField] [Tag] private string m_partTag = "Part";
        [SerializeField] [Tag] private string m_partDamagableTag = "PartDamageable";
        [SerializeField] [Tag] private string m_projectileTag = "Projectile";
        [SerializeField] [Tag] private string m_robotTag = "Robot";
        [SerializeField] [Min(1.0f)] private float m_robotPushIncrease = 50.0f;

        private float m_fanEffectDelay = 0.1f;
        private float m_fanForce = 5.0f;
        public float fanForce { set { m_fanForce = value; } }
        // Index of the team that fired the Fan weapon.
        private ITeamIndex m_teamIndex = null;

        // Relation of affected GameObjects and their cooldowns.
        private Dictionary<GameObject, float> m_affectedGameObjects
            = new Dictionary<GameObject, float>();


        // Domestic Initialization
        private void Awake()
        {
            m_teamIndex = GetComponentInParent<ITeamIndex>();
            #region Asserts
            CustomDebug.AssertIComponentInParentIsNotNull(m_teamIndex, this);
            #endregion Asserts
        }
        private void OnTriggerStay(Collider other)
        {
            // If the tag aren't any of the ones we are looking for
            if (!other.CompareTag(m_partTag) &&
                !other.CompareTag(m_partDamagableTag) &&
                !other.CompareTag(m_projectileTag))
            {
                #region Logs
                CustomDebug.LogForComponent($"{other.name} is not of the tag we are " +
                    $"looking for", this, IS_DEBUGGING);
                #endregion Logs
                return;
            }
            // Check that the object is not in the affected list (if so,
            // it has a delay before it can be affected again)
            if (m_affectedGameObjects.ContainsKey(other.gameObject))
            {
                #region Logs
                CustomDebug.LogForComponent($"{other.name} has already received " +
                    $"a force recently", this, IS_DEBUGGING);
                #endregion Logs
                return;
            }
            //// Try to pull the team index off the object
            //ITeamIndex temp_index = GetComponentInParent<ITeamIndex>();
            //if (temp_index == null) { return; }
            //if (temp_index == m_teamIndex.teamIndex) { return; }

            // Find rigidbody.
            Rigidbody temp_rigidBody = other.attachedRigidbody;
            // No rigidbody, might be laser or something else that isn't
            // physics based.
            if (temp_rigidBody == null)
            {
                #region Logs
                CustomDebug.LogForComponent($"{other.name} does not have a " +
                    $"rigidbody", this, IS_DEBUGGING);
                #endregion Logs
                return;
            }
            // If there is a rigidbody, make sure it isn't for our bot root
            if (IsMyTeamsBotRootsRigidbody(temp_rigidBody))
            {
                #region Logs
                CustomDebug.LogForComponent($"{other.name} is my bot root",
                    this, IS_DEBUGGING);
                #endregion Logs
                return;
            }

            // Apply force
            #region Logs
            CustomDebug.LogForComponent($"Applying force for {temp_rigidBody.name}",
                this, IS_DEBUGGING);
            #endregion Logs
            Vector3 temp_forceToApply = transform.forward * m_fanForce;
            bool temp_isRobot = temp_rigidBody.gameObject.CompareTag(m_robotTag);
            temp_forceToApply *= temp_isRobot ? m_robotPushIncrease : 1.0f;
            temp_rigidBody.AddForce(temp_forceToApply);

            // Since force was applied properly, the Collider can be added
            // with a cooldown to m_objectsAffectedByFan
            KeyValuePair<GameObject, float> temp_gameObjectAffected
                = new KeyValuePair<GameObject, float>(
                    other.gameObject, m_fanEffectDelay);
            m_affectedGameObjects.Add(temp_gameObjectAffected.Key,
                temp_gameObjectAffected.Value);
            StartCoroutine(WaitForFanDelay(temp_gameObjectAffected));
        }
        private bool IsMyTeamsBotRootsRigidbody(Rigidbody rb)
        {
            // It isn't even a robot
            if (!rb.gameObject.CompareTag(m_robotTag)) { return false; }

            ITeamIndex temp_teamIndex = rb.GetComponent<ITeamIndex>();
            #region Asserts
            CustomDebug.AssertIComponentIsNotNull(temp_teamIndex, this);
            #endregion Asserts
            // Team index isn't for our team
            if (temp_teamIndex.teamIndex != m_teamIndex.teamIndex) { return false; }

            // It is our robot's rigidbody.
            return true;
        }
        private IEnumerator WaitForFanDelay(
            KeyValuePair<GameObject,float> affectedGO)
        {
            yield return new WaitForSeconds(affectedGO.Value);
            m_affectedGameObjects.Remove(affectedGO.Key);
        }
    }
}
