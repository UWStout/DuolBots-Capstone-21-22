using System.Collections.Generic;

using Mirror;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Extensions for NetworkWriter and NetworkReader for sending a
    /// BuiltBotInputData over the network.
    /// </summary>
    public static class BuiltBotInputDataReaderWriter
    {
        public static void WriteBuiltBotInputData(this NetworkWriter writer,
            BuiltBotInputData inpData)
        {
            IReadOnlyList<CustomInputBinding> temp_customInpBindingsList =
                inpData.customInputBindings;
            int temp_amountInpBindings = temp_customInpBindingsList.Count;

            writer.Write(temp_amountInpBindings);   // int
            foreach (CustomInputBinding temp_singleInpBind in
                temp_customInpBindingsList)
            {
                writer.Write(temp_singleInpBind);   // CustomInputBinding
            }
        }
        public static BuiltBotInputData ReadBuiltBotInputData(
            this NetworkReader reader)
        {
            int temp_amountInpBindings = reader.Read<int>();
            List<CustomInputBinding> temp_inpBindList =
                new List<CustomInputBinding>(temp_amountInpBindings);
            for (int i = 0; i < temp_amountInpBindings; ++i)
            {
                CustomInputBinding temp_inpBind = reader.Read<CustomInputBinding>();
                temp_inpBindList.Add(temp_inpBind);
            }

            return new BuiltBotInputData(temp_inpBindList);
        }
    }
}
