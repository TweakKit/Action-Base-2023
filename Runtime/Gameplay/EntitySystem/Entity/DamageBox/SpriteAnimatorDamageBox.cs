using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Runtime.Animation;
using Runtime.Manager.Pool;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace Runtime.Gameplay.EntitySystem
{
    public class SpriteAnimatorDamageBox : MonoBehaviour
    {
        #region Members

        [SerializeField]
        private int[] _triggeredColliderFrames;
        [SerializeField]
        private int[] _turnOffColliderFrames;
        [SerializeField]
        private SpriteAnimator _spriteAnimator;
        [SerializeField]
        private EntityTargetDetector _targetDetector;
        [SerializeField]
        private bool _notUseSelfDestroy;
        [SerializeField]
        private bool _showWarningBeforeDamage;
        [ShowIf(nameof(_showWarningBeforeDamage), true)]
        [SerializeField]
        private GameObject _warningIndicator;

        private DamageSource _damageSource;
        private float _damageFactorValue;
        private StatusEffectModel[] _damageStatusEffectModels;
        private CharacterModel _creatorModel;
        private Action _finishedAction;
        private bool _hasStartedDamaging;
        private List<IInteractable> _damagedInteractables;

        #endregion Members

        #region API Methods

        protected virtual void Update()
        {
            if (_hasStartedDamaging)
            {
                if (_triggeredColliderFrames.Contains(_spriteAnimator.FrameIndex))
                    _targetDetector.Enable();
                else if (_turnOffColliderFrames.Contains(_spriteAnimator.FrameIndex))
                    _targetDetector.Disable();
            }
        }

        private void OnEnable()
            => _spriteAnimator.ClearRenderer();

        #endregion API Methods

        #region Unity Event Callback Methods

        public virtual void StartDamageWithWarning()
        {
            _warningIndicator.SetActive(false);
            _hasStartedDamaging = true;
            PlayAnim();
        }

        protected virtual void PlayAnim()
        {
            _spriteAnimator.Play(playOneShot: true);
        }

        #endregion Unity Event Callback Methods

        #region Class Methods

        public virtual void Init(CharacterModel creatorModel, DamageSource damageSource, bool destroyWithCreator, float damageFactorValue,
                                 StatusEffectModel[] damageStatusEffectModels, Action finishedAction = null, float durationWarning = 1)
        {
            _hasStartedDamaging = false;
            _damagedInteractables = new();
            _damageSource = damageSource;
            _damageFactorValue = damageFactorValue;
            _damageStatusEffectModels = damageStatusEffectModels;
            _finishedAction = finishedAction;
            _creatorModel = creatorModel;
            _spriteAnimator.AnimationStoppedAction = null;
            _spriteAnimator.AnimationStoppedAction = OnStopAnim;
            _targetDetector.Disable();
            _targetDetector.Init(null, null, OnColliderEnterredAction);

            if (destroyWithCreator)
                _creatorModel.DeathEvent += OnCreatorDeath;

            if (_showWarningBeforeDamage)
            {
                _warningIndicator.GetComponentInChildren<DOTweenAnimation>().duration = durationWarning;
                _warningIndicator.SetActive(true);
            }
            else
                StartDamageWithoutWarning();
        }

        protected virtual void OnColliderEnterredAction(Collider2D collider)
        {
            var interactable = collider.GetComponent<IInteractable>();
            if (interactable != null && !interactable.Model.IsDead && interactable.Model.EntityType != _creatorModel.EntityType &&
                !_damagedInteractables.Contains(interactable) && interactable.IsCharacter)
            {
                var hitPoint = collider.ClosestPoint(transform.position);
                _damagedInteractables.Add(interactable);
                HitTarget(interactable, hitPoint);
            }
        }

        protected virtual void StartDamageWithoutWarning()
        {
            _hasStartedDamaging = true;
            PlayAnim();
        }

        protected virtual void HitTarget(IInteractable interactable, Vector2 hitPoint)
        {
            var damageInfo = _creatorModel.GetDamageInfo(_damageSource, _damageFactorValue, _damageStatusEffectModels, interactable.Model);
            var hitDirection = (hitPoint - (Vector2)transform.position).normalized;
            interactable.GetHit(damageInfo, hitDirection);
        }

        protected virtual void OnCreatorDeath(DamageSource damageSource)
            => PoolManager.Instance.Remove(gameObject);

        protected virtual void OnStopAnim()
        {
            _finishedAction?.Invoke();
            if (!_notUseSelfDestroy)
                PoolManager.Instance.Remove(gameObject);
        }

        #endregion Class Methods
    }
}