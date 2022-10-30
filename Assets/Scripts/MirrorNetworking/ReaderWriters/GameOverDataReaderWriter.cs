using Mirror;
// Original Author - Wyatt Senalik

namespace DuolBots.Mirror
{
    public static class GameOverDataReaderWriter
    {
        private const bool IS_DEBUGGING = true;


        public static void WriteGameOverData(this NetworkWriter writer,
                GameOverData goData)
        {
            writer.Write((byte)goData.cause); // byte
            #region Logs
            CustomDebug.Log($"{nameof(GameOverDataReaderWriter)} " +
                $"wrote a byte with value {(byte)goData.cause} " +
                $"for sending {nameof(GameOverData)}.", IS_DEBUGGING);
            #endregion Logs
            writer.Write(goData.winningTeamIndices.Count);    // int
            #region Logs
            CustomDebug.Log($"{nameof(GameOverDataReaderWriter)} " +
                $"wrote an int with value {goData.winningTeamIndices.Count} " +
                $"for sending {nameof(GameOverData)}.", IS_DEBUGGING);
            #endregion Logs
            foreach (byte temp_b in goData.winningTeamIndices)
            {
                writer.Write(temp_b);   // byte
                #region Logs
                CustomDebug.Log($"{nameof(GameOverDataReaderWriter)} " +
                    $"wrote an byte with value {temp_b} " +
                    $"for sending {nameof(GameOverData)}.", IS_DEBUGGING);
                #endregion Logs
            }
        }
        public static GameOverData ReadGameOverData(this NetworkReader reader)
        {
            // Convert the array of bytes back into an object
            eGameOverCause temp_cause = (eGameOverCause)reader.Read<byte>();
            #region Logs
            CustomDebug.Log($"{nameof(GameOverDataReaderWriter)} " +
                $"read a byte with value {(byte)temp_cause} " +
                $"for receiving {nameof(GameOverData)}.", IS_DEBUGGING);
            #endregion Logs
            int temp_arrLength = reader.Read<int>();
            #region Logs
            CustomDebug.Log($"{nameof(GameOverDataReaderWriter)} " +
                $"read an int with value {temp_arrLength} " +
                $"for receiving {nameof(GameOverData)}.", IS_DEBUGGING);
            #endregion Logs
            byte[] temp_winningTeamIndices = new byte[temp_arrLength];
            for (int i = 0; i < temp_arrLength; ++i)
            {
                byte temp_b = reader.Read<byte>();
                #region Logs
                CustomDebug.Log($"{nameof(GameOverDataReaderWriter)} " +
                    $"read a byte with value {temp_b} " +
                    $"for receiving {nameof(GameOverData)}.", IS_DEBUGGING);
                #endregion Logs
                temp_winningTeamIndices[i] = temp_b;
            }

            return new GameOverData(temp_cause,
                temp_winningTeamIndices);
        }
    }
}
