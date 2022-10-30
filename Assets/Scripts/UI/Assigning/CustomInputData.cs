using UnityEngine.InputSystem;

namespace DuolBots
{
    /// <summary>
    ///
    /// NOTE: If this class is ever updated please also update
    /// CustomInputDataReadWriter for packing purposes. If you need
    /// help updating this, ask Wyatt.
    ///
    /// We use InputValue and messaging instead of InputAction.CallbackContext
    /// with UnityEvents because there is some bug that randomly will make
    /// member variables null when the event is invoked. Messages avoids that.
    /// </summary>
    public class CustomInputData
    {
        private object m_data = null;
        private readonly bool m_isPressed = false;

        private readonly InputValue m_trueInputValue = null;
        private readonly eInputType m_inputType = eInputType.buttonEast;
        public eInputType inputType => m_inputType;


        public CustomInputData()
        {
            m_data = 0.0f;
            m_isPressed = false;
            m_inputType = eInputType.buttonEast;
        }
        public CustomInputData(object data, bool pressed, eInputType type = eInputType.buttonEast)
        {
            m_data = data;
            m_isPressed = pressed;
            m_inputType = type;
        }
        public CustomInputData(InputValue inpVal, eInputType type = eInputType.buttonEast)
        {
            m_trueInputValue = inpVal;
            m_inputType = type;
        }


        public T Get<T>() where T : struct
        {
            if (m_trueInputValue != null)
            {
                return m_trueInputValue.Get<T>();
            }

            return (T)m_data;
        }
        public object Get()
        {
            if (m_trueInputValue != null)
            {
                return m_trueInputValue.Get();
            }

            return m_data;
        }
        public bool isPressed
        {
            get
            {
                if (m_trueInputValue != null)
                {
                    return m_trueInputValue.isPressed;
                }
                return m_isPressed;
            }
        }


        public void SetData(object _data)
        {
            m_data = _data;
        }

        public override string ToString()
        {
            return $"isPressed: {isPressed}. data: {Get()}";
        }
    }
}
