using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;
// Original Authors - Shelby Vian
// Tweaked by Wyatt Senalik

namespace DuolBots
{
    public class Shared_GetPartImagesForDisplay : MonoBehaviour
    {
        [SerializeField] [Required] private InstantiateLayoutPrefabs m_layoutPrefabs = null;


        /// <summary>
        /// Called from start on the local variant.
        /// </summary>
        /// <param name="teamIndex">teamIndex to get the data for the team
        /// and the bot root.</param>
        public void InitializePartImagesForDisplay(byte teamIndex)
        {
            int temp_numParts = GetPartsInScene(teamIndex,
                out string[] temp_partStringIDs, out byte[] temp_partSlotIndices);
            PartDatabase temp_partDatabase = PartDatabase.instance;

            IReadOnlyList<SetImageTextures> temp_imageSpots =
                InstantiateAndGetSpots(temp_numParts);

            int temp_index = 0;

            foreach (string part in temp_partStringIDs)
            {
                int temp_firstIndex = temp_index;
                int temp_secondIndex = temp_index + (temp_imageSpots.Count / 2);
                #region Asserts
                CustomDebug.AssertIndexIsInRange(temp_firstIndex, temp_imageSpots,
                    this);
                CustomDebug.AssertIndexIsInRange(temp_secondIndex, temp_imageSpots,
                    this);
                CustomDebug.AssertIndexIsInRange(temp_index, temp_partSlotIndices,
                    this);
                #endregion Asserts
                byte temp_partSlotIndex = temp_partSlotIndices[temp_index];

                PartScriptableObject temp_partSO = temp_partDatabase.
                    GetPartScriptableObject(part);
                PartUIData temp_partUIData = temp_partSO.partUIData;
                Texture2D temp_unlockedTex = temp_partSO.partUIData.unlockedSprite;

                temp_imageSpots[temp_firstIndex].SetTexture(temp_unlockedTex,
                    temp_partSlotIndex);
                temp_imageSpots[temp_secondIndex].SetTexture(temp_unlockedTex,
                    temp_partSlotIndex);

                temp_index++;
            }
        }
        public void ClearPartImages()
        {
            m_layoutPrefabs.ClearLayout();
        }

        /// <summary>
        /// Get the current parts from BuildSceneBotData and create a list of stringIDs
        /// </summary>
        private int GetPartsInScene(byte teamIndex, out string[] stringIDs,
            out byte[] slotIndices)
        {
            BuiltBotData temp_botData = BuildSceneBotData.GetBotData(teamIndex);

            int temp_partAm = temp_botData.slottedPartIDList.Count;
            stringIDs = new string[temp_partAm];
            slotIndices = new byte[temp_partAm];
            for (int i = 0; i < temp_partAm; ++i)
            {
                PartInSlot temp_stringID = temp_botData.slottedPartIDList[i];
                stringIDs[i] = temp_stringID.partID;
                slotIndices[i] = temp_stringID.slotIndex;
            }

            return temp_partAm;
        }

        /// <summary>
        /// Calls to InstantiateLayoutPrefab to instantiate part image spots and control icons spots
        /// Also calls to FixedPartCameras to instantate camera objects and sat their viewport location
        /// </summary>
        private IReadOnlyList<SetImageTextures> InstantiateAndGetSpots(int numParts)
        {
            m_layoutPrefabs.SetPartsAndLayout(numParts);
            m_layoutPrefabs.GetImageAndCameraSpots(
                out List<SetImageTextures> m_imageSpots);
            return m_imageSpots;
        }

    }//end class

}
