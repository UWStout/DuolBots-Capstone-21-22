using Mirror;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Extensions for the <see cref="NetworkWriter"/> and
    /// <see cref="NetworkReader"/> classes for <see cref="WwiseEventClientData"/>
    /// to be sent over the network.
    /// </summary>
    public static class WwiseEventClientDataReaderWriter
    {
        public static void WriteWwiseEventClientData(this NetworkWriter writer,
            WwiseEventClientData clientData)
        {
            writer.Write(clientData.eventName);    // string
            writer.Write(clientData.pathToOwner);   // TransformChildPath
            // TransformChildPath has its own extension,
            // so the above should be fine.
        }
        public static WwiseEventClientData ReadWwiseEventClientData(
            this NetworkReader reader)
        {
            string temp_eventName = reader.Read<string>();
            TransformChildPath temp_pathToOwner = reader.Read<TransformChildPath>();
            return new WwiseEventClientData(temp_eventName, temp_pathToOwner);
        }
    }
}
