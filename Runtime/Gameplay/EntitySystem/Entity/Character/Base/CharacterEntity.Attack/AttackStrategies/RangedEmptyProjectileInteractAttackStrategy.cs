using System.Threading;
using Cysharp.Threading.Tasks;
using Runtime.Audio;
using Runtime.Manager.Pool;

namespace Runtime.Gameplay.EntitySystem
{
    public class RangedEmptyProjectileInteractAttackStrategy : RangedInteractAttackStrategy
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

        protected override void PlaySoundRangeAttack(CancellationToken cancellationToken)
        {
            var keySound = string.Format(AudioConstants.RANGE_EMPTY_ATK, ownerCharacterModel.EntityVisualId);
            AudioController.Instance.PlaySoundEffectAsync(keySound, cancellationToken).Forget();
        }

        #endregion Class Methods
    }
}