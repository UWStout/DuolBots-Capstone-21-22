using System;
using UnityEngine;

namespace DuolBots.Tutorial
{
    public class KillTarget : MonoBehaviour, IObjectDestroyer
    {


        private IRobotHealth m_RobotHealth;
        private GameObject tutorial;

        public event Action<GameObject> onShouldDestroyObject;

        private void Start()
        {
            m_RobotHealth = GetComponent<IRobotHealth>();
            m_RobotHealth.onHealthReachedCritical += destroyBot;
        }

        public void setTutorial(GameObject pt)
        {
            tutorial = pt;
        }


        private void destroyBot(IRobotHealth botToDie)
        {
            m_RobotHealth.onHealthReachedCritical -= destroyBot;
            if (tutorial != null)
            {
                tutorial.GetComponent<PartTutorial>().RemoveFromList(gameObject);
            }
            onShouldDestroyObject?.Invoke(gameObject);
        }

    }
}
