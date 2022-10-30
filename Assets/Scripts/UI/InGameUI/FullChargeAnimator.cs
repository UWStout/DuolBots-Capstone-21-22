using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using DuolBots;
//Original Author - Shelby Vian

/// <summary>
/// Triggers full charge animation on UI icon
/// </summary>
public class FullChargeAnimator : MonoBehaviour
{
    private Animator m_fireIcon;
    private IChargeWeapon m_weapon = null;
    [SerializeField] private byte m_playerIndex = 0;

    void Awake()
    {
        m_fireIcon = GetComponent<Animator>();
    }

    /// <summary>
    /// Sets animation parameter bool to true
    /// </summary>
    void StartAnimationState()
    {
        m_fireIcon.SetBool("fullCharge", true);
    }

    /// <summary>
    /// Sets animation parameter bool to false
    /// </summary>
    void EndAnimationState()
    {
        m_fireIcon.SetBool("fullCharge", false);
    }

    IChargeWeapon[] m_chargeWeapons = null;

    /// <summary>
    /// Passed the current active slot from ActivePartIconManager, searches for a charge
    /// type part in scene with correct slot index and subscribes fire icon to the weapon charge event
    /// </summary>
    /// <param name="slot">slot index of the current active part</param>
    /// <param name="teamIndex">team index</param>
    /// <param name="playerIndex">player being subscirbed to event</param>
    public void SetActivePart(byte slot, byte teamIndex, byte playerIndex)
    {
        #region Asserts
        CustomDebug.AssertIndexIsInRange(playerIndex, 0, 2, this);
        #endregion

        if (playerIndex == m_playerIndex)
        {
            if (m_weapon != null)
            {
                m_weapon.onFullyChargedStart -= StartAnimationState;
                m_weapon.onFullyChargedEnd -= EndAnimationState;
                m_weapon = null;
            }

            if (m_chargeWeapons == null)
            {
                GameObject temp_myBot = RobotHelpersSingleton.instance.FindBotRoot(teamIndex);

                m_chargeWeapons = FindObjectsOfType<MonoBehaviour>().OfType<IChargeWeapon>().ToArray();
            }

            #region Asserts
            if (m_chargeWeapons.Length == 0)
            {
                Debug.Log("No weapons with charge mechanic found.");
                return;
            }
            #endregion

            foreach (IChargeWeapon weapon in m_chargeWeapons)
            {
                GameObject temp_weaponObj = ((MonoBehaviour)weapon).gameObject;

                if (temp_weaponObj.GetComponent<PartSlotIndex>().slotIndex == slot)
                {
                    m_weapon = weapon;
                    m_weapon.onFullyChargedStart += StartAnimationState;
                    m_weapon.onFullyChargedEnd += EndAnimationState;

                    return;
                }
            }
          
        }
    }
}
