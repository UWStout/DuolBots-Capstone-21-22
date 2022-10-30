using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Original Author - Wyatt Senalik

namespace DuolBots.Mirror
{
    public class NetworkTransformDynamicChildsChild : NetworkTransformDynamicChildBase
    {
        // The transform that should be synced.
        public override Transform targetComponent => m_targetComponent;
        [SerializeField] private Transform m_targetComponent = null;
    }
}
