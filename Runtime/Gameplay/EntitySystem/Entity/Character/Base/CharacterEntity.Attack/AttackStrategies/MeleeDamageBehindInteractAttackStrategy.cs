using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Runtime.Extensions;
using Runtime.Manager.Pool;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class MeleeDamageBehindInteractAttackStrategy : MeleeInteractAttackStrategy
    {
        #region Members

        private readonly float _angleConeDamage = 120.0f;
        private readonly float _radiusDamage = 3.0f;
        private readonly float _damageAttackPercent = 1.0f;
        private const string VFX_NAME = "vfx_damage_behind";

        #endregion Members

        #region Class Methods

        protected override void DamageToTarget(CancellationToken cancellationToken)
        {
            base.DamageToTarget(cancellationToken);
            if (ownerCharacterModel.CanDealDamageBehindTarget)
            {
                if (ownerCharacterModel.CurrentAttackedTarget != null && ownerCharacterModel.CurrentAttackedTarget.IsCharacter)
                {
                    var originalTarget = ownerCharacterModel.CurrentAttackedTarget;
                    var startPosition = ownerCharacterModel.CurrentAttackedTarget.Position;
                    var direction = (startPosition - ownerCharacterModel.Position).normalized;
                    GenerateVFXBegin(startPosition, direction, cancellationToken).Forget();
                    var colliders = Physics2D.OverlapCircleAll(startPosition, _radiusDamage);
                    colliders = OverlapConeAll(ownerCharacterModel.CurrentAttackedTarget.CenterPosition, direction, colliders, _angleConeDamage);
                    foreach (var collider in colliders)
                    {
                        var interactable = collider.GetComponent<IInteractable>();
                        if (interactable != null && interactable.IsCharacter 
                                                 && interactable.CanGetAffected(ownerCharacterModel, DamageSource.FromSkill)
                                                 && interactable != originalTarget)
                        {
                            var damageInfo = ownerCharacterModel.GetDamageInfo(DamageSource.FromSkill, _damageAttackPercent, null, interactable.Model);
                            var damageDirection = (interactable.Position - ownerCharacterModel.Position).normalized;
                            interactable.GetHit(damageInfo, damageDirection);
                        }
                    }
                }
            }
        }
        
        protected Collider2D[] OverlapConeAll(Vector2 startPosition, Vector2 triggerDirection, Collider2D[] orinalTargets, float angleCone)
        {
            List<Collider2D> listColliderResult = new List<Collider2D>();
            for (int i = 0; i < orinalTargets.Length; i++)
            {
                Vector2 targetPosition = orinalTargets[i].transform.position;
                var directionTarget = (targetPosition - startPosition).normalized;
                var angleValue = Math.Abs(Vector2.Angle(triggerDirection, directionTarget));
                if (angleValue <= angleCone / 2)
                    listColliderResult.Add(orinalTargets[i]);
            }
            return listColliderResult.ToArray();
        }
        
        private async UniTaskVoid GenerateVFXBegin(Vector2 position, Vector2 direction, CancellationToken cancellationToken)
        {
            var vfx = await PoolManager.Instance.Get(VFX_NAME, cancellationToken);
            vfx.transform.position = position;
            vfx.transform.rotation = direction.ToQuaternion();
        }

        #endregion Class Methods
    }
}