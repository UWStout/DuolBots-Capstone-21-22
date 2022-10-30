using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace DuolBots
{
    public class CurrentDuolPlayerInput : SingletonMonoBehaviour<CurrentDuolPlayerInput>
    {
        private MenuStack m_menuStack;
        private DuolPlayerMenuSetup m_currentMenu;

        public DuolPlayerMenuSetup currentMenu => m_currentMenu;

        [SerializeField] DuolPlayerMenuSetup m_startingMenu;

        protected override void Awake()
        {
            base.Awake();
            m_menuStack = gameObject.GetComponent<MenuStack>();
            m_currentMenu = m_startingMenu;
        }

        private void OnEnable()
        {
            m_menuStack.OnMenuChange += OnMenuChange;
        }

        private void OnMenuChange(GameObject obj)
        {
            m_currentMenu = obj.GetComponent<DuolPlayerMenuSetup>();
            Assert.IsNotNull(m_currentMenu, "Current Player is NULL");
            m_currentMenu.SetupMenu();
            
        }

        private void OnDisable()
        {
            if(m_menuStack != null)
                m_menuStack.OnMenuChange -= OnMenuChange;
        }
    }
}
