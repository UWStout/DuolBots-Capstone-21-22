using System;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik
// Hey! You found this! Congrats! You're probably the one doing some of the
// menuing tasks we have in the powerpoint that said to use this script.
// The idea of this script is to make using back buttons easier.
// To make a back button function, simply call CloseCurrentMenu.
// To open a submenu, call OpenMenu.
// To swap between two submenus (that are suppose to be the same level), call
// ReplaceCurrentMenu.
// Have the starting menu be the root menu that has no back butotn.


namespace DuolBots
{
    /// <summary>
    /// Manages a stack of the menus accessed so that the menu on top of the stack
    /// is always the active menu. Allows back button to simply call the
    /// CloseCurrentMenu function to return to the last menu.
    /// </summary>
    public class MenuStack : MonoBehaviour
    {
        // Reference to the gameobject of the first/root menu
        [Tooltip("The game object of the menu that starts the game visible")]
        [SerializeField]
        private GameObject m_startingMainMenu = null;

        // Stack of the menus. Will never remove the root menu from itself
        private Stack<GameObject> m_menuStack = new Stack<GameObject>();


        /// <summary>Event called when a new menu is set active.
        /// Parameters: GameObject - Menu that was set active.</summary>
        public event Action<GameObject> OnMenuChange;

        /// <summary>Event called when the root menu is set active.</summary>
        public event Action OnRootMenuActivate;
        /************************************************************************************************************************/

        /// <summary>
        /// Called 0th [Domestic Initialization]
        /// </summary>
        private void Awake()
        {
            if (m_startingMainMenu == null)
            {
                Debug.Log($"<color=red> Starting Menu not assigned for MenuStack</color> => {this.gameObject.name}");
            }

            // Push the root menu on as the only menu in the stack
            ResetToDefaultMenu();
        }

        /************************************************************************************************************************/
        /// <summary>
        /// Pushes the given menu onto the stack and makes it the active menu.
        /// </summary>
        /// <param name="menuObj">Menu to make active.</param>
        public void OpenMenu(GameObject menuObj)
        {
            // Hide the previous menu
            GameObject prevMenuObj = m_menuStack.Peek();
            prevMenuObj.SetActive(false);

            // Show the given menu
            m_menuStack.Push(menuObj);
            ActivateMenu(menuObj);
        }

        /// <summary>
        /// Pops the current open menu off the stack and actives the new top.
        /// Does nothing if the only menu in the stack is the root.
        /// </summary>
        public void CloseCurrentMenu()
        {
            // If there is no previous menu don't close the current menu
            // Don't let the stack remove the root
            if (m_menuStack.Count <= 1)
            {
#if UNITY_EDITOR
                //Debug.Log($"Could not close menu. There is no previous menu... ");
#endif
                return;
            }

            // Remove the what was the current menu and hide it
            GameObject prevMenuObj = m_menuStack.Pop();
            prevMenuObj.SetActive(false);

            // See what the last menu was and make it the new active menu
            GameObject curMenuObj = m_menuStack.Peek();
            ActivateMenu(curMenuObj);
        }

        /// <summary>
        /// Replaces the current menu with the given menu.
        /// </summary>
        /// <param name="menuObj">Menu to make active.</param>
        public void ReplaceCurrentMenu(GameObject menuObj)
        {
            // Remove the what was the current menu and hide it
            GameObject prevMenuObj = m_menuStack.Pop();
            prevMenuObj.SetActive(false);

            // Show the given menu
            m_menuStack.Push(menuObj);
            ActivateMenu(menuObj);
        }

        public void ResetToDefaultMenu()
        {
            //First time
            if (m_menuStack.Count == 0)
            {
                m_menuStack.Push(m_startingMainMenu);
                ActivateMenu(m_startingMainMenu);
                return;
            }
            //reset to default menu
            while (m_menuStack.Count > 1)
            {
                CloseCurrentMenu();
            }

        }
        /************************************************************************************************************************/

        /// <summary>
        /// Helper function that activates the given GameObject and
        /// calls the OnMenuChange event with the menu's GameObject as a parameter.
        /// </summary>
        /// <param name="menuToActive">Menu's GameObject to activate.</param>
        private void ActivateMenu(GameObject menuToActive)
        {
            menuToActive.SetActive(true);
            OnMenuChange?.Invoke(menuToActive);
            if (menuToActive == m_startingMainMenu)
            {
                OnRootMenuActivate?.Invoke();
            }
        }
    }
}
