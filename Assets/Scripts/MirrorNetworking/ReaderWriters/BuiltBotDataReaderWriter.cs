using System.Collections.Generic;

using Mirror;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    public static class BuiltBotDataReaderWriter
    {
        public static void WriteBuiltBotData(this NetworkWriter writer, BuiltBotData botData)
        {
            writer.Write(botData.chassisID);    // string
            writer.Write(botData.movementPartID);   // string
            writer.Write(botData.slottedPartIDList.Count);  // int
            foreach (PartInSlot temp_partInSlot in botData.slottedPartIDList)
            {
                writer.Write(temp_partInSlot.partID); // string
                writer.Write(temp_partInSlot.slotIndex); // byte
            }
        }
        public static BuiltBotData ReadBuiltBotData(this NetworkReader reader)
        {
            string temp_chassisID = reader.Read<string>();
            string temp_movementPartID = reader.Read<string>();
            int temp_amountSlottedParts = reader.Read<int>();
            List<PartInSlot> temp_partInSlotList = new List<PartInSlot>(temp_amountSlottedParts);
            for (int i = 0; i < temp_amountSlottedParts; i++)
            {
                string temp_partID = reader.Read<string>();
                byte temp_slotIndex = reader.Read<byte>();

                PartInSlot temp_partInSlot = new PartInSlot(temp_partID, temp_slotIndex);
                temp_partInSlotList.Add(temp_partInSlot);
            }
            return new BuiltBotData(temp_chassisID, temp_movementPartID, temp_partInSlotList);
        }
    }
}
