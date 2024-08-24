using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Runtime.Definition;
using Runtime.Gameplay.Manager;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class RangedPenetrateProjectileAttackStrategy : RangedAttackStrategy
    {
        private CharacterWeaponSpriteAnimationActionPlayer _characterWeaponSpriteAnimationActionPlayer;

        private int _damageSpearsIndex;
        private bool _isTargetPoisoned;
        private string _projectileId;
        private float _delaySpawnSpear;
        private float[] _damageSpears;

        private const float DELAY_SPAWN_NORMAL_SPEAR = 0.3f;
        private const string PROJECTILE_SKILL1 = "_projectile_skill1";
        private const string ATTACK_ANIMATION = "Attack";
        private const string NEW_ATTACK_ANIMATION = "Skill_1";

        public override void Init(CharacterModel ownerCharacterModel, Transform ownerCharacterTransform)
        {
            base.Init(ownerCharacterModel, ownerCharacterTransform);

            GetDataFromSkillModels();
            ownerCharacterModel.DeathEvent += (DamageSource _) => {

                _characterWeaponSpriteAnimationActionPlayer.SetNewAnimationName(
                    CharacterWeaponAnimationType.RangedAttack, ATTACK_ANIMATION
                );
            };

            _characterWeaponSpriteAnimationActionPlayer =
                ownerCharacterTransform.GetComponentInChildren<CharacterWeaponSpriteAnimationActionPlayer>();
        }

        private void GetDataFromSkillModels()
        {
            if (ownerCharacterModel is not HeroModel heroModel)
            {
                return;
            }

            var skillModels = heroModel.SkillModels;

            foreach (var skillModel in skillModels)
            {
                if (skillModel is PenetratingThrowSkillModel penetratingThrowSkillModel)
                {
                    _delaySpawnSpear = penetratingThrowSkillModel.DelaySpawnSpear;
                    _damageSpears = penetratingThrowSkillModel.DamageSpearsByAttackPercent;

                    break;
                }
            }
        }

        protected override void FireProjectileTowardsTarget(IInteractable interactableTarget, CancellationToken cancellationToken)
        {
            if (interactableTarget == null || interactableTarget.IsDead)
            {
                return;
            }

            SpawnProjectilesAsync(interactableTarget, cancellationToken).Forget();
        }

        private async UniTaskVoid SpawnProjectilesAsync(IInteractable interactableTarget, CancellationToken cancellationToken)
        {
            _damageSpearsIndex = 0;
            _isTargetPoisoned = IsTargetPoisoned();
            _projectileId = ownerCharacterModel.EntityVisualId;

            if (_isTargetPoisoned)
            {
                _projectileId += PROJECTILE_SKILL1;

                _characterWeaponSpriteAnimationActionPlayer.SetNewAnimationName(
                    CharacterWeaponAnimationType.RangedAttack, NEW_ATTACK_ANIMATION
                );
            }
            else
            {
                _projectileId += PROJECTILE_POSTFIX_NAME;

                _characterWeaponSpriteAnimationActionPlayer.SetNewAnimationName(
                    CharacterWeaponAnimationType.RangedAttack, ATTACK_ANIMATION
                );
            }

            for (int i = 0; i < _damageSpears.Length; i++)
            {
                SpawnProjectileAsync(interactableTarget, cancellationToken).Forget();
                await UniTask.Delay(
                      TimeSpan.FromSeconds(_isTargetPoisoned ? _delaySpawnSpear : DELAY_SPAWN_NORMAL_SPEAR)
                    , cancellationToken: cancellationToken
                );
            }
        }

        protected override async UniTaskVoid SpawnProjectileAsync(IInteractable interactableTarget, CancellationToken cancellationToken)
        {
            if (ownerCharacterModel.EntityType != EntityType.Hero)
            {
                return;
            }

            var projectilePosition = projectileSpawnPointTransform.position;
            var projectileDirection = GetProjectileDirection(interactableTarget.CenterPosition, projectilePosition);

            var projectileGameObject = await EntitiesManager.Instance.CreateProjectileAsync(
                  _projectileId
                , projectilePosition
                , ownerCharacterModel
                , cancellationToken
            );
            var projectile = projectileGameObject.GetComponent<ProjectileCanPenetrate>();

            projectile.SetSingleTarget(_isTargetPoisoned == false);

            projectile.Init(
                  ProjectileMoveRange
                , _damageSpears[_damageSpearsIndex]
                , DamageSource.FromAttack
                , null
                , projectileDirection
                , interactableTarget
                , null
            );

            _damageSpearsIndex++;
        }

        private bool IsTargetPoisoned()
        {
            if (ownerCharacterModel.CurrentAttackedTarget.Model is not CharacterModel targetModel)
            {
                return false;
            }

            return targetModel.CheckContainStatusEffectInStack(StatusEffectType.PoisonAttack);
        }
    }
}