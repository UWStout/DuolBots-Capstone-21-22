using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DuolBots.Tutorial
{
    public class PartTutorial : MonoBehaviour
    {
        public static event Action<int> onTargetDestroyed;
        [SerializeField]
        private List<GameObject> m_Chassis = new List<GameObject>();

        public int GetChassisCount()
        {
            return m_Chassis.Count;
        }

        public void AddToList(GameObject go)
        {
            m_Chassis.Add(go);
            go.GetComponent<KillTarget>().setTutorial(gameObject);
        }

        public void RemoveFromList(GameObject go)
        {
            m_Chassis.Remove(go);
            if (m_Chassis.Count == 0)
            {
                GetComponentInParent<ReturnFromTutorials>().ReturnFromTutorial();
            }
        }
    }
}
