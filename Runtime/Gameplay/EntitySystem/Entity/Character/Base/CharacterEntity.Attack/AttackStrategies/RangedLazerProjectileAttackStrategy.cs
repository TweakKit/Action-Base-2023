using System;
using System.Threading;
using Runtime.Manager.Pool;
using Cysharp.Threading.Tasks;
using Runtime.Audio;
using Runtime.Extensions;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class RangedLazerProjectileAttackStrategy : RangedAttackStrategy
    {
        #region Members

        private const string VFX_PROJECTILE_NAME = "lazer_vfx";
        private const string LAZER_IMPACT_EFFECT_NAME = "lazer_vfx_impact";
        private readonly float _widthDamage = 1;

        #endregion Members
        
        #region Class Methods

        protected override async UniTaskVoid SpawnProjectileAsync(IInteractable interactableTarget, CancellationToken cancellationToken)
        {
            var projectileObj = await PoolManager.Instance.Get(VFX_PROJECTILE_NAME, cancellationToken);
            var position = projectileSpawnPointTransform.position;
            projectileObj.transform.position = position;
            var distanceDamage = Vector2.Distance(interactableTarget.CenterPosition, position);
            var lazerBeam = projectileObj.GetComponent<ScalableVFX>();
            if (lazerBeam)
                lazerBeam.Scale(distanceDamage, _widthDamage);
            var directionLazer = (interactableTarget.CenterPosition - (Vector2)position).normalized;
            projectileObj.transform.rotation = directionLazer.ToQuaternion();
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f), ignoreTimeScale: true);
            TriggerDamageLazerSingleTarget(default);
        }

         private void TriggerDamageLazerSingleTarget(CancellationToken cancellationToken)
        {
            var interactable = ownerCharacterModel.CurrentAttackedTarget;
            if (interactable != null && interactable.CanGetAffected(ownerCharacterModel, DamageSource.FromAttack))
            {
                var damageInfo = ownerCharacterModel.GetDamageInfo(DamageSource.FromSkill, 1, null, interactable.Model);
                var damageDirection = (interactable.Position - ownerCharacterModel.Position).normalized;
                interactable.GetHit(damageInfo, damageDirection);
                SpawnImpactAsync(interactable.CenterPosition, cancellationToken).Forget();
            }
        }

        /// <summary>
        ///  var hitBoxCenterPoint = (Vector2)position + directionLazer * distanceDamage / 2;
        ///  var angle = projectileObj.transform.eulerAngles.z;
        ///  var size = new Vector2(_widthDamage, distanceDamage);
        /// </summary>
        /// <param name="point"></param>
        /// <param name="size"></param>
        /// <param name="angle"></param>
        /// <param name="cancellationToken"></param>
        private void TriggerDamageLazerBox(Vector2 point, Vector2 size, float angle, CancellationToken cancellationToken)
        {
            var colliders = Physics2D.OverlapBoxAll(point, size, angle);
            foreach (var collider in colliders)
            {
                var interactable = collider.GetComponent<IInteractable>();
                if (interactable != null && interactable.CanGetAffected(ownerCharacterModel, DamageSource.FromAttack))
                {
                    var damageInfo = ownerCharacterModel.GetDamageInfo(DamageSource.FromSkill, 1, null, interactable.Model);
                    var damageDirection = (interactable.Position - ownerCharacterModel.Position).normalized;
                    interactable.GetHit(damageInfo, damageDirection);
                    SpawnImpactAsync(interactable.CenterPosition, cancellationToken).Forget();
                }
            }
        }
        
        private async UniTaskVoid SpawnImpactAsync(Vector2 spawnPoint, CancellationToken token)
        {
            var impact = await PoolManager.Instance.Get(LAZER_IMPACT_EFFECT_NAME, cancellationToken: token);
            impact.transform.position = spawnPoint;
        }
        
        #endregion Class Methods
    }
}