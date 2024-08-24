using UnityEngine;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class HeroEntity : CharacterEntity<HeroModel>
    {
        #region Class Methods

        protected override void PartialUpdateIdle()
        {
            if (ownerModel.CurrentTargetedTarget != null)
            {
                ownerModel.HasCompletedFormation = false;
                if (CanTriggerAttack())
                {
                    navigationAgent.Stop();
                    ownerModel.MoveDirection = Vector2.zero;
                    ownerModel.UpdateState(CharacterState.Attack);
                }
                else
                {
                    var isTargetInAttackRange = IsTargetInAttackTriggeredRange(AttackRange);
                    if (isTargetInAttackRange)
                    {
                        navigationAgent.Stop();
                        ownerModel.MoveDirection = Vector2.zero;
                    }
                    else
                    {
                        var targetPosition = ownerModel.CurrentTargetedTarget.Position;
                        ownerModel.MovementStrategyType = MovementStrategyType.GoToAttackTarget;
                        ownerModel.MoveDirection = (targetPosition - ownerModel.Position).normalized;
                        ownerModel.MovePosition = targetPosition;
                        ownerModel.UpdateState(CharacterState.Move);
                    }
                }
            }
            else
            {
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

        #endregion Class Methods
    }
}