using UnityEngine;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class CharacterHUD : MonoBehaviour
    {
        #region Members

        [Header("Health")]
        [SerializeField]
        private Transform _healthBarSliderAnchor;
        [SerializeField]
        private SpriteRenderer _healthBarSliderSpriteRenderer;
        private Vector2 _currentHealthAnchorLocalScale;

        [Space] [Header("Shield")] [SerializeField]
        private GameObject _shieldContainer;
        [SerializeField]
        private Transform _shieldBarSliderAnchor;
        [SerializeField]
        private SpriteRenderer _shieldBarSliderSpriteRenderer;
        private Vector2 _currentShieldAnchorLocalScale;

        #endregion Members

        #region Class Methods

        public void Init(EntityType entityType, bool isHeroBoss)
        {
            var healthBarColor = Constant.GetCharacterHealthBarColor(entityType, isHeroBoss);
            _healthBarSliderSpriteRenderer.color = healthBarColor;
            _currentHealthAnchorLocalScale = _healthBarSliderAnchor.transform.localScale;
            _currentHealthAnchorLocalScale.x = 1;
            _healthBarSliderAnchor.transform.localScale = _currentHealthAnchorLocalScale;
            SetVisibility(false);

            if (_shieldContainer != null)
            {
                SetVisibilityShield(false);
                var shieldBarColor = Constant.GetCharacterShieldBarColor(entityType, isHeroBoss);
                _shieldBarSliderSpriteRenderer.color = shieldBarColor;
                _currentShieldAnchorLocalScale = _shieldBarSliderAnchor.transform.localScale;
                _currentShieldAnchorLocalScale.x = 0;
                _shieldBarSliderAnchor.transform.localScale = _currentShieldAnchorLocalScale;
            }
        }

        public void UpdateHealthBar(float currentHP, float maxHP)
        {
            _currentHealthAnchorLocalScale.x = Mathf.Max(currentHP / maxHP, 0);
            _healthBarSliderAnchor.transform.localScale = _currentHealthAnchorLocalScale;
            if (currentHP < maxHP && !gameObject.activeSelf)
                SetVisibility(true);
            else if (currentHP >= maxHP && gameObject.activeSelf)
                SetVisibility(false);
        }
        
        public void UpdateShieldBar(float currentDefense, float maxDefense)
        {
            if (_shieldContainer != null && _shieldBarSliderAnchor != null && _shieldBarSliderSpriteRenderer != null)
            {
                if (maxDefense <= 0 || currentDefense <= 0)
                {
                    SetVisibilityShield(false);
                    return;
                }

                SetVisibility(true);
                SetVisibilityShield(true);
                _currentShieldAnchorLocalScale.x = Mathf.Max(currentDefense / maxDefense, 0);
                _shieldBarSliderAnchor.transform.localScale = _currentShieldAnchorLocalScale;
            }
        }

        public void SetVisibility(bool isVisible)
            => gameObject.SetActive(isVisible);
        
        public void SetVisibilityShield(bool isVisible)
            => _shieldContainer.SetActive(isVisible);

        #endregion Class Methods
    }
}