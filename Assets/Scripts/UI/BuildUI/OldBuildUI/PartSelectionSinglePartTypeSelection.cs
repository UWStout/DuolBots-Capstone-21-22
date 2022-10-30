using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using DuolBots;
using static UnityEngine.InputSystem.InputAction;
using NaughtyAttributes;
// Original Author(s) - Eslis Vang

public class PartSelectionSinglePartTypeSelection : MonoBehaviour
{
    private const bool IS_DEBUGGING = true;

    private LinkedList<PartScriptableObject> m_row = new LinkedList<PartScriptableObject>();
    [SerializeField] private ePartType m_ePartTypeToFind = ePartType.Chassis;
    [SerializeField] [ReadOnly] PartScriptableObject[] m_rowElements;

    // Start is called before the first frame update
    void Start()
    {
        if (PartDatabase.instance != null)
        {
            List<PartScriptableObject> temp_objectList = PartDatabase.instance.GetAllPartScriptableObjects();
            foreach (PartScriptableObject part in temp_objectList)
            {
                if (part.partType == m_ePartTypeToFind)
                {
                    if (m_row.Count == 0)
                    {
                        m_row.AddFirst(part);
                    }
                    else
                    {
                        m_row.AddLast(part);
                    }
                }
            }
            m_rowElements = new PartScriptableObject[m_row.Count];
            for (int i = 0; i < m_row.Count; i++)
            {
                m_rowElements[i] = GetNodeAtIndex(i);
            }
        }
        else
        {
            CustomDebug.Log($"PartDatabase singleton is null.", IS_DEBUGGING);
        }

        Vector2 temp_cellSize = GetComponent<GridLayoutGroup>().cellSize;

        foreach (Transform child in this.transform)
        {
            foreach (Transform childsChildren in child)
            {
                childsChildren.GetComponent<RectTransform>().sizeDelta = new Vector2(temp_cellSize.x, temp_cellSize.y);
            }
        }
    }

    public PartScriptableObject GetNodeAtIndex(int index)
    {
        LinkedListNode<PartScriptableObject> temp_node = m_row.First;

        for (int i = 0; i < index; i++)
        {
            if (temp_node.Next != null)
            {
                temp_node = temp_node.Next;
            }
            else
            {
                CustomDebug.Log($"ChassisSelection script linked list was null when trying to get value at index {i}.", IS_DEBUGGING);
                return null;
            }
        }

        return temp_node.Value;
    }
}
