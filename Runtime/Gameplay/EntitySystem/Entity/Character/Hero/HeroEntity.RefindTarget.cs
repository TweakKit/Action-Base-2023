using System.Collections.Generic;
using Runtime.Definition;
using Runtime.Gameplay.Manager;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class HeroEntity : CharacterEntity<HeroModel>
    {
        #region Members

        private List<IInteractable> _ignoredTargets;

        #endregion Members

        #region Class Methods

        private partial void PartialValidateRefindTarget() { }
        private partial void PartialDisposeRefindTarget() { }

        private partial void PartialInitializeRefindTarget()
        {
            _ignoredTargets = null;
            ownerModel.ReactionChangedEvent += OnReactionChangedOnRefindTarget;
        }

        private partial void PartialUpdateRefindTarget()
        {
            if (ownerModel.CurrentTargetedTarget != null)
            {
                ownerModel.HasCompletedFormation = false;
                var pathSetType = navigationAgent.SetDestinationStrictly(ownerModel.CurrentTargetedTarget.Position);
                if (pathSetType == PathSetType.Invalid || !navigationAgent.HasPath)
                {
                    _ignoredTargets.Add(ownerModel.CurrentTargetedTarget);
                    HeroesControlManager.Instance.RefindTargetForHero(ownerModel, _ignoredTargets);
                }
                else
                {
                    _ignoredTargets = null;
                    var targetPosition = ownerModel.CurrentTargetedTarget.Position;
                    ownerModel.MovementStrategyType = MovementStrategyType.GoToAttackTarget;
                    ownerModel.MoveDirection = (targetPosition - ownerModel.Position).normalized;
                    ownerModel.MovePosition = targetPosition;
                    ownerModel.UpdateState(CharacterState.Move);
                }
            }
            else
            {
                _ignoredTargets = null;
                if (!ownerModel.HasCompletedFormation)
                {
                    var targetPosition = ownerModel.FormationPosition;
                    ownerModel.MovementStrategyType = MovementStrategyType.GoToFormation;
                    ownerModel.MoveDirection = (targetPosition - ownerModel.Position).normalized;
                    ownerModel.MovePosition = targetPosition;
                    ownerModel.UpdateState(CharacterState.Move);
                }
            }
        }

        private void OnReactionChangedOnRefindTarget(CharacterReactionType characterReactionType)
        {
            switch (characterReactionType)
            {
                case CharacterReactionType.JustRefindTarget:
                    _ignoredTargets = new List<IInteractable>();
                    if (ownerModel.CurrentTargetedTarget != null)
                        _ignoredTargets.Add(ownerModel.CurrentTargetedTarget);
                    HeroesControlManager.Instance.RefindTargetForHero(ownerModel, _ignoredTargets);
                    break;
            }
        }

        #endregion Class Methods
    }
}