using System;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;
// Original Authors - Eslis Vang and Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Handles activating each pair of cameras in the build ui scene.
    /// </summary>
    public class BuildUICameraSystem : MonoBehaviour
    {
        [SerializeField] private PairCamera<CinemachineVirtualCamera>
            m_chassisMovementCameras = null;
        [SerializeField] private PairCamera<CinemachineVirtualCamera>
            m_partSelectCameras = null;

        public PairCamera<CinemachineVirtualCamera> chassisMovementCameras
            => m_chassisMovementCameras;
        public PairCamera<CinemachineVirtualCamera> partSelectCameras
            => m_partSelectCameras;


        public void ChangeToChassisMovementCamera()
        {
            TurnOffAllCameras();

            m_chassisMovementCameras.ToggleCameras(true);
        }
        public void ChangeToPartSelectCamera()
        {
            TurnOffAllCameras();

            m_partSelectCameras.ToggleCameras(true);
        }


        private void TurnOffAllCameras()
        {
            m_chassisMovementCameras.ToggleCameras(false);
            m_partSelectCameras.ToggleCameras(false);
        }
    }

    /// <summary>
    /// Groupiing of two <see cref="CinemachineVirtualCamera"/>s together
    /// for player zero and player one.
    /// </summary>
    [Serializable]
    public class PairCamera<T> where T : CinemachineVirtualCameraBase
    {
        [SerializeField] private T m_playerZeroCam = null;
        [SerializeField] private T m_playerOneCam = null;

        public T playerZeroCam => m_playerZeroCam;
        public T playerOneCam => m_playerOneCam;
        public IReadOnlyList<T> bothCameras => new T[2]
            { m_playerZeroCam, m_playerOneCam };


        public void ToggleCameras(bool cond)
        {
            m_playerZeroCam.gameObject.SetActive(cond);
            m_playerOneCam.gameObject.SetActive(cond);
        }
        public T GetPlayerCam(byte playerIndex)
        {
            if (playerIndex == 0) { return playerZeroCam; }
            if (playerIndex == 1) { return playerOneCam; }
            Debug.LogError($"Invalid {nameof(playerIndex)} for " +
                $"{GetType().Name}'s {nameof(GetPlayerCam)}");
            return null;
        }
    }
}
