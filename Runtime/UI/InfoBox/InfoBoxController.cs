using UnityEngine;
using UnityEngine.UI;
using Runtime.Common.Singleton;
using Cysharp.Threading.Tasks;
using TMPro;
using Runtime.Extensions;

namespace Runtime.UI
{
    public class InfoBoxController : MonoSingleton<InfoBoxController>
    {
        #region Members

        [SerializeField]
        private TextMeshProUGUI _infoBoxTitleText;
        [SerializeField]
        private TextMeshProUGUI _infoBoxDescriptionText;
        [SerializeField]
        private CanvasGroup _infoBoxCanvasGroup;
        [SerializeField]
        private Button _closeInfoBoxButton;
        [SerializeField]
        private Vector2 _infoBoxCenterBonusOffset;
        private RectTransform _infoBoxRectTransform;
        private bool _isShowing = false;

        #endregion Members

        #region API Methods

        protected override void Awake()
        {
            base.Awake();
            _isShowing = false;
            _infoBoxRectTransform = _infoBoxCanvasGroup.GetComponent<RectTransform>();
            _infoBoxCanvasGroup.SetActive(false);
            _closeInfoBoxButton.gameObject.SetActive(false);
            _closeInfoBoxButton.onClick.AddListener(OnClickCloseInfoBoxButton);
        }

        #endregion API Methods

        #region Class Methods

        public void ShowInfo(InfoBoxData infoBoxData)
        {
            if (!_isShowing)
            {
                _isShowing = true;
                _infoBoxTitleText.text = infoBoxData.Title;
                _infoBoxDescriptionText.text = infoBoxData.Description;
                RunSetUpInfoBoxTransformAsync(infoBoxData).Forget();
            }
        }

        private void OnClickCloseInfoBoxButton()
        {
            _isShowing = false;
            _infoBoxCanvasGroup.SetActive(false);
            _closeInfoBoxButton.gameObject.SetActive(false);
        }

        private async UniTask RunSetUpInfoBoxTransformAsync(InfoBoxData infoBoxData)
        {
            await UniTask.Yield(PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
            var infoBoxCenterOffset = Vector2.zero;
            var infoBoxSize = _infoBoxCanvasGroup.GetComponent<RectTransform>().sizeDelta;
            if (infoBoxData.IsPivotTop)
            {
                if (infoBoxData.IsPivotRight)
                    infoBoxCenterOffset = new Vector2(-(infoBoxSize.x * 0.5f + _infoBoxCenterBonusOffset.x), -(infoBoxSize.y * 0.5f + _infoBoxCenterBonusOffset.y));
                else
                    infoBoxCenterOffset = new Vector2((infoBoxSize.x * 0.5f + _infoBoxCenterBonusOffset.x), -(infoBoxSize.y * 0.5f + _infoBoxCenterBonusOffset.y));
            }
            else
            {
                if (infoBoxData.IsPivotRight)
                    infoBoxCenterOffset = new Vector2(-(infoBoxSize.x * 0.5f + _infoBoxCenterBonusOffset.x), (infoBoxSize.y * 0.5f + _infoBoxCenterBonusOffset.y));
                else
                    infoBoxCenterOffset = new Vector2((infoBoxSize.x * 0.5f + _infoBoxCenterBonusOffset.x), (infoBoxSize.y * 0.5f + _infoBoxCenterBonusOffset.y));
            }
            _infoBoxRectTransform.transform.position = infoBoxData.Position + infoBoxCenterOffset;
            _infoBoxCanvasGroup.SetActive(true);
            _closeInfoBoxButton.gameObject.SetActive(true);
        }

        #endregion Class Methods
    }
}