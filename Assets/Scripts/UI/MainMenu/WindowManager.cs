using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Original Authors - Cole Woulf using youtube video
namespace DuolBots
{

    /// <summary>
    /// This script gets and sets the resolutions as well as changes to text plus it also saves the current resolution.
    /// The Resolution Only Works in the Build though the text and stuff will change and stay work how it is supposed to.
    /// </summary>
    public class WindowManager : MonoBehaviour
    {
        // Const player prefs key
        private const string RESOLUTION_PREF_KEY = "resolution";
        private const string TOGGLE_FULLSCREEN_PREF_KEY = "fullscreenToggle";
        private const string BOOL_FIRSTTIME_PREF_KEY = "firstTimeBool";
        // the text that will change when moving through array 
        [SerializeField] private TextMeshProUGUI m_resolutionNumberText;

        // array of resolutions
        private Resolution[] m_resolutions;
        // the current index that the player is at in the Resolution array
        private int m_currentResolutionIndex = 0;

        //[SerializeField] private Toggle m_isFullScreenToggle;
        private bool m_IsFullScreen;
        [SerializeField] private TextMeshProUGUI m_IsFullscreenText = null;

        [SerializeField] private PlayingSounds m_playingSounds = null;

        /// <summary>
        /// Setting the player to 1920 by 1080 for their first time playing because that
        /// is the preferred resolution for our game otherwise load the players preferences
        /// previously saved
        /// </summary>
        private void Awake()
        {
            m_resolutions = Screen.resolutions;
            m_currentResolutionIndex = m_resolutions.Length - 1;
            m_currentResolutionIndex = PlayerPrefs.GetInt(RESOLUTION_PREF_KEY, 0);
            m_IsFullScreen = (PlayerPrefs.GetInt(TOGGLE_FULLSCREEN_PREF_KEY) == 1) ? true : false;
            if(m_IsFullScreen)
                m_IsFullscreenText.text = "On";
            else
                m_IsFullscreenText.text = "Off";
            SetResolutionText(m_resolutions[m_currentResolutionIndex]);
            ApplyChanges();
            /*
            if (PlayerPrefs.GetInt(BOOL_FIRSTTIME_PREF_KEY) == 0)
            {
                m_resolutions = Screen.resolutions;
                m_currentResolutionIndex = m_resolutions.Length - 1;
                m_currentResolutionIndex = PlayerPrefs.GetInt(RESOLUTION_PREF_KEY, 0);
                m_isFullScreenToggle.isOn = (PlayerPrefs.GetInt(TOGGLE_FULLSCREEN_PREF_KEY) == 1) ? true : false;
                SetResolutionText(m_resolutions[m_currentResolutionIndex]);
                ApplyChanges();
            }
            else
            {
                m_resolutionNumberText.text = "1920 x 1080";
                Screen.SetResolution(1920, 1080, true);
                PlayerPrefs.SetInt(TOGGLE_FULLSCREEN_PREF_KEY, 1);
                PlayerPrefs.SetInt(BOOL_FIRSTTIME_PREF_KEY, 0);
            }
            */
        }

        /// <summary>
        /// These set of functions help with cycling through the resolutions and setting the text
        /// </summary>
        #region Resolution Cycling
        private void SetResolutionText(Resolution resolution)
        {
            m_resolutionNumberText.text = resolution.width + "x" + resolution.height;
        }

        public void SetNextResolution()
        {
            m_currentResolutionIndex = GetNextWrappedIndex(m_resolutions, m_currentResolutionIndex);
            SetResolutionText(m_resolutions[m_currentResolutionIndex]);
        }

        public void SetPreviousResolution()
        {
            m_currentResolutionIndex = GetPreviousWrappedIndex(m_resolutions, m_currentResolutionIndex);
            SetResolutionText(m_resolutions[m_currentResolutionIndex]);
        }

        private int GetNextWrappedIndex<T>(IList<T> collection, int currentIndex)
        {
            if (collection.Count < 1) return 0;
            return (currentIndex + 1) % collection.Count;
        }

        private int GetPreviousWrappedIndex<T>(IList<T> collection, int currentIndex)
        {
            if (collection.Count < 1) return 0;
            if (currentIndex - 1 < 0) return collection.Count - 1;
            return (currentIndex - 1) % collection.Count;
        }
        #endregion

        /// <summary>
        /// Sets the current index in the resolutions array as well as calls ApplyCurrentResolution function
        /// </summary>
        /// <param name="newResIndex"></param>
        private void SetAndApplyResolution(int newResIndex)
        {
            m_currentResolutionIndex = newResIndex;
            ApplyCurrentResolution();
        }

        /// <summary>
        /// Applys the current resolution that is in the resolutions list using the current index
        /// </summary>
        private void ApplyCurrentResolution()
        {
            ApplyResolution(m_resolutions[m_currentResolutionIndex]);
        }

        /// <summary>
        /// Applying the resolution to the screen and the player prefs
        /// </summary>
        /// <param name="res"></param>
        private void ApplyResolution(Resolution res)
        {
            SetResolutionText(res);
            Screen.SetResolution(res.width, res.height, m_IsFullScreen);
            PlayerPrefs.SetInt(RESOLUTION_PREF_KEY, m_currentResolutionIndex);
        }

        /// <summary>
        /// Switching the toggle off and on
        /// </summary>
        public void SwitchScreen()
        {
            int currentFullscreenState; // = m_IsFullScreen == true ? 1 : 0;
            m_IsFullScreen = !m_IsFullScreen;

            if (m_IsFullScreen)
            {
                m_IsFullscreenText.text = "On";
                currentFullscreenState = 1;
            }
            else
            {
                m_IsFullscreenText.text = "Off";
                currentFullscreenState = 0;
            }

            PlayerPrefs.SetInt(TOGGLE_FULLSCREEN_PREF_KEY, currentFullscreenState);
            m_playingSounds.SelectSound();
        }

        /// <summary>
        /// Setting the toggle for controller support
        /// </summary>
        public void SetToggle()
        {
            /*
            m_isFullScreenToggle.isOn = !m_isFullScreenToggle.isOn;
            if (m_isFullScreenToggle.isOn)
                m_IsFullscreenText.text = "On";
            else
                m_IsFullscreenText.text = "Off";
            */
        }

        /// <summary>
        /// The public function that is called to apply everything needed for the resolutions
        /// </summary>
        public void ApplyChanges()
        {
            SetAndApplyResolution(m_currentResolutionIndex);
        }
    }
}
