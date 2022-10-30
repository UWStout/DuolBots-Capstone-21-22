using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTargetGameObject : MonoBehaviour
{
    [SerializeField]
    public GameObject m_Target = null;

    // Update is called once per frame
    void Update()
    {
        if (m_Target != null)
        {
            transform.position = new Vector3(m_Target.transform.position.x, m_Target.transform.position.y, m_Target.transform.position.z);
        }
    }

    public void SetTarget(GameObject Go)
    {
        m_Target = Go;
    }

    
}
