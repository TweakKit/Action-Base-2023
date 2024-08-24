using System.Threading;
using UnityEngine;
using Runtime.Definition;
using Runtime.Extensions;
using Runtime.Gameplay.Manager;
using Cysharp.Threading.Tasks;

namespace Runtime.Gameplay.EntitySystem
{
    public class RangedBurnEffectAttackStrategy : RangedAttackStrategy
    {
        #region Members

        private StatusEffectModel[] _damageStatusEffectModels;
        private readonly float _burnDamageByAttackPercent = 0.3f;
        private readonly float _burnDamageDuration = 3.0f;

        #endregion Members

        #region Properties

        private float ProjectileMoveRange
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

            var statusEffectModels = new StatusEffectModel[1];
            BurnAttackStatusEffectModel burnAttackStatusEffectModel = new BurnAttackStatusEffectModel(_burnDamageByAttackPercent, _burnDamageDuration);
            statusEffectModels[0] = burnAttackStatusEffectModel;
            _damageStatusEffectModels = statusEffectModels;
        }

        protected override async UniTaskVoid SpawnProjectileAsync(IInteractable interactableTarget, CancellationToken cancellationToken)
        {
            Vector2 projectilePosition = projectileSpawnPointTransform.position;
            var projectileDirection = GetProjectileDirection(interactableTarget.CenterPosition, projectilePosition);
            var projectileId = ownerCharacterModel.EntityVisualId + PROJECTILE_POSTFIX_NAME;
            var projectileGameObject = await EntitiesManager.Instance.CreateProjectileAsync(projectileId, projectilePosition, ownerCharacterModel, cancellationToken);
            var projectile = projectileGameObject.GetComponent<Projectile>();
            projectile.Init(ProjectileMoveRange, 1, DamageSource.FromAttack, _damageStatusEffectModels, projectileDirection, interactableTarget, null);
        }

        #endregion Class Methods
    }
}