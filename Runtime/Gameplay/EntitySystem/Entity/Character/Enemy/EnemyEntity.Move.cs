using UnityEngine;
using Runtime.Definition;
using Runtime.Gameplay.Map;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class EnemyEntity : CharacterEntity<EnemyModel>
    {
        #region Members

        private bool _canFindNewPathToTarget;
        private bool _hasFoundAPathToTarget;
        private bool _hasStartedMovingRandomly;

        #endregion Members

        #region Properties

        protected float RefindTargetThresholdSqr
        {
            get
            {
                return (AttackRange + Constant.ENEMY_REFIND_TARGET_BONUS_RANGE) * (AttackRange + Constant.ENEMY_REFIND_TARGET_BONUS_RANGE);
            }
        }

        #endregion Properties

        #region Class Methods

        protected override void PartialInitializeMove()
        {
            base.PartialInitializeMove();
            _canFindNewPathToTarget = true;
            _hasFoundAPathToTarget = false;
            _hasStartedMovingRandomly = false;
        }

        protected override void PartialUpdateMove()
        {
            if (IsInActivatedRangeInMoveState())
            {
                if (!navigationAgent.IsInNavigationBound)
                {
                    ResetAgent();
                    ownerModel.UpdateState(CharacterState.Idle);
                    return;
                }
                else if (!navigationAgent.UpdateAgent())
                {
                    ResetAgent();
                    ownerModel.UpdateState(CharacterState.Idle);
                    return;
                }

                UpdateClosestTarget();
                if (IsTargetInDetectionRangeInMoveState())
                {
                    if (CanTriggerAttack())
                    {
                        _canFindNewPathToTarget = true;
                        _hasFoundAPathToTarget = false;
                        navigationAgent.Stop();
                        ownerModel.MoveDirection = Vector2.zero;
                        ownerModel.UpdateState(CharacterState.Attack);
                    }
                    else
                    {
                        var isTargetInAttackRange = IsTargetInAttackTriggeredRange(AttackRange);
                        if (isTargetInAttackRange)
                        {
                            _canFindNewPathToTarget = true;
                            _hasFoundAPathToTarget = false;
                            navigationAgent.Stop();
                            ownerModel.MoveDirection = Vector2.zero;
                        }
                        else
                        {
                            CheckFindPathToTarget();
                            if (_hasFoundAPathToTarget)
                                CheckMoveOnPathToTarget();
                            else
                                ownerModel.UpdateState(CharacterState.Idle);
                        }
                    }
                }
                else
                {
                    _canFindNewPathToTarget = true;
                    _hasFoundAPathToTarget = false;
                    if (HasFinishedMovingRandomly())
                    {
                        ownerModel.UpdateState(CharacterState.Idle);
                        _hasStartedMovingRandomly = false;
                    }
                    else NavigateRandomly();
                }
            }
            else
            {
                _canFindNewPathToTarget = true;
                _hasFoundAPathToTarget = false;
                _hasStartedMovingRandomly = true;
                navigationAgent.Stop();
                ownerModel.UpdateTargetedTarget(null);
                transform.position = ownerModel.OriginalPosition;
                ownerModel.Position = ownerModel.OriginalPosition;
                SetActive(false);
            }
        }

        private void ResetAgent()
        {
            _canFindNewPathToTarget = true;
            _hasFoundAPathToTarget = false;
            _hasStartedMovingRandomly = true;
            navigationAgent.Stop();
        }

        private bool IsInActivatedRangeInMoveState()
        {
            var distanceSqr = Vector2.SqrMagnitude(ownerModel.Position - ownerModel.OriginalPosition);
            return distanceSqr <= ownerModel.ActivatedSqrRange;
        }

        private bool IsTargetInDetectionRangeInMoveState()
        {
            if (ownerModel.CurrentTargetedTarget != null)
            {
                if (CheckCanMoveToPosition(ownerModel.CurrentTargetedTarget.Position))
                {
                    var distanceSqr = Vector2.SqrMagnitude(ownerModel.Position - ownerModel.CurrentTargetedTarget.Position);
                    return distanceSqr <= ownerModel.DetectedSqrRange;
                }
            }

            return false;
        }

        private bool HasFinishedMovingRandomly()
        {
            var hasFinishedMoving = _hasStartedMovingRandomly &&
                                    !navigationAgent.HasPath || (navigationAgent.HasPath && navigationAgent.stoppingDistance >= navigationAgent.RemainingDistance);
            return hasFinishedMoving;
        }

        private void NavigateRandomly()
        {
            if (!_hasStartedMovingRandomly)
            {
                float radius = Random.Range(Constant.CHARACTER_MOVE_RANDOM_MIN_RADIUS, Constant.CHARACTER_MOVE_RANDOM_MAX_RADIUS);
                float randomAngleInRadians = Random.Range(0f, 2f * Mathf.PI);
                float xOffset = Mathf.Cos(randomAngleInRadians) * radius;
                float yOffset = Mathf.Sin(randomAngleInRadians) * radius;
                Vector2 randomPosition = ownerModel.Position + new Vector2(xOffset, yOffset);
                if (CheckCanMoveToPosition(randomPosition))
                {
                    var pathSetType = navigationAgent.SetDestinationStrictly(randomPosition);
                    _hasStartedMovingRandomly = pathSetType != PathSetType.Invalid && navigationAgent.HasPath;
                    if (_hasStartedMovingRandomly)
                        ownerModel.MoveDirection = (randomPosition - ownerModel.Position).normalized;
                }
            }
        }

        private void CheckFindPathToTarget()
        {
            if (_canFindNewPathToTarget)
            {
                _canFindNewPathToTarget = false;
                FindNewPath();
            }
        }

        private void FindNewPath()
        {
            var targetPosition = ownerModel.CurrentTargetedTarget.Position;
            var pathSetType = navigationAgent.SetDestinationStrictly(targetPosition);
            if (pathSetType != PathSetType.Invalid && navigationAgent.HasPath)
            {
                ownerModel.MoveDirection = (targetPosition - ownerModel.Position).normalized;
                _hasFoundAPathToTarget = true;
            }
            else _canFindNewPathToTarget = true;
        }

        private void CheckMoveOnPathToTarget()
        {
            // If the target has moved far from the destination where this enemy was supposed to move to, then find another new path.
            if (Vector2.SqrMagnitude(ownerModel.CurrentTargetedTarget.Position - navigationAgent.DestinationPoint) >= RefindTargetThresholdSqr)
            {
                RefindNewPath();
                return;
            }

            if (navigationAgent.stoppingDistance >= navigationAgent.RemainingDistance)
                RefindNewPath();
        }

        private void RefindNewPath()
        {
            _hasFoundAPathToTarget = false;
            _canFindNewPathToTarget = true;
        }

        private bool CheckCanMoveToPosition(Vector2 moveToPosition)
            => MapManager.Instance.CheckEnemyCanMoveToPosition(moveToPosition);

        #endregion Class Methods
    }
}