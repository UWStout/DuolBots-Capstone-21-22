using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    public class ToggleActiveOnBetterBuildStateChange : MonoBehaviour
    {
        public enum eListType { WhiteList, BlackList }

        [SerializeField] private eListType m_listType = eListType.WhiteList;
        [SerializeField] private eBetterBuildSceneState[] m_activeStates =
            new eBetterBuildSceneState[0];

        // TO-DO Make it customizable with serializefield enum
        private BetterBuildSceneStateChangeHandler m_stateHandler = null;


        private void Start()
        {
            Action temp_enterState = null;
            Action temp_exitState = null;
            switch (m_listType)
            {
                case eListType.WhiteList:
                    temp_enterState = ToggleActive;
                    temp_exitState = ToggleInactive;
                    break;
                case eListType.BlackList:
                    temp_enterState = ToggleInactive;
                    temp_exitState = ToggleActive;
                    break;
                default:
                    CustomDebug.UnhandledEnum(m_listType, this);
                    break;
            }

            m_stateHandler = BetterBuildSceneStateChangeHandler.CreateNew(
                temp_enterState, temp_exitState, m_activeStates);
        }
        private void OnDestroy()
        {
            m_stateHandler.ToggleActive(false);
        }


        private void ToggleActive()
        {
            gameObject.SetActive(true);
        }
        private void ToggleInactive()
        {
            gameObject.SetActive(false);
        }
    }
}
