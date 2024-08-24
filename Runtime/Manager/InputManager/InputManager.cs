using UnityEngine;
using Runtime.Common.Singleton;
using Runtime.Message;
using Runtime.Definition;

namespace Runtime.Manager.Input
{
    public class InputManager : MonoSingleton<InputManager>
    {
        #region Members

        [SerializeField]
        private Joystick _joystick;

        #endregion Members

        #region Properties

        public IVector3Input MovementInput => _joystick;

        #endregion Properties

        #region API Methods

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
                Messenger.Publish(new GameStateChangedMessage(GameStateEventType.PressBackKey));
        }

        #endregion API Methods
    }
}