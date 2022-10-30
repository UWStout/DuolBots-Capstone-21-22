using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DuolBots
{
    public class SelectedIsLarger : MonoBehaviour
    {
        [SerializeField] private GameObject m_playButton = null;
        [SerializeField] private GameObject m_settingsButton = null;
        [SerializeField] private GameObject m_quitButton= null;
        [SerializeField] private EventSystem m_eventSystem = null;

        private void Update()
        {
            // Compare selected gameObject with referenced Button gameObject
            if (m_eventSystem.currentSelectedGameObject == m_playButton)
            {
                m_playButton.GetComponent<RectTransform>().sizeDelta = new Vector2(902.19f, 62);
                m_playButton.GetComponent<RectTransform>().localPosition = new Vector3(-265, -75, 0);
            }
            else
            {
                m_playButton.GetComponent<RectTransform>().sizeDelta = new Vector2(840f, 62);
                m_playButton.GetComponent<RectTransform>().localPosition = new Vector3(-297, -75, 0);
            }

            // Compare selected gameObject with referenced Button gameObject
            if (m_eventSystem.currentSelectedGameObject == m_settingsButton)
            {
                m_settingsButton.GetComponent<RectTransform>().sizeDelta = new Vector2(902.19f, 62);
                m_settingsButton.GetComponent<RectTransform>().localPosition = new Vector3(-265, -165, 0);
            }
            else
            {
                m_settingsButton.GetComponent<RectTransform>().sizeDelta = new Vector2(840f, 62);
                m_settingsButton.GetComponent<RectTransform>().localPosition = new Vector3(-297, -165, 0);
            }

            // Compare selected gameObject with referenced Button gameObject
            if (m_eventSystem.currentSelectedGameObject == m_quitButton)
            {
                m_quitButton.GetComponent<RectTransform>().sizeDelta = new Vector2(902.19f, 62);
                m_quitButton.GetComponent<RectTransform>().localPosition = new Vector3(-265, -345, 0);
            }
            else
            {
                m_quitButton.GetComponent<RectTransform>().sizeDelta = new Vector2(840f, 62);
                m_quitButton.GetComponent<RectTransform>().localPosition = new Vector3(-297, -345, 0);
            }
        }
    }
}
