using Mirror;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    /// <summary>
    /// Extensions for <see cref="NetworkWriter"/> and
    /// <see cref="NetworkReader"/> for sending <see cref="TransformChildPath"/>
    /// over the network.
    /// </summary>
    public static class TransformChildPathReaderWriter
    {
        public static void WriteTransformData(this NetworkWriter writer,
            TransformChildPath transData)
        {
            // Amount
            writer.Write(transData.path.Count); // int
            // Individual path points
            foreach (int temp_siblingIndex in transData.path)
            {
                writer.Write(temp_siblingIndex);    // int
            }
        }
        public static TransformChildPath ReadTransformData(this NetworkReader reader)
        {
            // Amount
            int temp_pathSize = reader.Read<int>();
            int[] temp_pathArray = new int[temp_pathSize];
            // Individual path points
            for (int i = 0; i < temp_pathSize; ++i)
            {
                temp_pathArray[i] = reader.Read<int>();
            }

            return new TransformChildPath(temp_pathArray);
        }
    }
}
