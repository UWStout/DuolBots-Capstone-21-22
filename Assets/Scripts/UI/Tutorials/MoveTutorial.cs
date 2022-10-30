using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using DuolBots.Mirror;

namespace DuolBots.Tutorial
{
    public class MoveTutorial : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> m_Targets = null;

        [SerializeField]
        private GameObject m_rootPrefab = null;
        [SerializeField]
        private StringID m_partUniqueID = null;
        [SerializeField]
        private bool IsTutorial = false;

        private bool TutorialDone = false;
        public bool TutDone => TutorialDone;

        BotUnderConstruction m_underConstruction;

        private int m_CurIndex = -1;
        // Start is called before the first frame update
        void Start()
        {
            m_Targets[0].SetActive(true);
            m_CurIndex = 0;
            for (int i = 1; i < m_Targets.Count; i++)
            {
                m_Targets[i].SetActive(false);
            }
        }

        public int GetActiveIndex()
        {
            foreach (GameObject target in m_Targets)
            {
                if (target.activeSelf)
                    return m_Targets.IndexOf(target);
            }
            return -1;
        }

        public void JumpToTarget(int index)
        {
            if (index < 0 || index >= m_Targets.Count) { return; }
            m_Targets[m_CurIndex].SetActive(false);
            m_CurIndex = index;
            m_Targets[m_CurIndex].SetActive(true);

        }

        public void NextTarget()
        {
            m_Targets[m_CurIndex].SetActive(false);
            m_CurIndex++;
            if (m_CurIndex < m_Targets.Count)
            { 
                m_Targets[m_CurIndex].SetActive(true);
            }
        }
        private bool flag = false;
        public void EndTarget()
        {
            if (flag) { return; }
            flag = true;
            NextTarget();
            TutorialDone = true;
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
