using UnityEngine;
using UnityEngine.EventSystems;

namespace Runtime.Manager.Input
{
    public class TutorialCustomJoystick : CustomJoystick
    {
        #region Members

        [SerializeField]
        private GameObject _tutorialJoystick;

        #endregion Members

        #region API Methods

        protected override void Start()
        {
            base.Start();
            var canShowTutorialJoystick = true;
            _tutorialJoystick.SetActive(canShowTutorialJoystick);
        }

        #endregion API Methods

        #region Class Methods

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            _tutorialJoystick.SetActive(false);
        }

        #endregion Class Methods
    }
}