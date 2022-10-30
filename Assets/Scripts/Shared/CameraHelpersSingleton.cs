using UnityEngine;
using UnityEngine.Assertions;

using NaughtyAttributes;
// Original Authors - Eslis Vang and Wyatt Senalik

namespace DuolBots
{
    public class CameraHelpersSingleton :
        DynamicSingletonMonoBehaviourPersistant<CameraHelpersSingleton>
    {
        [SerializeField] [Tag] private string m_playerCameraTag = "PlayerCamera";


        public GameObject FindMyCameraObj(byte playerIndex)
        {
            GameObject[] temp_camObjList = GameObject.
                FindGameObjectsWithTag(m_playerCameraTag);
            Assert.AreEqual(2, temp_camObjList.Length, $"Expected to find 2 game " +
                $"objects with the tag {m_playerCameraTag}. Instead found " +
                $"{temp_camObjList.Length}.");
            foreach (GameObject temp_singleCamObj in temp_camObjList)
            {
                PlayerIndex temp_playerIndex = temp_singleCamObj.
                    GetComponent<PlayerIndex>();
                Assert.IsNotNull(temp_playerIndex, $"{name}'s {GetType().Name} " +
                    $"expected {temp_singleCamObj.name} to have " +
                    $"{nameof(PlayerIndex)} attached but none was found.");

                if (temp_playerIndex.playerIndex == playerIndex)
                {
                    return temp_singleCamObj;
                }
            }

            Debug.LogError($"There was no camera with the tag " +
                $"{m_playerCameraTag} and the player index {playerIndex}");
            return null;
        }
        public Camera FindMyCamera(byte playerIndex)
        {
            GameObject temp_myCamObj = FindMyCameraObj(playerIndex);
            Camera temp_myCam = temp_myCamObj.GetComponent<Camera>();
            Assert.IsNotNull(temp_myCam, $"{name}'s {GetType().Name} " +
                    $"expected {temp_myCamObj.name} to have " +
                    $"{nameof(Camera)} attached but none was found.");
            return temp_myCam;
        }
    }
}
