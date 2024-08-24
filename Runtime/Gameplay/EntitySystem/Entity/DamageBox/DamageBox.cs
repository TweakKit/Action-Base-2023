using System.Collections.Generic;
using UnityEngine;
using Runtime.Manager.Pool;

namespace Runtime.Gameplay.EntitySystem
{
    public class DamageBox : MonoBehaviour
    {
        #region Members

        [SerializeField]
        private EntityTargetDetector _targetDetector;
        private DamageSource _damageSource;
        private float _damageFactorValue;
        private StatusEffectModel[] _damageStatusEffectModels;
        private CharacterModel _creatorModel;
        private List<IInteractable> _damagedInteractables;

        #endregion Members

        #region API Methods

        private void OnEnable()
            => _targetDetector.Disable();

        #endregion API Methods

        #region Class Methods

        public virtual void Init(CharacterModel creatorModel, DamageSource damageSource, bool destroyWithCreator,
                                 float damageFactorValue, StatusEffectModel[] damageStatusEffectModels)
        {
            _damagedInteractables = new();
            _damageSource = damageSource;
            _damageFactorValue = damageFactorValue;
            _damageStatusEffectModels = damageStatusEffectModels;
            _creatorModel = creatorModel;

            if (destroyWithCreator)
                _creatorModel.DeathEvent += OnCreatorDeath;

            _targetDetector.Init(null, null, OnColliderEnterredAction);
            _targetDetector.Enable();
        }

        protected virtual void OnColliderEnterredAction(Collider2D collider)
        {
            var interactable = collider.GetComponent<IInteractable>();
            if (interactable != null && !interactable.Model.IsDead && !_damagedInteractables.Contains(interactable))
            {
                var hitPoint = collider.ClosestPoint(transform.position);
                _damagedInteractables.Add(interactable);
                HitTarget(interactable, hitPoint);
            }
        }

        protected virtual void HitTarget(IInteractable interactable, Vector2 hitPoint)
        {
            var damageInfo = _creatorModel.GetDamageInfo(_damageSource, _damageFactorValue, _damageStatusEffectModels, interactable.Model);
            var hitDirection = (hitPoint - (Vector2)transform.position).normalized;
            interactable.GetHit(damageInfo, hitDirection);
        }

        protected virtual void OnCreatorDeath(DamageSource damageSource)
            => PoolManager.Instance.Remove(gameObject);

        #endregion Class Methods
    }
}