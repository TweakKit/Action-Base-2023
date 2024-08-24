using System;
using System.Threading;
using Runtime.Manager.Pool;
using Cysharp.Threading.Tasks;

namespace Runtime.Gameplay.EntitySystem
{
    public class RangedSpawnImpactAttackStrategy : RangedAttackStrategy
    {
        #region Members

        private const int NUMBER_OF_IMPACTS = 4;
        private const float DISTANCE_BETWEEN_IMPACTS = 1.5f;
        private const float DELAY_BETWEEN_IMPACTS = 0.1f;
        private const string IMPACT_PREFAB = "2004_explode_impact";

        #endregion Members

        #region Class Methods

        protected override async UniTaskVoid SpawnProjectileAsync(IInteractable interactableTarget, CancellationToken cancellationToken)
        {
            var direction = (interactableTarget.Position - ownerCharacterModel.Position).normalized;
            for (int i = 0; i < NUMBER_OF_IMPACTS; i++)
            {
                var order = i + 1;
                var impact = await PoolManager.Instance.Get(IMPACT_PREFAB, cancellationToken);
                impact.transform.position = ownerCharacterModel.Position + direction * order * DISTANCE_BETWEEN_IMPACTS;
                var damageBox = impact.GetComponent<SpriteAnimatorDamageBox>();
                damageBox.Init(ownerCharacterModel, DamageSource.FromAttack, false, 1, null);
                await UniTask.Delay(delayTimeSpan: TimeSpan.FromSeconds(DELAY_BETWEEN_IMPACTS), cancellationToken: cancellationToken);
            }
        }

        #endregion Class Methods
    }
}