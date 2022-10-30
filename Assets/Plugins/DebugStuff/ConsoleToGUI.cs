using UnityEngine;
// https://answers.unity.com/questions/125049/is-there-any-way-to-view-the-console-in-a-build.html
// Tweaked by Wyatt Senalik

namespace DebugStuff
{
    public class ConsoleToGUI : MonoBehaviour
    {
        [SerializeField] [Range(0.0f, 1.0f)] private float m_xScreenPos = 0.75f;
        [SerializeField] [Range(0.0f, 1.0f)] private float m_yScreenPos = 0.75f;
        [SerializeField] [Range(0.0f, 1.0f)] private float m_widthScreenSize = 0.24f;
        [SerializeField] [Range(0.0f, 1.0f)] private float m_heightScreenSize = 0.24f;

        [SerializeField] private bool m_hideInEditor = true;

        [SerializeField] private bool m_includeErrors = false;

//#if !UNITY_EDITOR
        static string myLog = "";
        private string output;
        private string stack;

        void OnEnable()
        {
            Application.logMessageReceived += Log;
        }

        void OnDisable()
        {
            Application.logMessageReceived -= Log;
        }

        public void Log(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Error && !m_includeErrors) { return; }

            output = logString;
            stack = stackTrace;
            myLog = output + "\n" + myLog;
            if (myLog.Length > 5000)
            {
                myLog = myLog.Substring(0, 4000);
            }
        }

        void OnGUI()
        {
            //Do not display in editor ( or you can use the UNITY_EDITOR macro to also disable the rest)
            if (m_hideInEditor && Application.isEditor) { return; }

            float x = Screen.width * m_xScreenPos;
            float y = Screen.height * m_yScreenPos;
            float width = Screen.width * m_widthScreenSize;
            float height = Screen.height * m_heightScreenSize;
            myLog = GUI.TextArea(new Rect(x, y, width, height), myLog);
        }
//#endif
    }
}
