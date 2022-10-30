using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using TMPro;
// Original Authors - ?

namespace DuolBots.Tutorial
{
    public class BattleTutorialManager : MonoBehaviour
    {
        [SerializeField] private MoveTutorial m_move;
        [SerializeField] private PartTutorial m_part;
        private TutorialDialogueLists m_dialogueList;
        [SerializeField] private GameObject m_textPanel;
        private DialogueIndex m_dialogueIndex;
        [SerializeField] private ReturnFromTutorials m_returnFromTuts;
        //private TutorialAnimationsManager m_setAnims;
        //[SerializeField] private GameObject m_partIcons;

        [SerializeField] private SingletonTutorialStateManager m_tutorialManager = null;
        [SerializeField] private int m_tutorialStage = -1;
        private List<string> textList = new List<string>();

        private bool m_isNextTutorialCoroutActive = false;


        void Awake()
        {
            m_dialogueList = GetComponent<TutorialDialogueLists>();
            m_dialogueIndex = GetComponent<DialogueIndex>();
            //m_setAnims = GetComponent<TutorialAnimationsManager>();

            ActiveTextPromptTimer.onActiveNextButton += ValidateNextDialogue;
            ActiveTextPromptTimer.onActiveBackButton += BackText;
        }

        IEnumerator Start()
        {
            m_dialogueIndex.dialogueIndex = -1;
            m_tutorialStage = 0;
            Assert.IsNotNull(m_tutorialManager, $"Tutorial Singleton is missing or null.");
            Assert.IsFalse(m_tutorialStage != 0 && m_tutorialStage != 1, $"Tutorial stage indexes are not valid: {m_tutorialStage}");

            //m_partIcons.SetActive(false);
            textList = m_dialogueList.stage1Dialogue;
            //m_part.gameObject.SetActive(false);

            ValidateNextDialogue();

            MoveTutorialObject.onCheckpointReached += CheckCheckpointIndex;
            
            Debug.Log(m_dialogueList.stage1Dialogue[m_dialogueIndex.dialogueIndex]);
            yield return new WaitForSeconds(1.1f);
            //Time.timeScale = 0;
            //m_setAnims.SetStageInt(m_tutorialStage);
        }
        private void OnDestroy()
        {
            ActiveTextPromptTimer.onActiveNextButton -= ValidateNextDialogue;
            ActiveTextPromptTimer.onActiveBackButton -= BackText;
        }


        public IEnumerator NextTutorial()
        {
            m_isNextTutorialCoroutActive = true;

            while (m_move.TutDone == false)
            {
                yield return new WaitForEndOfFrame();
            }

            m_dialogueIndex.dialogueIndex = -1;
            m_tutorialStage = 1;
            textList = m_dialogueList.stage2Dialogue;
            m_textPanel.gameObject.SetActive(true);
            ValidateNextDialogue();

            yield return new WaitForSeconds(1.1f);
            //Time.timeScale = 0;
            //m_setAnims.SetStageInt(m_tutorialStage);

            m_isNextTutorialCoroutActive = false;
        }

        void ValidateNextDialogue()
        {
            if (m_textPanel.activeSelf)
            {
                NextDialogue(m_dialogueIndex.dialogueIndex);
            }
        }

        void BackText()
        {
            if (m_dialogueIndex.dialogueIndex != 0)
            {
                m_dialogueIndex.dialogueIndex--;

                m_textPanel.GetComponentInChildren<TextMeshProUGUI>().text = textList[m_dialogueIndex.dialogueIndex];
                //m_setAnims.ActivateAnims(m_dialogueIndex.dialogueIndex);

            }
        }

        void NextDialogue(int curIndex)
        {
            curIndex++;

            if (curIndex < textList.Count)
            {
                m_textPanel.GetComponentInChildren<TextMeshProUGUI>().text = textList[curIndex];
                IncrementIndex();
                //m_setAnims.ActivateAnims(curIndex);
            }

            if (curIndex + 1 >= textList.Count)
            {
                //m_textPanel.gameObject.SetActive(false);
                if (m_tutorialStage < 1)
                {
                    if (m_isNextTutorialCoroutActive) { return; }
                    StartCoroutine(NextTutorial());
                }
            }
        }

        void IncrementIndex()
        {
            m_dialogueIndex.dialogueIndex++;
        }

        void CheckCheckpointIndex()
        {
            return;
            if (m_move.GetActiveIndex() == 1 || m_move.GetActiveIndex() == -1)
                ToggleTextPrompt(true);
        }

        void CheckTargetIndex(int index)
        {
            return;
            if (index == 1 || index == 0)
                ToggleTextPrompt(true);
        }

        void ToggleTextPrompt(bool state)
        {
            if (!state)
            {
                m_textPanel.SetActive(false);
                //   Time.timeScale = 1;
            }
            else
            {
                m_textPanel.SetActive(true);
                //   Time.timeScale = 0;
            }

        }

    }
}
