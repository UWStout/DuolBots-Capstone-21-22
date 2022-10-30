using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Controls playing music on scene change.
    /// </summary>
    public class WwiseMusicManager :
        SingletonMonoBehaviourPersistant<WwiseMusicManager>
    {
        // Constants
        private const bool IS_DEBUGGING = false;

        // If we should play the music if entering a scene that was not specified.
        [SerializeField] private bool m_stopInUnregScenes = false;
        // Name of the stop music event.
        [SerializeField, Required, ShowIf(nameof(m_stopInUnregScenes))]
        private WwiseEventName m_stopAllMusicEventName = null;
        // Scene and music to play in it.
        [SerializeField] private WwiseMusicScene[] m_musicScenes =
            new WwiseMusicScene[0];
        // Quick mapping from scene build index to event name.
        private Dictionary<string, string> m_sceneToSoundDict =
            new Dictionary<string, string>();

        private string m_curPlayEventName = "";


        // Domestic Initialization
        protected override void Awake()
        {
            base.Awake();

            // Initialize the quick reference dictionary
            m_sceneToSoundDict = new Dictionary<string, string>(m_musicScenes.Length);
            foreach (WwiseMusicScene temp_mScene in m_musicScenes)
            {
                m_sceneToSoundDict.Add(temp_mScene.sceneNameToPlayIn,
                    temp_mScene.musicEventName);
            }
        }
        private void OnEnable()
        {
            SceneManager.activeSceneChanged += ChangeMusicOnSceneChange;
        }
        private void OnDisable()
        {
            SceneManager.activeSceneChanged -= ChangeMusicOnSceneChange;
        }


        public void PlayMusic(WwiseEventName temp_wiseMusicEventName)
        {
            PlayMusicHelper(temp_wiseMusicEventName.wwiseEventName);
        }


        /// <summary>
        /// Plays the music for the scene changed to.
        /// </summary>
        private void ChangeMusicOnSceneChange(Scene prevScene, Scene newScene)
        {
            // Try to get the event for the music to play for this scene.
            if (!m_sceneToSoundDict.TryGetValue(newScene.name,
                out string temp_musicEventName))
            {
                // If there is no event and we want to stop playing
                // the previous music.
                if (m_stopInUnregScenes)
                {
                    AkSoundEngine.PostEvent(m_stopAllMusicEventName.wwiseEventName,
                        gameObject);
                }
                return;
            }
            PlayMusicHelper(temp_musicEventName);
        }
        /// <summary>
        /// Plays the given music if its not already playing.
        /// </summary>
        /// <param name="musicEventName"></param>
        private void PlayMusicHelper(string musicEventName)
        {
            // Don't play the music on top of the other one.
            if (musicEventName == m_curPlayEventName) { return; }

            #region Logs
            CustomDebug.LogForComponent($"Playing {musicEventName}",
                this, IS_DEBUGGING);
            #endregion Logs

            // Start playing the new music.
            AkSoundEngine.PostEvent(musicEventName, gameObject);
            m_curPlayEventName = musicEventName;
        }


        /// <summary>
        /// Holds a scene and the music to be played in that scene.
        /// </summary>
        [Serializable]
        public class WwiseMusicScene
        {
            [SerializeField] private WwiseEventName m_musicEventName = null;
            [SerializeField, Scene] private string m_sceneToPlayIn = "";

            public string musicEventName => m_musicEventName.wwiseEventName;
            public string sceneNameToPlayIn => m_sceneToPlayIn;
        }
    }
}
