// Original Authors - Wyatt Senalik
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An event that ties its callbacks to ids instead of references to the callbacks themselves.
/// For when you need to specify actions which are dependent on one another.
/// </summary>
//
// Example of necessariness
//   int subID = damagingCollider.GetUnusedID();
//   int amountCallbacks = 0;
//   // Declare the new callback in advance
//   Action newContextCallback = () =>
//   {
//     int amountHits = damagingCollider.GetAmountHitsSinceEnable();
//     if (amountHits <= ++amountCallbacks)
//     {
//       damagingCollider.UnsubscribeFromOnTriggerEnter(subID);
//       effectContext.InvokeCallback();
//     }
//   };
//   // Create a new context for the trigger event
//   SkillEffectContext newContext = new SkillEffectContext(additionalEffect, effectContext.GetSkill(),
//   effectContext.GetUserPawn(), effectContext.GetTargetPawns(), newContextCallback);
//   // Action to call when the trigger
//   Action onTriggerAction = () =>
//   {
//     PerformEffect(newContext);
//   };
//   // Subscribe to the collision
//   damagingCollider.SubscribeToOnTriggerEnter(subID, onTriggerAction);
public class IDEvent : IIDEvent
{
    // Dictionary containing the callbacks
    private Dictionary<int, Action> m_onTriggerEnterActions = new Dictionary<int, Action>();
    // Handling if callbacks try to change the dictionary
    private bool m_isExecutingActions = false;
    private List<int> m_actionsToUnsub = new List<int>();
    private List<Tuple<int, Action>> m_actionsToSub = new List<Tuple<int, Action>>();


    /************************************************************************************************************************/
    #region IIDEvent
    public void Invoke()
    {
        // Execute each action and mark it as executing so that no
        // actions may be added or removed during execution
        m_isExecutingActions = true;
        foreach (KeyValuePair<int, Action> pair in m_onTriggerEnterActions)
        {
            Action action = pair.Value;
            action?.Invoke();
        }
        m_isExecutingActions = false;
        // Sub actions from the invocations
        foreach (Tuple<int, Action> tuple in m_actionsToSub)
        {
            m_onTriggerEnterActions.Add(tuple.Item1, tuple.Item2);
        }
        m_actionsToSub.Clear();
        // Unsub actions from the invocations
        foreach (int id in m_actionsToUnsub)
        {
            m_onTriggerEnterActions.Remove(id);
        }
        m_actionsToUnsub.Clear();
    }

    /************************************************************************************************************************/
    #region IReadOnlyIDEvent
    public int GetUnusedID()
    {
        // Build a list of all the currently existing IDs
        List<int> existingIDList = new List<int>(m_onTriggerEnterActions.Keys);
        foreach (Tuple<int, Action> idToAdd in m_actionsToSub)
        {
            existingIDList.Add(idToAdd.Item1);
        }

        // If there are no existing IDs, any value is fine
        if (existingIDList.Count == 0)
        {
            return int.MinValue;
        }

        // Find the maximum value in the lsit
        int max = int.MinValue;
        int min = int.MaxValue;
        foreach (int existingID in existingIDList)
        {
            if (existingID > max)
            {
                max = existingID;
            }
            if (existingID < min)
            {
                min = existingID;
            }
        }
        // If its not max int, just add 1 and its unique
        if (max != int.MaxValue)
        {
            return max + 1;
        }
        // If it is max int, then check if the min is min value
        if (min != int.MinValue)
        {
            return min - 1;
        }

        // If somehow we have both min and max value as ids, subtract from the max
        // until we have a unique id
        int curID = max - 1;
        while (existingIDList.Contains(curID))
        {
            curID -= 1;
            if (curID == max)
            {
                Debug.LogError("Ran out of ids");
                return 0;
            }
        }
        return curID;
    }
    public void Subscribe(int id, Action triggerAction)
    {
        // If we are currently executing actions, we cannot add
        // the action right away and must wait
        if (m_isExecutingActions)
        {
            m_actionsToSub.Add(new Tuple<int, Action>(id, triggerAction));
        }
        // Add it if we are not currently executing actions
        else
        {
            m_onTriggerEnterActions.Add(id, triggerAction);
        }
    }
    public void Unsubscribe(int id)
    {
        // If we are currently executing actions, we cannot remove
        // the action right away and must wait
        if (m_isExecutingActions)
        {
            m_actionsToUnsub.Add(id);
        }
        // Remove it if we are not currently executing actions
        else
        {
            m_onTriggerEnterActions.Remove(id);
        }
    }
    #endregion IReadOnlyIDEvent
    /************************************************************************************************************************/

    #endregion IEventWithIDCallbacks
    /************************************************************************************************************************/
}
