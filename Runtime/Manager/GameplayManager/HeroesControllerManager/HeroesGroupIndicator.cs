using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Runtime.Gameplay.Manager
{
    public class HeroesGroupIndicator : MonoBehaviour
    {
        #region Members

        [SerializeField]
        private Transform _visualTransform;
        [SerializeField]
        private float _visualAnimationScaleMinusValue;
        [SerializeField]
        private float _visualAnimationScaleSpeedValue;
        [SerializeField]
        private float _visualSpriteSizeRate;
        private bool _isAnimatingVisual;
        private CancellationTokenSource _visualAnimationCancellationTokenSource;

        #endregion Members

        #region API Methods

        private void Start()
        {
            _isAnimatingVisual = false;
            _visualAnimationCancellationTokenSource = new CancellationTokenSource();
        }

        private void OnDestroy()
        {
            _visualAnimationCancellationTokenSource.Cancel();
            _visualAnimationCancellationTokenSource.Dispose();
        }

        #endregion API Methods

        #region Class Methods

        public void Show(float heroesGroupExploitRadius, Vector2 position)
        {
            _visualTransform.localScale = Vector3.one * (heroesGroupExploitRadius * 2 / _visualSpriteSizeRate);
            _visualTransform.position = position;
            if (!_isAnimatingVisual)
                PlayVisualAnimationAsync(_visualAnimationCancellationTokenSource.Token).Forget();
        }

        public void Hide()
        {
            _isAnimatingVisual = false;
            SetVisibility(false);
        }

        private async UniTask PlayVisualAnimationAsync(CancellationToken cancellationToken)
        {
            _isAnimatingVisual = true;
            SetVisibility(true);
            Vector2 originalScaleValue = _visualTransform.localScale;
            Vector2 animatedScaleValue = originalScaleValue - Vector2.one * _visualAnimationScaleMinusValue;
            var interpolationTime = 0.0f;
            while (interpolationTime < 1.0f)
            {
                interpolationTime += Time.deltaTime * _visualAnimationScaleSpeedValue;
                var parapolaInterpolationValue = 4.0f * (-interpolationTime * interpolationTime + interpolationTime);
                var scaleValue = Vector2.Lerp(originalScaleValue, animatedScaleValue, parapolaInterpolationValue);
                _visualTransform.localScale = scaleValue;
                await UniTask.Yield(cancellationToken: cancellationToken);
            }
            _isAnimatingVisual = false;
            SetVisibility(false);
        }

        private void SetVisibility(bool isVisible)
            => _visualTransform.gameObject.SetActive(isVisible);

        #endregion Class Methods
    }
}