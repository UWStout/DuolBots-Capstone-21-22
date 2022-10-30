using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DuolBots
{
    public class GodHandTesting : MonoBehaviour
    {
        public float seconds = 5;
        public GameObject bot;

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(RunGodHand());
        }


        private IEnumerator RunGodHand()
        {
            while (bot == null) { yield return new WaitForEndOfFrame(); }
            yield return new WaitForSeconds(seconds);
            Debug.Log("run");
            GodHandSingleton.Instance.SpawnPlayGodHand(bot, bot.GetComponent<ITeamIndex>().teamIndex);
        }
    }
}
