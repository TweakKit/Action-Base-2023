using System;
using UnityEngine;
using Runtime.Utilities;
using Sirenix.OdinInspector;

namespace Runtime.Gameplay.EntitySystem
{
    /// <summary>
    /// This entity simulates the behavior of a curved projectile.<br/>
    /// </summary>
    public class CurvedProjectile : Projectile
    {
        #region Members

        [SerializeField]
        protected bool isDynamicFlyHeight = false;
        [ShowIf(nameof(isDynamicFlyHeight), true)]
        [SerializeField]
        protected float flyHeight;
        [SerializeField]
        protected float flyDuration;
        protected Vector2 targetPosition;
        protected Vector2 middlePosition;
        protected float currentFlyTime;

        #endregion Members

        #region API Methods

        protected override void Update()
        {
            if (currentFlyTime < flyDuration)
            {
                currentFlyTime += Time.deltaTime;
                var moveToPosition = MathUtility.Bezier(originalPosition, middlePosition, targetPosition, Mathf.Clamp01(currentFlyTime / flyDuration));
                transform.position = moveToPosition;

            }
            else CompleteMovement();
            CheckRotateProjectile();
        }

        protected override void OnTriggerEnter2D(Collider2D collider) { }

        #endregion API Methods

        #region Class Methods

        public override void Init(float moveDistance, float damageFactorValue, DamageSource damageSource, StatusEffectModel[] damageStatusEffectModels,
                                  Vector2 moveDirection, IInteractable interactableTarget, Action completedMovingAction, ProjectileFeatureData projectileFeatureData = null)
        {
            base.Init(moveDistance, damageFactorValue, damageSource, damageStatusEffectModels, moveDirection, interactableTarget, completedMovingAction, projectileFeatureData);     
            targetPosition = interactableTarget != null ? interactableTarget.Position : (Vector2)transform.position + moveDirection * moveDistance;
            if (isDynamicFlyHeight)
                flyHeight = Vector2.Distance(originalPosition, targetPosition) * 2;
            middlePosition = new Vector2((targetPosition.x + originalPosition.x) / 2, (targetPosition.y + originalPosition.y) / 2 + flyHeight);
            currentFlyTime = 0.0f;
        }

        public override void Init(float moveDistance, float damageFactorValue, DamageSource damageSource, StatusEffectModel[] damageStatusEffectModels,
                                 Vector2 moveDirection, Vector2 positionTarget, Action completedMovingAction, ProjectileFeatureData projectileFeatureData = null)
        {
            base.Init(moveDistance, damageFactorValue, damageSource, damageStatusEffectModels, moveDirection, positionTarget, completedMovingAction, projectileFeatureData);
            targetPosition = positionTarget;
            if (isDynamicFlyHeight)
                flyHeight = Vector2.Distance(originalPosition, targetPosition) * 2;
            middlePosition = new Vector2((targetPosition.x + originalPosition.x) / 2, (targetPosition.y + originalPosition.y) / 2 + flyHeight);
            currentFlyTime = 0.0f;
        }

        #endregion Class Methods
    }
}