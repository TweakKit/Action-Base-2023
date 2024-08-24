using UnityEngine;
using Runtime.Definition;
using Cysharp.Threading.Tasks;

namespace Runtime.Gameplay.EntitySystem
{
    public abstract partial class CharacterEntity<T> : Entity<T> where T : CharacterModel
    {
        #region Members

        protected IAttackStrategy attackStrategy;

        #endregion Members

        #region Properties

        /// <summary>
        /// The attack range of the character.
        /// The returned attack range should be the smallest range of all ranges (skill range, attack range, interaction range,..).
        /// Because it's used to determine where the character should stop chasing the target.
        /// </summary>
        protected virtual float AttackRange
        {
            get
            {
                return ownerModel.GetTotalStatValue(StatType.AttackRange);
            }
        }

        #endregion Properties

        #region Class Methods

        protected virtual partial void PartialValidateAttack() { }

        protected virtual partial void PartialInitializeAttack()
            => ownerModel.HardCCImpactedEvent += OnHardCCImpactedOnAttack;

        protected virtual partial void PartialUpdateAttack()
        {
            if (ownerModel.CheckCanAttack)
            {
                if (ownerModel.CurrentTargetedTarget != null)
                {
                    ownerModel.UpdateAttackedTarget(ownerModel.CurrentTargetedTarget);
                    TriggerAtttack();
                }
                else ownerModel.UpdateState(CharacterState.Idle);
            }
        }

        protected virtual partial void PartialDisposeAttack()
            => attackStrategy?.Dispose();

        protected void OnHardCCImpactedOnAttack(StatusEffectType statusEffectType)
            => StopActionByAllMeans();

        protected virtual async UniTask RunAttackAsync()
        {
            ownerModel.IsAttacking = true;
            await attackStrategy.OperateAttackAsync();
            ownerModel.IsAttacking = false;
            ownerModel.ReactionChangedEvent.Invoke(CharacterReactionType.JustFinishAttack);
            ownerModel.UpdateState(CharacterState.Idle);
        }

        protected virtual void StopActionByAllMeans()
        {
            if (ownerModel.IsAttacking)
            {
                ownerModel.IsAttacking = false;
                StopAttacking();
            }
        }

        protected virtual void StopAttacking()
            => attackStrategy.Cancel();

        protected virtual bool CanTriggerAttack() => false;
        protected virtual void TriggerAtttack() { }

        protected bool IsTargetInAttackTriggeredRange(float checkedAttackTriggeredRange)
        {
            var comparedRange = checkedAttackTriggeredRange + ownerModel.CurrentTargetedTarget.Model.BodyBoundRadius;
            var sqrDistanceBetweenTargetAndOwner = Vector2.SqrMagnitude(ownerModel.CurrentTargetedTarget.Position - ownerModel.Position);
            return sqrDistanceBetweenTargetAndOwner <= comparedRange * comparedRange;
        }

        #endregion Class Methods
    }
}