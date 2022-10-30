using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Mirror;

using DuolBots.Mirror;

namespace DuolBots.Tutorial.Mirror
{
    public class CreateChassis : NetworkBehaviour
    {
        [SerializeField]
        private GameObject m_rootPrefab = null;
        [SerializeField]
        private StringID m_partUniqueID = null;
        [SerializeField]
        private bool IsTutorial = false;

        BotUnderConstruction m_underConstruction;

        // Start is called before the first frame update
        public override void OnStartServer()
        {
            base.OnStartServer();
            PartDatabase temp_partDatabase = PartDatabase.instance;
            GameObject chassisPart = temp_partDatabase.GetPartScriptableObject(m_partUniqueID).battleNetworkPrefab;
            m_underConstruction = new BotUnderConstruction(m_rootPrefab, transform.position, "Target");
            m_underConstruction.CreateChassis(chassisPart);
            m_underConstruction.currentBotRoot.GetComponent<ITeamIndex>().teamIndex = (byte)(transform.GetSiblingIndex() + 100);
            if (IsTutorial)
            {
                GetComponentInParent<PartTutorial>().AddToList(m_underConstruction.currentBotRoot);
            }

            NetworkServer.Spawn(m_underConstruction.currentBotRoot);
            m_underConstruction.currentBotRoot.GetComponent<NetworkChildManager>().Spawn(m_underConstruction.currentChassis);
        }


    }
}
