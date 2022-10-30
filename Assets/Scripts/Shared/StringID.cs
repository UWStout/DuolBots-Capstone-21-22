using UnityEngine;
// Original Author - Wyatt Senalik

[CreateAssetMenu(fileName = "new StringID", menuName = "ScriptableObjects/StringID")]
public class StringID : ScriptableObject
{
    public string value => m_id;
    [SerializeField] private string m_id = "";


    public override string ToString()
    {
        return m_id;
    }

    public void Initialize(string id)
    {
        m_id = id;
    }
}
