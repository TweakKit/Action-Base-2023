using UnityEngine;
using UnityEngine.EventSystems;

namespace Runtime.UI
{
    public interface IPinchStartHandler
    {
        #region Interface Methods

        void OnPinchStart(PinchEventData eventData);

        #endregion Interface Methods
    }

    public interface IPinchEndHandler
    {
        #region Interface Methods

        void OnPinchEnd(PinchEventData eventData);

        #endregion Interface Methods
    }

    public interface IPinchZoomHandler
    {
        #region Interface Methods

        void OnPinchZoom(PinchEventData eventData);

        #endregion Interface Methods
    }

    public class PinchEventData
    {
        #region Members

        public PointerEventData targetPointerData;
        public PointerEventData unchangedPointerData;
        public float distanceDelta;

        #endregion Members

        #region Properties

        public Vector2 MidPoint
        {
            get
            {
                return (targetPointerData.position + unchangedPointerData.position) / 2f;
            }
        }

        #endregion Properties

        #region Class Methods

        public PinchEventData(PointerEventData targetPointerData, PointerEventData unchangedPointerData, float distanceDelta = 0.00f)
        {
            this.targetPointerData = targetPointerData;
            this.unchangedPointerData = unchangedPointerData;
            this.distanceDelta = distanceDelta;
        }

        #endregion Class Methods
    }
}