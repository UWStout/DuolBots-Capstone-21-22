using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// Original Authors - Shelby
// Small Edits by Wyatt

namespace DuolBots
{
    public class FixedPartCameras : MonoBehaviour
    {
        [SerializeField] private GameObject m_fixedCameraPrefab, botroot;
        [SerializeField] private CameraPostionsForParts positions;
        [SerializeField] private List<RenderTexture> m_renderList = new List<RenderTexture>();

        private List<GameObject> cameras = new List<GameObject>(), root0Parts = new List<GameObject>(), camerasSpotsList = new List<GameObject>();

        private GameObject[] parts;
        public int numOfCameras = -1;
        private byte layoutType;

     /*   private void Update()//do this differently please
        {
            if (cameras.Count < numOfCameras && numOfCameras != -1)
            {
                parts = GameObject.FindGameObjectsWithTag("Part");
                GameObject [] temp = GameObject.FindGameObjectsWithTag("Robot");
                foreach(GameObject bot in temp)
                {
                    if(bot.name == "Bot Root" && bot.GetComponent<TeamIndex>().teamIndex == 0)
                    {
                        botroot = bot;
                    }
                }


                if (layoutType == 0)
                { //horiz
                    SetCameraLayoutPositions();
                    SetCameraLayoutPositions();

                    InstantiateCameraObjects();
                }
                else if (layoutType == 1)
                {
                    // rectValues = vertRectValues;
                    SetCameraLayoutPositions();
                    InstantiateCameraObjects();
                }

            }

        }*/

        /// <summary>
        /// Called from Shared_GetPartImagesForDisplay InitializePartImagesForDisplay()
        /// Sets the number of cameras to create
        /// </summary>
        /// <param name="num">number of parts on bot (minus chassis and wheels)</param>
        /// <param name="layout">what axis the cameras are on: 0-vertical 1-horizontal 2-off</param>
        public void SetNumOfCameras(int num, byte layout)
        {
            numOfCameras = num;
            layoutType = layout;

        }

        /// <summary>
        /// Sets the camera viewports to the correct positions
        /// </summary>
        private void SetCameraLayoutPositions()
        {
            GetTeamParts();

            for (int i = 0; i < numOfCameras; i++)
            {
                var temp = Instantiate(m_fixedCameraPrefab, m_fixedCameraPrefab.transform.position, m_fixedCameraPrefab.transform.rotation);
                temp.transform.SetParent(root0Parts[i + 2].transform, false);
                cameras.Add(temp);

            }

            foreach (GameObject camera in cameras)
            {
                int temp_index = 0;

                if (cameras.IndexOf(camera) >= numOfCameras)
                    temp_index = (cameras.IndexOf(camera) - numOfCameras);
                else
                    temp_index = cameras.IndexOf(camera);

                if (m_renderList.Count > 0)
                    camera.GetComponent<Camera>().targetTexture = m_renderList[temp_index];
            }
        }

        public void SetRendersList(List<RenderTexture> renders)
        {
            m_renderList = renders;
        }

        /// <summary>
        /// Intantiates camera objects and sets potition relative to the part its on
        /// </summary>
        private void InstantiateCameraObjects()
        {
            int temp_index = 2;
            foreach (GameObject camera in cameras)
            {
                //if on horizontal layout, cameras have to be instantiated twice
                if (temp_index >= root0Parts.Count)
                {
                    temp_index = 2;
                }

                string temp_part = root0Parts[temp_index].GetComponent<PartSOReference>().partScriptableObject.partID.ToString();
                Debug.Log("Key: " + temp_part);
                // Vector3 test = positions.cameraTransformRotation[newKey];
                camera.transform.localRotation = Quaternion.Euler(positions.cameraTransformRotation[temp_part]);
                camera.transform.localPosition = positions.cameraTransformPosition[temp_part];
                camera.GetComponent<Camera>().cullingMask = (1 << (temp_index + 15));
                temp_index++;
            }
        }

        /// <summary>
        /// Sort through all parts in scene and add parts for the correct team index to list
        /// </summary>
        private void GetTeamParts()
        {
            foreach (GameObject botPart in parts)
            {
                if (botPart.transform.root == botroot.transform && !root0Parts.Contains(botPart))
                {
                    root0Parts.Add(botPart);

                    if (root0Parts.Count >= 2 && numOfCameras > 0)
                        SetLayerRecursively(botPart, root0Parts.IndexOf(botPart) + 15);
                }
            }
        }

        private void SetLayerRecursively(GameObject obj, int newLayer)
        {
            obj.layer = newLayer;

            foreach (Transform child in obj.transform)
            {
                if (null == child)
                {
                    continue;
                }

                SetLayerRecursively(child.gameObject, newLayer);
            }
        }


    }
}
