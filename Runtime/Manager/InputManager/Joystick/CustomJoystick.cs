using UnityEngine;
using UnityEngine.EventSystems;
using Runtime.Extensions;

namespace Runtime.Manager.Input
{
    public class CustomJoystick : Joystick
    {
        #region Members

        [SerializeField]
        private Transform _directionVisualTransform;

        #endregion Members

        #region API Methods

        protected override void Start()
        {
            base.Start();
            _directionVisualTransform.gameObject.SetActive(false);
        }

        #endregion API Methods

        #region Class Methods

        public override void OnPointerDown(PointerEventData eventData)
        {
            _directionVisualTransform.gameObject.SetActive(true);
            joystickBackgroundTransform.anchoredPosition = ScreenPointToAnchoredPosition(eventData);
            base.OnPointerDown(eventData);
        }

        public override void OnDrag(PointerEventData pointerEventData)
        {
            base.OnDrag(pointerEventData);
            _directionVisualTransform.transform.rotation = Input.ToQuaternion();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            _directionVisualTransform.gameObject.SetActive(false);
            joystickBackgroundTransform.anchoredPosition = originalJoystickBackgroundPosition;
            base.OnPointerUp(eventData);
        }

        private Vector2 ScreenPointToAnchoredPosition(PointerEventData pointerEventData)
        {
            Vector2 localPosition = Vector2.zero;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickContainerTransform, pointerEventData.position, pointerEventData.pressEventCamera, out localPosition))
            {
                Vector2 pivotOffset = new Vector2(joystickContainerTransform.rect.width * 0.5f, joystickContainerTransform.rect.height * 0.5f);
                return localPosition + pivotOffset;
            }
            return Vector2.zero;
        }

        #endregion Class Methods
    }
}