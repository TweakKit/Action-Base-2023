using UnityEngine;
using UnityEngine.EventSystems;
using Runtime.Utilities;

namespace Runtime.Manager.Input
{
    public class Joystick : MonoBehaviour, IVector3Input, IDragHandler, IPointerDownHandler, IPointerUpHandler, IIgnorePointer
    {
        #region Members

        [SerializeField]
        protected float deadZone;
        [SerializeField]
        protected RectTransform joystickBackgroundTransform;
        [SerializeField]
        protected RectTransform joystickKnobTransform;

        protected Vector2 joystickBackgroundHalfSizeDelta;
        protected RectTransform joystickContainerTransform;
        protected Canvas worldCanvas;
        protected Vector2 input;
        protected Vector2 originalJoystickBackgroundPosition;

        #endregion Members

        #region Properties

        public Vector3 Input => input.normalized;

        #endregion Properties

        #region API Methods

        protected virtual void Start()
        {
            joystickContainerTransform = gameObject.GetComponent<RectTransform>();
            worldCanvas = gameObject.GetComponentInParent<Canvas>();
            joystickBackgroundTransform.pivot = Vector2.one * 0.5f;
            joystickKnobTransform.pivot = Vector2.one * 0.5f;
            joystickKnobTransform.anchoredPosition = Vector2.zero;
            joystickBackgroundHalfSizeDelta = joystickBackgroundTransform.sizeDelta / 2;
            originalJoystickBackgroundPosition = joystickBackgroundTransform.anchoredPosition;
        }

        #endregion API Methods

        #region Class Methods

        public void Reset()
        {
            input = Vector2.zero;
            joystickKnobTransform.anchoredPosition = Vector2.zero;
        }

        public void Disable()
        {
            Reset();
            gameObject.SetActive(false);
        }

        public void Enable()
        {
            Reset();
            gameObject.SetActive(true);
        }

        public virtual void OnPointerDown(PointerEventData pointerEventData) => OnDrag(pointerEventData);

        public virtual void OnDrag(PointerEventData pointerEventData)
        {
            Vector2 joystickBackgroundScreenPosition = RectTransformUtility.WorldToScreenPoint(pointerEventData.pressEventCamera, joystickBackgroundTransform.position);
            input = (pointerEventData.position - joystickBackgroundScreenPosition) / (joystickBackgroundHalfSizeDelta * worldCanvas.scaleFactor);
            CheckAndUpdateInput(input.sqrMagnitude, input);
            joystickKnobTransform.anchoredPosition = input * joystickBackgroundHalfSizeDelta;
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            input = Vector2.zero;
            joystickKnobTransform.anchoredPosition = Vector2.zero;
        }

        protected virtual void CheckAndUpdateInput(float inputSqrMagnitude, Vector2 input)
        {
            if (inputSqrMagnitude > deadZone)
            {
                if (inputSqrMagnitude > 1)
                    this.input = input.normalized;
                else
                    this.input = input;
            }
            else this.input = Vector2.zero;
        }

        #endregion Class Methods
    }
}