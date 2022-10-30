using System;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Data to send to the client for a posting a Wwise event.
    /// </summary>
    [Serializable]
    public class WwiseEventClientData
    {
        public readonly string eventName = "UNINIT";
        public readonly TransformChildPath pathToOwner = null;


        public WwiseEventClientData(string nameOfEvent,
            TransformChildPath ownerPath)
        {
            eventName = nameOfEvent;
            pathToOwner = ownerPath;
        }
    }
}
