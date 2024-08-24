using UnityEngine;
using TMPro;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace Runtime.Common.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class NumberCounter : MonoBehaviour
    {
        #region Members

        [SerializeField] private int _countFPS = 30;
        [SerializeField] private float _duration = 1f;
        [SerializeField] private float _strongScale = 1.2f;
        [SerializeField] private Color _colorOnCount;
        private TMP_Text _text;
        private CancellationTokenSource _cancallationTokenSource;
        private Vector3 _originalScale;
        private Color _originalColor;

        #endregion Members

        #region API Methods

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
            _duration = 0.7f;
            _strongScale = 1.5f;
            _cancallationTokenSource = new();
            _originalScale = transform.localScale;
            _originalColor = _text.color;
        }

        private void OnDestroy()
        {
            _cancallationTokenSource.Cancel();
            _cancallationTokenSource.Dispose();
        }

        #endregion API Methods

        #region Class Methods

        public void UpdateText(long lastValue, long newValue, string firstString = "", string lastString = "", bool isScale = false, bool isColor = false)
        {
            if (lastValue == newValue)
            {
                _text.SetText(firstString + lastValue.ToString() + lastString);
                return;
            }

            if (_cancallationTokenSource != null)
            {
                _cancallationTokenSource.Cancel();
                _cancallationTokenSource.Dispose();
                _cancallationTokenSource = new();
            }

            ResetHighLight();
            if (isScale)
            {
                HighLightScale();
            }
            if (isColor)
            {
                HighLightColor();
            }

            CountTextAsync(lastValue, newValue, firstString, lastString).Forget();
        }

        private async UniTask CountTextAsync(long lastValue, long newValue, string firstString = "", string lastString = "")
        {
            var wait = (int)(1000f / _countFPS);
            int stepAmount;
            long difference = newValue - lastValue;

            if (difference < 0)
                stepAmount = Mathf.FloorToInt((difference) / (_countFPS * _duration));
            else
                stepAmount = Mathf.CeilToInt((difference) / (_countFPS * _duration));

            for (int i = 0; i < Mathf.Abs(difference); i++)
            {
                lastValue += stepAmount;
                if ((lastValue > newValue && difference > 0) || (lastValue < newValue && difference < 0))
                    lastValue = newValue;

                _text.SetText(firstString + lastValue.ToString() + lastString);
                await UniTask.Delay(wait, cancellationToken: _cancallationTokenSource.Token, ignoreTimeScale: true);
            }
        }

        public void HighLightScale()
        {
            transform.DOScale(_originalScale * _strongScale, _duration).OnComplete(() => {
                transform.DOScale(_originalScale, _duration / 2);
            });
        }

        public void HighLightColor()
        {
            _text.DOColor(_colorOnCount, _duration).OnComplete(() => {
                _text.DOColor(_originalColor, _duration / 2);
            });
        }

        public void ResetHighLight()
        {
            transform.DOKill();
            transform.localScale = _originalScale;
            _text.color = _originalColor;
        }

        #endregion Class Methods
    }
}