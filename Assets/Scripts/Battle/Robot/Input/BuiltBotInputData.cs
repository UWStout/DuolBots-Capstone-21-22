using System.Collections.Generic;
// Original Authors - Wyatt Senalik

namespace DuolBots
{
    /// <summary>
    /// Data for the specified inputs of a built bot.
    /// </summary>
    public class BuiltBotInputData
    {
        public IReadOnlyList<CustomInputBinding> customInputBindings
            => m_customInputBindings;
        private readonly List<CustomInputBinding> m_customInputBindings =
            new List<CustomInputBinding>();


        public BuiltBotInputData()
        {
            m_customInputBindings = new List<CustomInputBinding>();
        }
        public BuiltBotInputData(IReadOnlyList<CustomInputBinding> inputBindings)
        {
            m_customInputBindings = new List<CustomInputBinding>(inputBindings);
        }


        public override string ToString()
        {
            if (m_customInputBindings == null)
            {
                return $"null {nameof(m_customInputBindings)} " +
                    $"({nameof(BuiltBotInputData)})";
            }

            string temp_str = $"{customInputBindings.Count} input bindings: ";
            foreach (CustomInputBinding temp_binding in m_customInputBindings)
            {
                temp_str += $"[{temp_binding}], ";
            }
            temp_str = temp_str.Substring(0, temp_str.Length - 2);
            return temp_str;
        }
    }
}
