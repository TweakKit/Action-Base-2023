using System;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    /// <summary>
    /// This entity simulates the behavior of a returned projectile: Fly to target and go back.<br/>
    /// </summary>
    [DisallowMultipleComponent]
    public class ReturnedProjectile : Projectile
    {
        #region Members

        private bool _hasReturnedBack;

        #endregion Members

        #region API Methods

        #endregion API Methods

        protected override void Update()
        {
            transform.position = transform.position + moveSpeed * moveDirection.normalized * Time.deltaTime;
            CheckRotateProjectile();
            if (!_hasReturnedBack)
            {
                if (Vector2.SqrMagnitude(originalPosition - Position) > moveDistance * moveDistance)
                    ReturnBack();
            }
            else
            {
                float sqrDistance = moveSpeed * Time.deltaTime * moveSpeed * Time.deltaTime;
                if (Vector2.SqrMagnitude(originalPosition - Position) <= sqrDistance)
                    CompleteMovement();
            }
        }

        #region Class Methods

        public override void Init(float moveDistance, float damageFactorValue, DamageSource damageSource, StatusEffectModel[] damageStatusEffectModels,
                                  Vector2 moveDirection, IInteractable interactableTarget, Action completedMovingAction, ProjectileFeatureData projectileFeatureData = null)
        {
            base.Init(moveDistance, damageFactorValue, damageSource, damageStatusEffectModels, moveDirection, interactableTarget, completedMovingAction, projectileFeatureData);
            _hasReturnedBack = false;
        }

        private void ReturnBack()
        {
            _hasReturnedBack = true;
            moveDirection = -moveDirection;
        }

        #endregion Class Methods
    }
}