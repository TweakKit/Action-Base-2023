using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class PVPHeroEntity : HeroEntity
    {
        #region Class Methods

        protected override void PartialUpdateMove()
        {
            if(ownerModel.IsBlockControl)
                return;
            navigationAgent.UpdateAgent();
            UpdateClosestTarget();
            if (HasTargetInMoveState())
            {
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
                        ownerModel.MoveDirection = (targetPosition - ownerModel.Position).normalized;
                        navigationAgent.SetDestination(targetPosition);
                    }
                }
            }
            else
            {
                navigationAgent.Stop();
                ownerModel.MoveDirection = Vector2.zero;
            }
        }

        private bool HasTargetInMoveState()
        {
            if (ownerModel.CurrentTargetedTarget != null)
                return true;
            else
                return false;
        }

        #endregion Class Methods
    }
}