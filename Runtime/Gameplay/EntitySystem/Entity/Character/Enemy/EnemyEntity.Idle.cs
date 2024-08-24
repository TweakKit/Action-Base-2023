using UnityEngine;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class EnemyEntity : CharacterEntity<EnemyModel>
    {
        #region Members

        private float _currentIdleDuration;
        private float _idleDurationDelay;

        #endregion Members

        #region Class Methods

        protected override void PartialInitializeIdle()
        {
            base.PartialInitializeIdle();
            _idleDurationDelay = Random.Range(Constant.CHARACTER_IDLE_MIN_DURATION, Constant.CHARACTER_IDLE_MAX_DURATION);
            _currentIdleDuration = _idleDurationDelay;
            ownerModel.ReactionChangedEvent += OnReactionChangedOnIdle;
        }

        protected override void PartialUpdateIdle()
        {
            if (navigationAgent.IsInNavigationBound)
            {
                UpdateClosestTarget();
                if (IsTargetInDetectionRangeInIdleState())
                {
                    _currentIdleDuration = 0.0f;
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
                            var pathSetType = navigationAgent.SetDestinationStrictly(ownerModel.CurrentTargetedTarget.Position);
                            if (pathSetType != PathSetType.Invalid && navigationAgent.HasPath)
                            {
                                var targetPosition = ownerModel.CurrentTargetedTarget.Position;
                                ownerModel.MoveDirection = (targetPosition - ownerModel.Position).normalized;
                                ownerModel.MovePosition = targetPosition;
                                ownerModel.UpdateState(CharacterState.Move);
                            }
                        }
                    }
                }
                else
                {
                    _currentIdleDuration += Time.deltaTime;
                    if (_currentIdleDuration >= _idleDurationDelay)
                    {
                        _currentIdleDuration = 0.0f;
                        _idleDurationDelay = Random.Range(Constant.CHARACTER_IDLE_MIN_DURATION, Constant.CHARACTER_IDLE_MAX_DURATION);
                        ownerModel.UpdateState(CharacterState.Move);
                    }
                }
            }
        }

        private void OnReactionChangedOnIdle(CharacterReactionType characterReactionType)
        {
            switch (characterReactionType)
            {
                case CharacterReactionType.JustIdle:
                    _idleDurationDelay = Random.Range(Constant.CHARACTER_IDLE_MIN_DURATION, Constant.CHARACTER_IDLE_MAX_DURATION);
                    _currentIdleDuration = 0.0f;
                    ownerModel.MoveDirection = Vector2.zero;
                    break;
            }
        }

        private bool IsTargetInDetectionRangeInIdleState()
        {
            if (ownerModel.CurrentTargetedTarget != null)
            {
                var distanceSqr = Vector2.SqrMagnitude(ownerModel.Position - ownerModel.CurrentTargetedTarget.Position);
                return distanceSqr <= ownerModel.DetectedSqrRange;
            }

            return false;
        }

        #endregion Class Methods
    }
}