using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace DuolBots.Tutorial
{
    public class MoveTutorialObject : MonoBehaviour
    {
        [SerializeField]
        private GameObject nextPosition = null;
        public static event Action onCheckpointReached;

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<ITeamIndex>().gameObject.tag != "Robot") { return; }

            if (nextPosition != null)
            {
                nextPosition.SetActive(true);
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.GetComponentInParent<MoveTutorial>().EndTarget();
                gameObject.SetActive(false);
            }
            onCheckpointReached?.Invoke();
        }
    }
}
