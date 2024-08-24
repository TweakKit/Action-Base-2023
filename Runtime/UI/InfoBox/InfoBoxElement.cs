using UnityEngine;

namespace Runtime.UI
{
    public class InfoBoxElement : MonoBehaviour
    {
        #region Members

        private RectTransform _rectTransform;

        #endregion Members

        #region API Methods

        private void Awake()
            => _rectTransform = gameObject.GetComponent<RectTransform>();

        #endregion API Methods

        #region Class Methods

        public void Show(InfoBoxData infoBoxData)
        {
            var isTopPivot = _rectTransform.position.y > Screen.height * 0.5f;
            var isRightPivot = _rectTransform.position.x > Screen.width * 0.5f;
            infoBoxData.SetPivot(isTopPivot, isRightPivot);
            infoBoxData.SetPosition(_rectTransform.position);
            InfoBoxController.Instance.ShowInfo(infoBoxData);
        }

        #endregion Class Methods
    }
}