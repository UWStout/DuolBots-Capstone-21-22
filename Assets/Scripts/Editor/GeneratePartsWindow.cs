using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;


namespace DuolBots
{
    public class GeneratePartsWindow : EditorWindow
    {
        private SerializedObject m_selectedPart = null;
        private PartScriptableObject m_selectedScriptable = null;
        private Vector2 m_scrollPosition = new Vector2(0, 0);
        // Add menu item named "My Window" to the Window menu
        [MenuItem("Window/Scriptable Parts")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow.GetWindow(typeof(GeneratePartsWindow));
            System.Type myType = typeof(FilePaths);
            FieldInfo[] fields = myType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
            //When this window is shown generate any missing directories that will be needed
            foreach (FieldInfo fi in fields)
            {
                if (!System.IO.Directory.Exists(fi.GetValue(null).ToString()))
                {
                    System.IO.Directory.CreateDirectory(fi.GetValue(null).ToString());
                }
            }
            //Refresh asset database
            FilePaths.REFRESHASSETDATABASE();
        }

        void OnGUI()
        {
            // These GUILayouts do not need to be tabbed but they are like this to show the formatting of the window
            GUILayout.Label("Scriptable Objects", EditorStyles.boldLabel);

            // Check if anything after this points changes
            EditorGUI.BeginChangeCheck();
            #region Edit Scriptables
                GUILayout.BeginHorizontal();
                        GUILayout.BeginVertical();
                            m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition,GUILayout.Width(200),GUILayout.Height(375));
                                // Get all the parts we have that are currently generated and make buttons for them and if the corresponding button is pushed, change the referenced items
                                PartScriptableObject[] parts = Resources.LoadAll<PartScriptableObject>(FilePaths.SCRIPTABLEPARTS.Substring(FilePaths.SCRIPTABLEPARTS.LastIndexOf("Resources")).Substring(10).Replace('\\', '/'));
                                foreach (PartScriptableObject part in parts)
                                {
                                    if (GUILayout.Button(part.name))
                                    {
                                        m_selectedPart = new SerializedObject(part);
                                        m_selectedScriptable = part;
                                    }
                                }
                                // Default to the first position
                                if (m_selectedPart == null && parts.Length>0)
                                {
                                    m_selectedScriptable = parts[0];
                                    m_selectedPart = new SerializedObject(parts[0]);
                                }
                            EditorGUILayout.EndScrollView();
                        GUILayout.EndVertical();
                        GUILayout.BeginVertical();
                        if (m_selectedPart != null)
                        {
                            // Get the Properties with correct formating 
                            EditorGUILayout.PropertyField(m_selectedPart.FindProperty("m_partName"), new GUIContent("Part Name"));
                            EditorGUILayout.PropertyField(m_selectedPart.FindProperty("m_partType"), new GUIContent("Part Type"));
                            EditorGUILayout.PropertyField(m_selectedPart.FindProperty("m_partID"), new GUIContent("Part ID"));
                            EditorGUILayout.PropertyField(m_selectedPart.FindProperty("m_weight"), new GUIContent("Weight"));
                            EditorGUILayout.PropertyField(m_selectedPart.FindProperty("m_health"), new GUIContent("Health"));
                            EditorGUILayout.PropertyField(m_selectedPart.FindProperty("m_movementSpeed"), new GUIContent("Movement Speed"));
                            EditorGUILayout.PropertyField(m_selectedPart.FindProperty("m_modelPrefab"), new GUIContent("Model Prefab"));
                            EditorGUILayout.PropertyField(m_selectedPart.FindProperty("m_battleLocalPrefab"), new GUIContent("Battle Local Prefab"));
                            EditorGUILayout.PropertyField(m_selectedPart.FindProperty("m_battleNetworkPrefab"), new GUIContent("Battle Network Prefab"));
                            EditorGUILayout.PropertyField(m_selectedPart.FindProperty("m_actionList"), new GUIContent("Action List"));
                            EditorGUILayout.PropertyField(m_selectedPart.FindProperty("m_partUIData"), new GUIContent("Part UI Data"));
                        }
                        GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                #endregion Edit Scriptables
                GUILayout.Space(15);

