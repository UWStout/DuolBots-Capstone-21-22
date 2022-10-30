using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DuolBots;
public class RespawnBots : MonoBehaviour
{
    [SerializeField]
    private float m_respawnTime = 2f;
    [SerializeField]
    private List<GameObject> m_spawnPoints = null;
    private List<bool> m_isRespawning = null;

    // make list so if object is respawning it doesnt call multiple times
    private void Start()
    {
        m_isRespawning = new List<bool>();
        foreach (GameObject sp in m_spawnPoints)
        {
            m_isRespawning.Add(false);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        //check if the collision is with Robot
        GameObject go = other.gameObject.GetComponentInParent<Rigidbody>().gameObject;
        if (go.tag == "Robot")
        {
            //find the corosponding SpawnPoint
            int i = m_spawnPoints.FindIndex(X => X.GetComponent<TeamIndex>().teamIndex == go.GetComponent<TeamIndex>().teamIndex);
            // initiate respawn
            if (m_isRespawning[i] == false)
            {
                m_isRespawning[i] = true;
                StartCoroutine(Respawn(go, i));
            }
        }
    }

    private IEnumerator Respawn(GameObject GO, int i)
    {
        // wait 2 seconds then respawn
        yield return new WaitForSeconds(m_respawnTime);
        GO.transform.position = m_spawnPoints[i].transform.position;
        GO.transform.SetPositionAndRotation(m_spawnPoints.Find(X => X.GetComponent<TeamIndex>().teamIndex == GO.GetComponent<TeamIndex>().teamIndex).transform.position, new Quaternion());
        m_isRespawning[i] = false;
    }
}
