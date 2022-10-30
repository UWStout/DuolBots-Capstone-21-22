using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik and Ben Lussman

namespace DuolBots
{
    /// <summary>
    /// Holds the base information about a part.
    /// </summary>
    [CreateAssetMenu(fileName = "new Part", menuName = "ScriptableObjects/Part")]
    public class PartScriptableObject : ScriptableObject
    {
        [SerializeField] private string m_partName = "";
        // If the part is chassis, movement, weapon, or utility
        [SerializeField] private ePartType m_partType = ePartType.Chassis;
        // ID may be changed to be a int/short/byte to save space later, but for readability
        // now we have them as strings
        [SerializeField] private StringID m_partID = null;
        [SerializeField] [Min(0)] private int m_weight = 0;
        [SerializeField] [Min(0.0f)] private float m_health = 0.0f;
        [SerializeField] [Range(0.0f, 1.0f)] private float m_movementSpeed = 0.0f;
        [SerializeField] [ShowAssetPreview] private GameObject m_modelPrefab = null;
        // Prefab for the battle object we need to spawn in battle (local)
        [SerializeField] [ShowAssetPreview] private GameObject m_battleLocalPrefab = null;
        // Prefab for the battle object we need to spawn in battle (network)
        [SerializeField] [ShowAssetPreview] private GameObject m_battleNetworkPrefab = null;
        // Prefab for the build ui scene
        [SerializeField] [ShowAssetPreview] private GameObject m_buildUIPrefab = null;
        // Prefab for a preview of the part for the build ui scene
        [SerializeField] [ShowAssetPreview] private GameObject m_buildUIPreviewPrefab = null;
        // List of input actions for the part to have
        [SerializeField] private List<actionInfo> m_actionList = new List<actionInfo>(1);
        // UI associated with a part
        [SerializeField] private PartUIData m_partUIData = null;

        public string partName => m_partName;
        public ePartType partType => m_partType;
        public string partID => m_partID.value;
        public int weight => m_weight;
        public float health => m_health;
        public float movementSpeed => m_movementSpeed;
        public GameObject modelPrefab => m_modelPrefab;
        public GameObject battleLocalPrefab => m_battleLocalPrefab;
        public GameObject battleNetworkPrefab => m_battleNetworkPrefab;
        public GameObject buildUIPrefab => m_buildUIPrefab;
        public GameObject buildUIPreviewPrefab => m_buildUIPreviewPrefab;
        public IReadOnlyList<actionInfo> actionList => m_actionList;
        public PartUIData partUIData => m_partUIData;


        /// <summary>
        /// Sets the variables of the PartScriptableObject.
        /// ONLY USE THIS FROM EDITOR SCRIPTS THAT GENERATE THIS OBJECT.
        /// </summary>
        /// <param name="name">Display name of the part.</param>
        /// <param name="type">Display/connection type of the part.</param>
        /// <param name="id">Reference to unique StringID for the part.</param>
        /// <param name="partWeight">Weight of the part.</param>
        /// <param name="partHealth">Health added due to the part.</param>
        /// <param name="speed">Speed added due to the part.</param>
        /// <param name="modelPref">Prefab for the build scene prefab.<param>
        /// <param name="battleLocalPref">Prefab for the battle scene prefab. (local version)</param>
        /// <param name="battleNetworkPref">Prefab for the battle scene prefab. (network version)</param>
        /// <param name="actions">List of action names for the parts.</param>
        /// <param name="uiData">UI part data.</param>
        public void Initialize(string name, ePartType type, StringID id, int partWeight, float partHealth,
            float speed, GameObject modelPref, GameObject battleLocalPref, GameObject battleNetworkPref,
            List<actionInfo> actions, PartUIData uiData)
        {
            m_partName = name;
            m_partType = type;
            m_partID = id;
            m_weight = partWeight;
            m_health = partHealth;
            m_movementSpeed = speed;
            m_modelPrefab = modelPref;
            m_battleLocalPrefab = battleLocalPref;
            m_battleNetworkPrefab = battleNetworkPref;
            m_actionList = actions;
            m_partUIData = uiData;
        }
        
    }

    public enum eActionType
    {
        Analog,
        Vector1,
        Vector2
    }

    public enum eIconType
    {
        FireCharge,
        Rotate,
        RaiseLower,
        Turn
    }

    [System.Serializable]
    public struct actionInfo
    {
        [SerializeField]
        private string m_action;
        [SerializeField]
        private eActionType m_actionType;
        [SerializeField]
        private eIconType m_IconType;
        [SerializeField]
        private bool m_hasCooldown;

        public string action => m_action;
        public eActionType actionType => m_actionType;
        public eIconType iconType => m_IconType;
        public bool hasCooldown => m_hasCooldown;


        public string getname()
        {
            return m_action;
        }
    }
}