                // Button to Generate all Scriptable objects and all parts that can be automated
                if (GUILayout.Button("Generate"))
                {
                    // Only proceed if the directory exists
                    if (System.IO.Directory.Exists(FilePaths.PARTMODELS))
                    {
                        // Get all directories in the target directory
                        string[] temp_Folders = System.IO.Directory.GetDirectories(FilePaths.PARTMODELS);
                        // In each of those sub directories
                        foreach (string temp_Folder in temp_Folders)
                        {
                            // Make a list of all the parts in the sub Directory
                            List<GameObject> temp_Parts = new List<GameObject>(Resources.LoadAll<GameObject>(temp_Folder.Substring(temp_Folder.LastIndexOf("Resources")).Substring(10).Replace('\\', '/')));
                            // For each part in the list
                            foreach (GameObject o in temp_Parts)
                            {
                                // Collect information and create instances of needed things
                                GameObject temp_Instantiated = Instantiate(o);
                                //PartImages.MakeAnimation(ref temp_Instantiated, 60);
                                PartImages.TakeImages(ref temp_Instantiated);
                                ePartType temp_type = ePartType.Chassis;
                                switch (temp_Folder.Substring(temp_Folder.LastIndexOf("Models\\")).Substring(7))
                                {
                                    case "Movement":
                                        temp_type = ePartType.Movement;
                                        break;
                                    case "Utility":
                                        temp_type = ePartType.Utility;
                                        break;
                                    case "Weapon":
                                        temp_type = ePartType.Weapon;
                                        break;
                                }

                                // If a needed scriptable object does not already exist 
                                if (!System.IO.File.Exists(FilePaths.GETPARTSCRIPTABLE(o.name)))
                                {
                                    // Generate PartID if there is not already one, otherwise use the current one
                                    PartUIData temp_Data = new PartUIData();
                                    temp_Data.Initialize(PartImages.LoadTexture(FilePaths.GETPARTSTILLPATH(o.name)), PartImages.LoadTexture(FilePaths.GETPARTSTILLPATH(o.name).Insert(FilePaths.GETPARTSTILLPATH(o.name).Length-4, "Locked")), null);                                
                                    if (!System.IO.File.Exists(FilePaths.GETPARTID(o.name + "_PartID").Substring(FilePaths.GETPARTID(o.name).LastIndexOf("Assets"))))
                                    {
                                        Debug.Log(FilePaths.GETPARTID(o.name + "_PartID").Substring(FilePaths.GETPARTID(o.name).LastIndexOf("Assets")));
                                        StringID stringID = CreateInstance<StringID>();
                                        stringID.Initialize(o.name);
                                        AssetDatabase.CreateAsset(stringID, FilePaths.GETPARTID(o.name + "_PartID").Substring(FilePaths.GETPARTID(o.name).LastIndexOf("Assets")));
                                        EditorUtility.SetDirty(stringID);
                                    }
                                    // Generate scriptable object and save it
                                    PartScriptableObject obj = CreateInstance<PartScriptableObject>();
                                    obj.Initialize(o.name, temp_type, AssetDatabase.LoadAssetAtPath<StringID>(FilePaths.GETPARTID(o.name + "_PartID").Substring(FilePaths.GETPARTID(o.name).LastIndexOf("Assets"))), 0, 0, 0, o, null, null, new List<actionInfo>(), temp_Data);
                                    AssetDatabase.CreateAsset(obj, FilePaths.GETPARTSCRIPTABLE(o.name).Substring(FilePaths.GETPARTSCRIPTABLE(o.name).LastIndexOf("Assets")));
                                    EditorUtility.SetDirty(obj);
                                }
                                // Destroy the gameobject so it does not linger in the scene
                                DestroyImmediate(temp_Instantiated);
                            }
                        }
                    }
                    // Save everything
                    AssetDatabase.SaveAssets();
                    FilePaths.REFRESHASSETDATABASE();
                }

            // If anything changed, apply changes
            if (EditorGUI.EndChangeCheck() && m_selectedPart != null)
            {
                m_selectedPart.ApplyModifiedProperties();
                EditorUtility.SetDirty(m_selectedScriptable);
            }
        }
    }
}
