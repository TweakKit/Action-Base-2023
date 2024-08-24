using UnityEngine;
using UnityEngine.EventSystems;

namespace Runtime.UI
{
    [DisallowMultipleComponent]
    public class PinchInputDetector : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        #region Members

        private IPinchStartHandler[] _pinchStartHandlers;
        private IPinchEndHandler[] _pinchEndHandlers;
        private IPinchZoomHandler[] _pinchZoomHandlers;
        private int _touchCount;
        private bool _isPinching;
        private float _previousDistance;
        private float _delta;
        private PointerEventData _firstPointerEventData;
        private PointerEventData _secondPointerEventData;

        #endregion Members

        #region API Methods

        private void Awake()
        {
            _pinchStartHandlers = gameObject.GetComponents<IPinchStartHandler>();
            _pinchEndHandlers = gameObject.GetComponents<IPinchEndHandler>();
            _pinchZoomHandlers = gameObject.GetComponents<IPinchZoomHandler>();
        }

        #endregion API Methods

        #region Class Methods

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            RegisterPointer(eventData);
            if (_touchCount == 1)
                return;
            if (_isPinching)
                eventData.Use();
            else if (!IsEqualPointer(_firstPointerEventData, eventData) && !IsEqualPointer(_secondPointerEventData, eventData))
                eventData.Use();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            UnregisterPointer(eventData);
            if (_touchCount == 0)
                return;
            if (_isPinching)
                eventData.Use();
            else if (!IsEqualPointer(_firstPointerEventData, eventData) && !IsEqualPointer(_secondPointerEventData, eventData))
                eventData.Use();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_touchCount == 0)
                return;
            if (IsEqualPointer(_firstPointerEventData, eventData))
            {
                _firstPointerEventData = eventData;
                if (_secondPointerEventData != null)
                    CalculateDistanceDelta();
                if (_isPinching)
                {
                    eventData.Use();
                    FireOnPinchZoom(new PinchEventData(_firstPointerEventData, _secondPointerEventData, _delta));
                }
            }
            else if (IsEqualPointer(_secondPointerEventData, eventData))
            {
                _secondPointerEventData = eventData;
                if (_firstPointerEventData != null)
                    CalculateDistanceDelta();
                if (_isPinching)
                {
                    eventData.Use();
                    FireOnPinchZoom(new PinchEventData(_secondPointerEventData, _firstPointerEventData, _delta));
                }
            }
            else eventData.Use();
        }

        private void RegisterPointer(PointerEventData eventData)
        {
            _touchCount++;
            if (_firstPointerEventData == null)
            {
                _firstPointerEventData = eventData;
            }
            else if (_secondPointerEventData == null)
            {
                _secondPointerEventData = eventData;
                CalculateDistanceDelta();
                if (_touchCount >= 2)
                {
                    _isPinching = true;
                    FireOnPinchStart(new PinchEventData(_secondPointerEventData, _firstPointerEventData));
                }
            }
        }

        private void UnregisterPointer(PointerEventData eventData)
        {
            _touchCount--;
            if (_touchCount < 0)
                return;
            if (IsEqualPointer(_firstPointerEventData, eventData))
            {
                if (_isPinching)
                {
                    _isPinching = false;
                    FireOnPinchEnd(new PinchEventData(_firstPointerEventData, _secondPointerEventData));
                }
                if (_secondPointerEventData != null)
                {
                    _firstPointerEventData = _secondPointerEventData;
                    _secondPointerEventData = null;
                }
                else _firstPointerEventData = null;
            }
            else if (IsEqualPointer(_secondPointerEventData, eventData))
            {
                if (_isPinching)
                {
                    _isPinching = false;
                    FireOnPinchEnd(new PinchEventData(_secondPointerEventData, _firstPointerEventData));
                }
                _secondPointerEventData = null;
            }
        }

        private bool IsEqualPointer(PointerEventData a, PointerEventData b)
        {
            if (a == null)
                return false;
            if (b == null)
                return false;
            return a.pointerId == b.pointerId;
        }

        private void FireOnPinchStart(PinchEventData data)
        {
            if (_pinchStartHandlers == null)
                return;
            for (int i = 0; i < _pinchStartHandlers.Length; i++)
                _pinchStartHandlers[i].OnPinchStart(data);
        }

        private void FireOnPinchEnd(PinchEventData data)
        {
            if (_pinchEndHandlers == null)
                return;
            for (int i = 0; i < _pinchEndHandlers.Length; i++)
                _pinchEndHandlers[i].OnPinchEnd(data);
        }

        private void FireOnPinchZoom(PinchEventData data)
        {
            if (_pinchZoomHandlers == null)
                return;
            for (int i = 0; i < _pinchZoomHandlers.Length; i++)
                _pinchZoomHandlers[i].OnPinchZoom(data);
        }

        private void CalculateDistanceDelta()
        {
            float newDistance = Vector2.Distance(_firstPointerEventData.position, _secondPointerEventData.position);
            _delta = newDistance - _previousDistance;
            _previousDistance = newDistance;
        }

        #endregion Class Methods
    }
}