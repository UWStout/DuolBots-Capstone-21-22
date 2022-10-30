using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEditor;
using System.Linq;

namespace DuolBots
{
    /// <summary>
    /// Literally a meme, do not press.
    /// </summary>
    public class MarkObsolete : EditorWindow
    {
        /*
        // DO NOT PRESS THIS IS A MEME
        private string m_divineIntervention = "[Obsolete(\"NOOOO Why would you use this?\",true)]";

        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/My Window")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            MarkObsolete m_obsoleteMarker = (MarkObsolete)EditorWindow.GetWindow(typeof(MarkObsolete));
            m_obsoleteMarker.Show();
        }

        void OnGUI()
        {
            if (GUILayout.Button("Don't Press"))
            {
                // Get all directories in the target directory
                List<string> Folders = new List<string>();
                List<string> Files = new List<string>();
                Folders.Add(Application.dataPath + "/Scripts");
                foreach (string f in Folders)
                {
                    Folders.AddRange(System.IO.Directory.GetDirectories(f));
                    Files.AddRange(System.IO.Directory.GetFiles(f));
                }
                foreach (string strin in Files)
                {
                    List<string> line = File.ReadAllLines(strin).ToList();
                    int temp_dum = 0;
                    foreach (string l in line)
                    {
                        if (l.Contains("public class"))
                        {
                            temp_dum = line.FindIndex(X => X.Contains(l));
                            line.Insert(temp_dum, m_divineIntervention);
                            File.WriteAllLines(strin, line);
                        }
                    }
                }
            }
        }
        */
    }
}
