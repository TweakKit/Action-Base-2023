using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Runtime.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class PinchableScrollRect : ScrollRect, IPinchStartHandler, IPinchEndHandler, IPinchZoomHandler
    {
        #region Members

        [SerializeField]
        private bool _resetOnEnable;
        [SerializeField]
        private bool _lockPinchCenter;  
        [SerializeField]
        private Vector3 _lowerScale;
        [SerializeField]
        private Vector3 _upperScale;
        [SerializeField]
        private float _pinchSensitivity = 0.01f;
        [SerializeField]
        private float _zoomMaxSpeed = 0.2f;
        [SerializeField, Range(1f, 0f)]
        private float _zoomDeceleration = 0.8f;
        private Vector2 _initialPivot;
        private Vector2 _initialAnchored;
        private Vector3 _initialScale;
        private float _zoomVelocity;
        private Vector2 _zoomPositionDelta;
        private bool _updatePivot;
        private bool _isZooming;
        private Vector2 _pinchStartPosition;
        private bool _isInitialized;

        #endregion Members

        #region API Methods

        protected override void Start()
        {
            base.Start();
            _initialPivot = content.pivot;
            _initialAnchored = content.anchoredPosition;
            _initialScale = content.localScale;
            _isInitialized = true;
            if (_resetOnEnable)
                ResetContent();
        }

        protected virtual void Update()
        {
            if (Mathf.Abs(_zoomVelocity) > 0.001f)
            {
                if (_zoomVelocity > 0f)
                {
                    if (_zoomVelocity > _zoomMaxSpeed)
                        HandleZoom(_zoomMaxSpeed);
                    else
                        HandleZoom(_zoomVelocity);
                }
                else
                {
                    if (_zoomVelocity < -_zoomMaxSpeed)
                        HandleZoom(-_zoomMaxSpeed);
                    else
                        HandleZoom(_zoomVelocity);
                }
                _zoomVelocity *= _zoomDeceleration;
            }
        }

        protected override void LateUpdate()
        {
            if (movementType == MovementType.Clamped)
            {
                base.LateUpdate();
                return;
            }

            if (_isZooming)
            {
                _isZooming = false;
                // Avoid dragging in next frame produces inaccurate velocity.
                UpdatePrevData();
            }
            else base.LateUpdate();
        }

        // Block scroll rect default dragging behaviour when multiple touches are detected.
        public override void OnDrag(PointerEventData eventData)
        {
            if (eventData.used)
                return;
            base.OnDrag(eventData);
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.used)
                return;
            base.OnBeginDrag(eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.used)
                return;
            base.OnEndDrag(eventData);
        }

        protected override void OnEnable()
        {
            if (_resetOnEnable && _isInitialized)
            {
                ResetContent();
            }
            ResetZoom();
            base.OnEnable();
        }

        #endregion API Methods

        #region Class Methods

        public void OnPinchStart(PinchEventData eventData)
        {
            if (!IsActive())
                return;
            ResetZoom();
            base.OnEndDrag(eventData.unchangedPointerData);
            _pinchStartPosition = eventData.MidPoint;
        }

        public void OnPinchEnd(PinchEventData eventData)
        {
            if (!IsActive())
                return;
            OnInitializePotentialDrag(eventData.targetPointerData);
            base.OnBeginDrag(eventData.unchangedPointerData);
        }

        public void OnPinchZoom(PinchEventData eventData)
        {
            if (!IsActive())
                return;
            float zoomValue = eventData.distanceDelta * this._pinchSensitivity;
            var localScale = content.localScale;
            if (zoomValue < 0f && localScale.x <= _lowerScale.x && localScale.y <= _lowerScale.y && localScale.z <= _lowerScale.z)
                return;
            if (zoomValue > 0f && localScale.x >= _upperScale.x && localScale.y >= _upperScale.y && localScale.z >= _upperScale.z)
                return;
            var localPosition = Vector2.zero;
            if (_lockPinchCenter && !RectTransformUtility.ScreenPointToLocalPointInRectangle(content, _pinchStartPosition, eventData.targetPointerData.pressEventCamera, out localPosition))
                return;
            if (!_lockPinchCenter && !RectTransformUtility.ScreenPointToLocalPointInRectangle(content, eventData.MidPoint, eventData.targetPointerData.pressEventCamera, out localPosition))
                return;
            _isZooming = true;
            _zoomVelocity = zoomValue;
            _zoomPositionDelta = localPosition;
            _updatePivot = true;
        }

        public override void OnScroll(PointerEventData eventData)
        {
            if (!IsActive())
                return;
            OnInitializePotentialDrag(eventData);
            float zoomValue = eventData.scrollDelta.y * scrollSensitivity;
            var localScale = content.localScale;
            if (zoomValue < 0f && localScale.x <= _lowerScale.x && localScale.y <= _lowerScale.y && localScale.z <= _lowerScale.z)
                return;
            if (zoomValue > 0f && localScale.x >= _upperScale.x && localScale.y >= _upperScale.y && localScale.z >= _upperScale.z)
                return;
            var localPosition = Vector2.zero;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(content, eventData.position, eventData.pressEventCamera, out localPosition))
                return;
            _isZooming = true;
            _zoomVelocity = zoomValue;
            _zoomPositionDelta = localPosition;
            _updatePivot = true;
        }

        private void HandleZoom(float zoomValue)
        {
            var localScale = content.localScale;
            var rect = content.rect;
            if (_updatePivot)
            {
                var anchorMin = content.anchorMin;
                var anchorMax = content.anchorMax;
                var localPosition = content.localPosition;
                // Set to new pivot before scaling.
                Vector2 pivotDelta = new Vector2(_zoomPositionDelta.x / rect.width, _zoomPositionDelta.y / rect.height);
                // Set fixed anchor to avoid incorrect calculation due to stretched anchors.
                content.anchorMin = new Vector2(0.5f, 0.5f);
                content.anchorMax = new Vector2(0.5f, 0.5f);
                UpdateBounds();
                SetContentPivotPosition(content.pivot + pivotDelta);
                // Apply position compensation due to pivot change.
                localPosition += new Vector3(_zoomPositionDelta.x * localScale.x, _zoomPositionDelta.y * localScale.y);
                // Reset to original anchors.
                content.anchorMin = anchorMin;
                content.anchorMax = anchorMax;
                // Apply final position.
                content.localPosition = localPosition;
            }
            // Set scale.
            Vector3 newScale = localScale + Vector3.one * zoomValue;
            newScale = new Vector3(Mathf.Clamp(newScale.x, _lowerScale.x, _upperScale.x),
                                   Mathf.Clamp(newScale.y, _lowerScale.y, _upperScale.y),
                                   Mathf.Clamp(newScale.z, _lowerScale.z, _upperScale.z));
            SetContentLocalScale(newScale);
            // Reset delta since zooming deceleration take place at the same pivot.
            _zoomPositionDelta = Vector2.zero;
            _updatePivot = false;
        }

        private void SetContentPivotPosition(Vector2 pivot)
        {
            var contentPivot = content.pivot;
            if (!horizontal)
                pivot.x = contentPivot.x;
            if (!vertical)
                pivot.y = contentPivot.y;
            if (pivot == contentPivot)
                return;
            content.pivot = pivot;
        }

        private void SetContentLocalScale(Vector3 newScale)
            => content.localScale = newScale;

        private void ResetZoom()
        {
            _zoomVelocity = 0f;
            _zoomPositionDelta = Vector2.zero;
            _updatePivot = false;
        }

        private void ResetContent()
        {
            if (!content)
                return;
            content.pivot = _initialPivot;
            content.anchoredPosition = _initialAnchored;
            content.localScale = _initialScale;
            UpdateBounds();
        }

        #endregion Class Methods
    }
}