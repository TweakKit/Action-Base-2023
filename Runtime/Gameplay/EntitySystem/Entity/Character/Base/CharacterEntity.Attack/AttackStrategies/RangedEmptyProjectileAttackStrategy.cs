using System.Threading;
using Runtime.Manager.Pool;
using Cysharp.Threading.Tasks;

namespace Runtime.Gameplay.EntitySystem
{
    public class RangedEmptyProjectileAttackStrategy : RangedAttackStrategy
    {
        #region Class Methods

        protected override async UniTaskVoid SpawnProjectileAsync(IInteractable interactableTarget, CancellationToken cancellationToken)
        {
            var damageInfo = ownerCharacterModel.GetDamageInfo(DamageSource.FromAttack, 1, null, interactableTarget.Model);
            var damageDirection = (interactableTarget.Position - ownerCharacterModel.Position).normalized;
            interactableTarget.GetHit(damageInfo, damageDirection);
            var projectilePosition = interactableTarget.Model.Position;
            var projectileId = ownerCharacterModel.EntityVisualId + PROJECTILE_POSTFIX_NAME;
            var projectitle = await PoolManager.Instance.Get(projectileId, cancellationToken);
            projectitle.transform.position = projectilePosition;
        }

        #endregion Class Methods
    }
}