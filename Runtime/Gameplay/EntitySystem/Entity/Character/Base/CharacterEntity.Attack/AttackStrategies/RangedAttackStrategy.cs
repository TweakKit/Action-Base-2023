using System.Threading;
using UnityEngine;
using Runtime.Definition;
using Runtime.Extensions;
using Runtime.Gameplay.Manager;
using Cysharp.Threading.Tasks;

namespace Runtime.Gameplay.EntitySystem
{
    public class RangedAttackStrategy : AttackStrategy
    {
        #region Members

        protected Transform projectileSpawnPointTransform;
        protected const string PROJECTILE_POSTFIX_NAME = "_projectile";
        protected const string PROJECTILE_SPAWN_POINT_TRANSFORM_NAME = "projectile_spawn_point";

        #endregion Members

        #region Properties

        protected float ProjectileMoveRange
        {
            get
            {
                var attackRange = ownerCharacterModel.GetTotalStatValue(StatType.AttackRange);
                return attackRange + attackRange * 0.5f;
            }
        }

        #endregion Properties

        #region Class Methods

        public override void Init(CharacterModel ownerCharacterModel, Transform ownerCharacterTransform)
        {
            base.Init(ownerCharacterModel, ownerCharacterTransform);
            projectileSpawnPointTransform = ownerCharacterTransform.FindChildTransform(PROJECTILE_SPAWN_POINT_TRANSFORM_NAME);
        }

        protected override float GetOriginalNormalAttackAnimationTime()
        {
            var characterWeaponAnimationType = CharacterWeaponAnimationType.RangedAttack;
            return characterWeaponActionPlayer.GetAnimationTime(characterWeaponAnimationType);
        }

        protected override async UniTask TriggerAttackAsync(CancellationToken cancellationToken)
        {
            var isPlayingAnimation = true;
            var characterWeaponPlayedData = new CharacterWeaponPlayedData
            (
                animationType: CharacterWeaponAnimationType.RangedAttack,
                speedMultiplier: normalAttackAnimationSpeedMultiplier,
                operatedPointTriggeredCallbackAction: () =>
                {
                    TriggerAttackPointOperation();
                    FireProjectileTowardsTarget(ownerCharacterModel.CurrentAttackedTarget, cancellationToken);
                },
                endActionCallbackAction: () => isPlayingAnimation = false
            );
            PlaySpecialSoundAtk(cancellationToken);
            characterWeaponActionPlayer.Play(characterWeaponPlayedData);
            await UniTask.WaitUntil(() => !isPlayingAnimation, cancellationToken: cancellationToken);
        }

        protected virtual void FireProjectileTowardsTarget(IInteractable interactableTarget, CancellationToken cancellationToken)
        {
            if (interactableTarget != null && !interactableTarget.IsDead)
                SpawnProjectileAsync(interactableTarget, cancellationToken).Forget();
        }

        protected virtual async UniTaskVoid SpawnProjectileAsync(IInteractable interactableTarget, CancellationToken cancellationToken)
        {
            Vector2 projectilePosition = projectileSpawnPointTransform.position;
            var projectileDirection = GetProjectileDirection(interactableTarget.CenterPosition, projectilePosition);
            var projectileId = ownerCharacterModel.EntityVisualId + PROJECTILE_POSTFIX_NAME;
            var projectileGameObject = await EntitiesManager.Instance.CreateProjectileAsync(projectileId, projectilePosition, ownerCharacterModel, cancellationToken);
            var projectile = projectileGameObject.GetComponent<Projectile>();
            projectile.Init(ProjectileMoveRange, 1, DamageSource.FromAttack, null, projectileDirection, interactableTarget, null);
        }

        protected Vector2 GetProjectileDirection(Vector2 targetPosition, Vector2 sourcePosition)
        {
            var projectileDirection = (targetPosition - sourcePosition).normalized;
            if (projectileDirection == Vector2.zero)
                return ownerCharacterModel.FaceDirection;
            else
                return projectileDirection;
        }

        #endregion Class Methods
    }
}