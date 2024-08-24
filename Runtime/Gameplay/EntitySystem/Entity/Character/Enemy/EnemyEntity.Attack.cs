using UnityEngine;
using Runtime.Definition;
using Cysharp.Threading.Tasks;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class EnemyEntity : CharacterEntity<EnemyModel>
    {
        #region Class Methods

        protected override void PartialInitializeAttack()
        {
            base.PartialInitializeAttack();
            attackStrategy = AttackStrategyFactory.GetAttackStrategy(ownerModel.EntityType.IsHero(), ownerModel.AttackType);
            attackStrategy.Init(ownerModel, transform);
            ownerModel.ReactionChangedEvent += OnReactionChangedOnAttack;
        }

        protected override bool CanTriggerAttack()
        {
            if (attackStrategy != null)
            {
                if (attackStrategy.CheckCanAttack())
                    return IsTargetInAttackTriggeredRange(AttackRange);
            }
            return false;
        }

        protected override void TriggerAtttack()
        {
            if(ownerModel.IsBlockControl)
                return;
            RunAttackAsync().Forget();
        }

        private void OnReactionChangedOnAttack(CharacterReactionType characterReactionType)
        {
            switch (characterReactionType)
            {
                case CharacterReactionType.JustSawHeroTeleported:
                    StopActionByAllMeans();
                    break;
            }
        }

        #endregion Class Methods
    }
}