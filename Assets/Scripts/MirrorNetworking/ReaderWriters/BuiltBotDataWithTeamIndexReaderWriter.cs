using Mirror;
// Original Authors - Wyatt Senalik

namespace DuolBots.Mirror
{
    public static class BuiltBotDataWithTeamIndexReaderWriter
    {
        public static void WriteBuiltBotDataWithTeamIndex(this NetworkWriter writer,
            BuiltBotDataWithTeamIndex botData)
        {
            writer.Write(botData.botData);    // BuiltBotData
            writer.Write(botData.teamIndex);   // byte
        }
        public static BuiltBotDataWithTeamIndex ReadBuiltBotDataWithTeamIndex(
            this NetworkReader reader)
        {
            BuiltBotData temp_botData = reader.Read<BuiltBotData>();
            byte temp_teamIndex = reader.Read<byte>();
            return new BuiltBotDataWithTeamIndex(temp_botData, temp_teamIndex);
        }
    }
}
