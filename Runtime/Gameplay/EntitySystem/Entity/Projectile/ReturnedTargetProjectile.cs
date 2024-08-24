using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    /// <summary>
    /// This entity simulates the behavior of a returned projectile: Fly to target and go back to creator.<br/>
    /// </summary>
    [DisallowMultipleComponent]
    public class ReturnedTargetProjectile : Projectile
    {
        #region Members

        private bool _hasReturnedBack;
        private Action<float> _onGetHitCallback;
        private Tween _tweenMove;
        private Dictionary<IInteractable, float> _cacheListTargetHadHit = new();
        private float _timeStepTriggerAttack;
        private float _dynamicMoveSpeed;

        #endregion Members

        #region API Methods

        #endregion API Methods

        protected override void Update()
        {
            transform.position = transform.position + moveDirection.normalized * (_dynamicMoveSpeed * Time.deltaTime);
            CheckRotateProjectile();
            if (!_hasReturnedBack)
            {
                if (Vector2.SqrMagnitude(originalPosition - Position) > moveDistance * moveDistance)
                    ReturnBack();
            }
            else
            {
                moveDirection = (CreatorModel.CenterPosition - (Vector2)Position).normalized;
                float sqrDistance = _dynamicMoveSpeed * Time.deltaTime * _dynamicMoveSpeed * Time.deltaTime;
                if (Vector2.SqrMagnitude(CreatorModel.CenterPosition - (Vector2)Position) <= sqrDistance)
                    CompleteMovement();
            }
        }

        protected override void OnTriggerEnter2D(Collider2D collider)
        {
            if (_timeStepTriggerAttack <= 0.0f)
            {
                base.OnTriggerEnter2D(collider);
            }
        }

        private void OnTriggerStay2D(Collider2D collider)
        {
            if (_timeStepTriggerAttack > 0.0f)
            {
                var interactable = collider.GetComponent<IInteractable>();
                if (interactable != null && interactable.CanGetAffected(CreatorModel, damageSource))
                {
                    if (!_cacheListTargetHadHit.ContainsKey(interactable))
                    {
                        _cacheListTargetHadHit.Add(interactable, 0.0f);
                        if (isSingleTarget)
                        {
                            if (!hasHitTarget)
                            {
                                hasHitTarget = true;
                                DamageTarget(interactable);
                                CompleteMovement();
                            }
                        }
                        else DamageTarget(interactable);
                    }
                    else
                    {
                        if (_cacheListTargetHadHit[interactable] < _timeStepTriggerAttack)
                        {
                            _cacheListTargetHadHit[interactable] += Time.deltaTime;
                        }
                        else
                        {
                            _cacheListTargetHadHit[interactable] = 0;
                            DamageTarget(interactable);
                        }
                    }
                }
            }
        }

        protected override void DamageTarget(IInteractable interactable)
        {
            DamageInfo damageInfo = null;
            var damageDirection = (interactable.Model.Position - (Vector2)transform.position).normalized;
            if (projectileFeatureData != null)
            {
                if (projectileFeatureData.projectileFeatureType == ProjectileFeatureType.CauseCritOnBurned)
                    damageInfo = GetDamageInfoWithCauseCritOnBurned(interactable);
                else if (projectileFeatureData.projectileFeatureType == ProjectileFeatureType.CauseMoreDamageOnBleed)
                    damageInfo = GetDamageInfoWithCauseMoreDamageOnBleed(interactable);
                else if (projectileFeatureData.projectileFeatureType == ProjectileFeatureType.CauseFreezeOnChill)
                    damageInfo = GetDamageInfoWithFreezeOnChill(interactable);
                else if (projectileFeatureData.projectileFeatureType == ProjectileFeatureType.CauseMoreDamageOnPoison)
                    damageInfo = GetDamageInfoWithCauseMoreDamageOnPoison(interactable);
                else if (projectileFeatureData.projectileFeatureType == ProjectileFeatureType.CauseCastOnCrit)
                {
                    var projectileCauseCastOnCritFeature = projectileFeatureData.projectileFeature as ProjectileCauseCastOnCritFeature;
                    var decreaseDamePercentStep = projectileCauseCastOnCritFeature.decreaseDamePercentStep;
                    var ratioDamage = 1 - decreaseDamePercentStep * projectileCauseCastOnCritFeature.countTargetGetHit;
                    if (ratioDamage < 0) ratioDamage = 0;
                    var finalDamageFactorValue = damageFactorValue * ratioDamage;
                    damageInfo = CreatorModel.GetDamageInfo(damageSource, finalDamageFactorValue, damageStatusEffectModels, interactable.Model);
                    if (damageInfo.damageSource == DamageSource.FromCritAttack || damageInfo.damageSource == DamageSource.FromCritSkill)
                        projectileCauseCastOnCritFeature.projectileFeatureCallBack?.Invoke();
                    projectileCauseCastOnCritFeature.countTargetGetHit++;
                }
            }
            else damageInfo = CreatorModel.GetDamageInfo(damageSource, damageFactorValue, damageStatusEffectModels, interactable.Model);
            interactable.GetHit(damageInfo, damageDirection);
            _onGetHitCallback?.Invoke(damageInfo.damage);
        }

        #region Class Methods

        public void Init(float moveDistance, float damageFactorValue, DamageSource damageSource, StatusEffectModel[] damageStatusEffectModels,
                                  Vector2 moveDirection, IInteractable interactableTarget, Action completedMovingAction, ProjectileFeatureData projectileFeatureData = null, Action<float> onGetHitCallback = null, float timeStepTriggerAttack = 0.0f)
        {
            base.Init(moveDistance, damageFactorValue, damageSource, damageStatusEffectModels, moveDirection, interactableTarget, completedMovingAction, projectileFeatureData);
            _hasReturnedBack = false;
            _onGetHitCallback = onGetHitCallback;
            _cacheListTargetHadHit = new();
            _timeStepTriggerAttack = timeStepTriggerAttack;
            _dynamicMoveSpeed = moveSpeed;
            var startMoveSpeed = moveSpeed;
            _tweenMove.Kill();
            _tweenMove = DOVirtual.Float(startMoveSpeed * 2, startMoveSpeed, 1, (x) => _dynamicMoveSpeed = x);
        }

        private void ReturnBack()
        {
            _hasReturnedBack = true;
            var startMoveSpeed = moveSpeed;
            _tweenMove.Kill();
            _tweenMove = DOVirtual.Float(startMoveSpeed * 0.5f, startMoveSpeed, 1, (x) => _dynamicMoveSpeed = x);
        }

        public override void Dispose()
        {
            _tweenMove.Kill();
            base.Dispose();
        }

        #endregion Class Methods
    }
}