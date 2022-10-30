using Mirror;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Extensions for NetworkWriter and NetworkReader for sending a
    /// CustomInputBinding over the network.
    /// </summary>
    public static class CustomInputBindingReaderWriter
    {
        public static void WriteCustomInputBinding(this NetworkWriter writer,
            CustomInputBinding inpBinding)
        {
            writer.Write(inpBinding.playerIndex);   // byte
            writer.Write(inpBinding.actionIndex);   // byte
            writer.Write((byte)inpBinding.inputType);   // byte
            writer.Write(inpBinding.partSlotID);    // byte
            writer.Write(inpBinding.partUniqueID);  // string
        }
        public static CustomInputBinding ReadCustomInputBinding(this NetworkReader reader)
        {
            byte temp_playerIndex = reader.Read<byte>();
            byte temp_actionIndex = reader.Read<byte>();
            eInputType temp_inputType = (eInputType)reader.Read<byte>();
            byte temp_partSlotID = reader.Read<byte>();
            string temp_partUniqueID = reader.Read<string>();

            return new CustomInputBinding(temp_playerIndex, temp_actionIndex,
                temp_inputType, temp_partSlotID, temp_partUniqueID);
        }
    }
}
