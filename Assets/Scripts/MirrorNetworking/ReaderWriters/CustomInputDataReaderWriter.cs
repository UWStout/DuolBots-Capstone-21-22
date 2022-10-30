using Mirror;
// Original Author - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Extensions for NetworkWriter and NetworkReader for sending a
    /// CustomInputData over the network.
    /// </summary>
    public static class CustomInputDataReaderWriter
    {
        public static void WriteCustomInputData(this NetworkWriter writer,
            CustomInputData inpData)
        {
            writer.Write(inpData.isPressed);    // bool
            writer.Write(inpData.inputType);    // eInputType

            // Convert the object into an array of bytes and write that
            object temp_inpDataObj = inpData.Get();
            byte[] temp_objByteArr = temp_inpDataObj.ToByteArray();
            writer.Write(temp_objByteArr.Length);   // int
            foreach (byte temp_b in temp_objByteArr)
            {
                writer.Write(temp_b);   // byte
            }
        }
        public static CustomInputData ReadCustomInputData(this NetworkReader reader)
        {
            bool temp_isPressed = reader.Read<bool>();
            eInputType temp_type = reader.Read<eInputType>();

            // Convert the array of bytes back into an object
            int temp_objByteArrLength = reader.Read<int>();
            byte[] temp_objByeArr = new byte[temp_objByteArrLength];
            for (int i = 0; i < temp_objByteArrLength; i++)
            {
                byte temp_b = reader.Read<byte>();
                temp_objByeArr[i] = temp_b;
            }
            object temp_inpDataObj = temp_objByeArr.ToObject();

            return new CustomInputData(temp_inpDataObj, temp_isPressed, temp_type);
        }

        
    }
}
