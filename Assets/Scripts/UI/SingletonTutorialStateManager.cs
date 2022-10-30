using UnityEngine;
using UnityEngine.SceneManagement;

using NaughtyAttributes;
// Original Authors - Eslis Vang

    public class SingletonTutorialStateManager : SingletonMonoBehaviourPersistant<SingletonTutorialStateManager>
    {
        public bool isTutorialActive => m_isTutorialActive;
        [SerializeField] private bool m_isTutorialActive = false;
        public int tutorialIndex => m_tutorialIndex;
        [SerializeField] [ShowIf("m_isTutorialActive")] private int m_tutorialIndex = 0;

        [SerializeField] [BoxGroup("Scenes")] [Scene] private string m_partSelect = "BetterPartSelection_SCENE";


        protected override void Awake()
        {
            // Call the singleton's awake.
            base.Awake();
        }


        public void EndTutorial()
        {
            m_isTutorialActive = false;
            m_tutorialIndex = -1;
        }
        public void NextTutorial()
        {
            m_tutorialIndex++;
            if (tutorialIndex > 1) { EndTutorial(); }
            SceneManager.LoadScene(m_partSelect);
        }
    }
