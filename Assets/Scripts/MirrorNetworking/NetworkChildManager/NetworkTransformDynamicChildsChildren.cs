using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// DEPRECATED.
    /// 
    /// Similar to <see cref="NetworkTransformDynamicChildsChild"/> but multiple
    /// children can be specified as the components that want to be synced.
    /// Helps clean up
    /// Does not extend <see cref="NetworkTransformDynamicChildBase"/>. Instead
    /// creates <see cref="NetworkTransformDynamicChildsChildrenSingleChild"/>s
    /// which do extend <see cref="NetworkTransformDynamicChildBase"/>.
    /// </summary>
    public class NetworkTransformDynamicChildsChildren : NetworkChildBehaviour
    {
        [Header("Synchronization")]
        [SerializeField] private bool m_syncPosition = true;
        [SerializeField] private bool m_syncRotation = true;
        [SerializeField] private bool m_syncScale = false;
        [SerializeField] private bool m_syncParent = false;
        [Header("Sensitivity")]
        [SerializeField] private float m_positionSensitivity = 0.01f;
        [SerializeField] private float m_rotationSensitivity = 0.01f;
        [SerializeField] private float m_scaleSensitivity = 0.01f;

        [SerializeField] private Transform[] m_targetComponents = new Transform[2];


        // Called 0th
        // Called on both client and server
        protected override void Awake()
        {
            base.Awake();

            // Create a single child for each specified target component.
            foreach (Transform temp_singleTrans in m_targetComponents)
            {
                NetworkTransformDynamicChildsChildrenSingleChild.Create(gameObject,
                    temp_singleTrans, m_syncPosition, m_syncRotation, m_syncScale,
                    m_syncParent, m_positionSensitivity, m_rotationSensitivity,
                    m_scaleSensitivity);
            }
        }
    }
}
