using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// This Class Saves Bindings to a dictonary
/// </summary>
namespace DuolBots
{
    public class SavingRebinds : MonoBehaviour
    {
        public InputActionAsset m_control;
        //private BuildSceneInputData m_buildSceneInputData;
        private CustomInputBinding m_customInputBinding;

        /// <summary>
        /// Private wrapper class for json serialization of the overrides
        /// </summary>
        [System.Serializable]
        class BindingWrapperClass
        {
            public List<BindingSerializable> bindingList = new List<BindingSerializable>();
        }

        /// <summary>
        /// internal struct to store an id overridepath pair for a list
        /// </summary>
        [System.Serializable]
        private struct BindingSerializable
        {
            public string id;
            public string path;

            public BindingSerializable(string bindingId, string bindingPath)
            {
                id = bindingId;
                path = bindingPath;
            }
        }

        /// <summary>
        /// Loads the saved rebinds at start of scene
        /// </summary>
        private void Start()
        {
            LoadControlOverrides();
        }

        /// <summary>
        /// Saves New Rebinds once scene is disabled
        /// </summary>
        private void OnDisable()
        {
            StoreControlOverrides();
        }

        /// <summary>
        /// stores the active control overrides to player prefs
        /// </summary>
        public void StoreControlOverrides()
        {
            //saving
            BindingWrapperClass bindingList = new BindingWrapperClass();
            foreach (var map in m_control.actionMaps)
            {
                foreach (var binding in map.bindings)
                {
                    if (!string.IsNullOrEmpty(binding.overridePath))
                    {
                        bindingList.bindingList.Add(new BindingSerializable(binding.id.ToString(), binding.overridePath));
                    }
                }
            }

            PlayerPrefs.SetString("ControlOverrides", JsonUtility.ToJson(bindingList));
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Loads control overrides from playerprefs
        /// </summary>
        public void LoadControlOverrides()
        {
            if (PlayerPrefs.HasKey("ControlOverrides"))
            {
                BindingWrapperClass bindingList = JsonUtility.FromJson(PlayerPrefs.GetString("ControlOverrides"), typeof(BindingWrapperClass)) as BindingWrapperClass;

                //create a dictionary to easier check for existing overrides
                Dictionary<System.Guid, string> overrides = new Dictionary<System.Guid, string>();
                foreach (var item in bindingList.bindingList)
                {
                    overrides.Add(new System.Guid(item.id), item.path);
                }

                //walk through action maps check dictionary for overrides
                foreach (var map in m_control.actionMaps)
                {
                    var bindings = map.bindings;
                    for (var i = 0; i < bindings.Count; ++i)
                    {
                        if (overrides.TryGetValue(bindings[i].id, out string overridePath))
                        {
                            //if there is an override apply it
                            map.ApplyBindingOverride(i, new InputBinding { overridePath = overridePath });
                            // TODO: Put CustomInputBindings DATA here

                            // TODO: Save Data to BuildSceneInputData using SetData Function
                        }
                    }
                }
            }
        }

    }
}
