using System;
using UnityEngine;
// Original Author - Wyatt Senalik, Aaron Duffey

namespace DuolBots
{
    /// <summary>
    /// Destroys the specified game object on impact
    /// </summary>
    [RequireComponent(typeof(PartImpactCollider))]
    public class DestroySpecifiedObjectImpactHandler: MonoBehaviour, IImpactHandler,
        IObjectDestroyer
    {
        [SerializeField] private int m_priority = 1;
        // Object to destroy on impact
        [SerializeField] private GameObject m_objectToDestroy = null;

        public event Action<GameObject> onShouldDestroyObject;


        #region IImpactHandler
        public int priority => m_priority;

        public void HandleImpact(Collider collider, bool didImpactEnemy, byte enemyTeamIndex)
        {
            // Destroy the object on impact
            onShouldDestroyObject?.Invoke(m_objectToDestroy);
            //Destroy(m_objectToDestroy);
        }
        #endregion IImpactHandler
    }
}
