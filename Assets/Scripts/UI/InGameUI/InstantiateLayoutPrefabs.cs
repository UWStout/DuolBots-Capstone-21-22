using System;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
// Original Authors - Shelby Vian
// Tweaked by Wyatt Senalik

namespace DuolBots
{
    public class InstantiateLayoutPrefabs :
        SingletonMonoBehaviour<InstantiateLayoutPrefabs>
    {
        [SerializeField] private GameObject m_partImagePrefab;
        [SerializeField] private RectTransform m_LayoutPanelHorizLeft, m_LayoutPanelHorizRight;

        private List<SetImageTextures> m_imageSpots = new List<SetImageTextures>();

        private int m_numOfParts = 0;

        private List<GameObject> m_spawnedPartImgUIElements = new List<GameObject>();
        private CatchupEvent<IReadOnlyList<GameObject>> m_onLayoutInstantiated
            = new CatchupEvent<IReadOnlyList<GameObject>>();

        public IEventPrimer<IReadOnlyList<GameObject>> onLayoutInstantiated
            => m_onLayoutInstantiated;


        // Foreign Initialization
        private void Start()
        {
            CatchupEventResetter temp_eventResetter = CatchupEventResetter.instance;
            #region Asserts
            CustomDebug.AssertSingletonMonoBehaviourIsNotNull(temp_eventResetter,
                this);
            #endregion Asserts
            temp_eventResetter.AddCatchupEventForReset(m_onLayoutInstantiated);
        }


        /// <summary>
        /// Called from Shared_GetPartImagesForDisplay InitializePartImagesForDisplay()
        /// Gets layout information and calls InstantiateLayout, passing the chosen layout objects
        /// </summary>
        /// <param name="num">number of parts on the bot(minus chassis and wheels)</param>
        public void SetPartsAndLayout(int num)
        {
            m_numOfParts = num;

            m_spawnedPartImgUIElements = new List<GameObject>();

            m_imageSpots.Clear();
            GameObject[] temp_leftObjs = InstantiateLayout(m_LayoutPanelHorizLeft);
            GameObject[] temp_rightObjs = InstantiateLayout(m_LayoutPanelHorizRight);

            m_spawnedPartImgUIElements.AddRange(temp_leftObjs);
            m_spawnedPartImgUIElements.AddRange(temp_rightObjs);

            m_onLayoutInstantiated.Invoke(m_spawnedPartImgUIElements);
        }
        /// <summary>
        /// Returns m_imageSpots List and m_cameraSpots List
        /// </summary>
        public void GetImageAndCameraSpots(out List<SetImageTextures> images)
        {
            images = m_imageSpots;
        }
        /// <summary>
        /// Destroys all the spawned image elements.
        /// </summary>
        public void ClearLayout()
        {
            foreach (GameObject temp_spawnedElem in m_spawnedPartImgUIElements)
            {
                Destroy(temp_spawnedElem);
            }
            m_spawnedPartImgUIElements.Clear();
            m_imageSpots.Clear();
        }


        /// <summary>
        /// Called from SetPartsAndLayout()
        /// Generates an image prefab and sets its position on the ui
        /// </summary>
        /// <param name="parentPanelPrefab">Panel for the given axis</param>
        private GameObject[] InstantiateLayout(RectTransform parentPanelPrefab)
        {
            GameObject[] temp_spawnedObjs = new GameObject[m_numOfParts];
            for (int i = 0; i < m_numOfParts; i++)
            {
                GameObject temp_spawedPartImg = Instantiate(m_partImagePrefab,
                    m_partImagePrefab.transform.position,
                    m_partImagePrefab.transform.rotation);
                temp_spawedPartImg.transform.SetParent(parentPanelPrefab, false);
                SetImageTextures addedSlot = temp_spawedPartImg.GetComponentInChildren<SetImageTextures>();
                m_imageSpots.Add(addedSlot);

                temp_spawnedObjs[i] = temp_spawedPartImg;
            }

            return temp_spawnedObjs;
        }
    }
}
