using System.Threading;
using UnityEngine;
using Runtime.Manager.Pool;
using Runtime.Definition;
using Cysharp.Threading.Tasks;

namespace Runtime.Gameplay.EntitySystem
{
    public class MeleeAutoDestroyAttackStrategy : MeleeAttackStrategy
    {
        #region Members

        private static readonly string s_autoDestroyExplosionName = "auto_destroy_effect";

        #endregion Members

        #region Class Methods

        protected override void DamageToTarget(CancellationToken cancellationToken)
        {
            SpawnDestroyVFX(cancellationToken).Forget();
            var attackRange = ownerCharacterModel.GetTotalStatValue(StatType.AttackRange);
            var colliders = Physics2D.OverlapCircleAll(ownerCharacterModel.Position, attackRange);
            foreach (var collider in colliders)
            {
                var interactable = collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    if (interactable.Model == ownerCharacterModel)
                    {
                        var damageInfo = ownerCharacterModel.GetDamageInfo(DamageSource.FromAttack, ownerCharacterModel.MaxHp, null, interactable.Model);
                        var damageDirection = (interactable.Model.Position - ownerCharacterModel.Position).normalized;
                        interactable.GetHit(damageInfo, damageDirection);
                    }
                    else if (interactable.CanGetAffected(ownerCharacterModel, DamageSource.FromAttack))
                    {
                        var damageInfo = GetDamageInfo(interactable.Model);
                        var damageDirection = (interactable.Model.Position - ownerCharacterModel.Position).normalized;
                        interactable.GetHit(damageInfo, damageDirection);
                    }
                }
            }
        }

        private async UniTaskVoid SpawnDestroyVFX(CancellationToken cancellationToken)
        {
            var destroyedVFX = await PoolManager.Instance.Get(s_autoDestroyExplosionName, cancellationToken);
            destroyedVFX.transform.position = ownerCharacterModel.Position;
        }

        #endregion Class Methods
    }
}