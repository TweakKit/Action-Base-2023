using UnityEngine;
using Runtime.Definition;
using Runtime.Gameplay.Map;
using Runtime.Gameplay.Manager;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class HeroEntity : CharacterEntity<HeroModel>
    {
        #region Members

        private float _stuckInObstacleTime;
        private float _stuckInPlaceTime;
        private bool _hasStartedMovingRandomlyInBase;
        private Vector2 _savedLastStuckInPlacePosition;

        #endregion Members

        #region Class Methods

        protected override void PartialInitializeMove()
        {
            base.PartialInitializeMove();
            ownerModel.MovementStrategyChangedEvent += OnMovementStrategyChanged;
            _hasStartedMovingRandomlyInBase = false;
        }

        protected override void PartialUpdateMove()
        {
            switch (ownerModel.MovementStrategyType)
            {
                case MovementStrategyType.Spread:
                    if (!navigationAgent.IsInNavigationBound)
                    {
                        navigationAgent.Stop();
                        ownerModel.UpdateState(CharacterState.StandStill);
                        return;
                    }
                    else if (!navigationAgent.UpdateAgent())
                    {
                        navigationAgent.Stop();
                        ownerModel.UpdateState(CharacterState.StandStill);
                        return;
                    }
                    Spread();
                    break;

                case MovementStrategyType.Follow:
                    Follow();
                    navigationAgent.UpdateAgent();
                    break;

                case MovementStrategyType.GoToFormation:
                    GoToFormation();
                    navigationAgent.UpdateAgent();
                    break;

                case MovementStrategyType.GoToAttackTarget:
                    GoToAttackTarget();
                    navigationAgent.UpdateAgent();
                    break;
            }
        }

        protected override void OnStatChangedOnMove(StatType statType, float updatedValue)
        {
            if (statType == StatType.MoveSpeed)
            {
                moveSpeed = updatedValue;
                navigationAgent.maxSpeed = moveSpeed;
            }
        }

        private void Spread()
        {
            if (ownerModel.FollowingModel != null)
            {
                if (HasFinishedMovingRandomlyInBase())
                {
                    ownerModel.UpdateState(CharacterState.StandStill);
                    _hasStartedMovingRandomlyInBase = false;
                }
                else NavigateRandomlyInBase();
            }
            else
            {
                if (ownerModel.HasMoveInput)
                {
                    navigationAgent.SetDestination(ownerModel.MovePosition);
                }
                else
                {
                    if (navigationAgent.HasPath)
                        navigationAgent.Stop();
                }
            }
        }

        private bool HasFinishedMovingRandomlyInBase()
        {
            var hasFinishedMoving = _hasStartedMovingRandomlyInBase &&
                                    !navigationAgent.HasPath || (navigationAgent.HasPath && navigationAgent.stoppingDistance >= navigationAgent.RemainingDistance);
            return hasFinishedMoving;
        }

        private void Follow()
        {
            var movementBiasHeroModel = HeroesControlManager.Instance.MovementBiasHeroModel;
            if (ownerModel != movementBiasHeroModel)
            {
                var distanceBetween = (movementBiasHeroModel.Position - ownerModel.Position).magnitude;
                if (distanceBetween > Constant.HEROES_FORMATION_GROUP_SPEED_DISTANCE_THRESHOLD)
                {
                    var speedMultiplyTimes = Mathf.Min(distanceBetween / Constant.HEROES_FORMATION_GROUP_SPEED_DISTANCE_THRESHOLD,
                                                       Constant.HEROES_FORMATION_GROUP_SPEED_MAX_MULTIPLY_TIMES);
                    var appropriateSpeed = speedMultiplyTimes * moveSpeed;
                    navigationAgent.maxSpeed = appropriateSpeed;
                    navigationAgent.SetDestination(movementBiasHeroModel.Position);
                }
                else
                {
                    navigationAgent.maxSpeed = moveSpeed;
                    navigationAgent.SetDestination(ownerModel.MovePosition);
                }
            }
            else
            {
                navigationAgent.maxSpeed = moveSpeed;
                navigationAgent.SetDestination(ownerModel.MovePosition);
            }
        }

        private void NavigateRandomlyInBase()
        {
            if (!_hasStartedMovingRandomlyInBase)
            {
                var destination = MapManager.Instance.GetRestRandomHeroMoveToPosition(ownerModel.Position);
                ownerModel.MoveDirection = (destination - ownerModel.Position).normalized;
                var pathSetType = navigationAgent.SetDestination(destination);
                _hasStartedMovingRandomlyInBase = pathSetType != PathSetType.Invalid && navigationAgent.HasPath;
            }
        }

        private void GoToFormation()
        {
            if (ownerModel.CurrentTargetedTarget != null)
            {
                ResetStuckTime();
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
                    }
                }
            }
            else
            {
                var pathSetType = navigationAgent.SetDestination(ownerModel.FormationPosition);
                if (pathSetType == PathSetType.Invalid)
                {
                    _stuckInPlaceTime = 0.0f;
                    _stuckInObstacleTime += Time.deltaTime;
                    if (_stuckInObstacleTime >= Constant.HERO_MAX_STUCK_IN_OBSTACLE_TIME_TO_IDLE)
                    {
                        _stuckInObstacleTime = 0.0f;
                        navigationAgent.Stop();
                        ownerModel.HasCompletedFormation = true;
                        ownerModel.MoveDirection = Vector2.zero;
                        ownerModel.UpdateState(CharacterState.Idle);
                    }
                }
                else
                {
                    _stuckInObstacleTime = 0.0f;
                    _stuckInPlaceTime += Time.deltaTime;
                    if (_stuckInPlaceTime >= Constant.HERO_MAX_STUCK_IN_PLACE_TIME_TO_IDLE)
                    {
                        if (Vector2.SqrMagnitude(ownerModel.Position - _savedLastStuckInPlacePosition) <= Constant.HERO_STUCK_IN_PLACE_THRESHOLD)
                        {
                            navigationAgent.Stop();
                            ownerModel.HasCompletedFormation = true;
                            ownerModel.MoveDirection = Vector2.zero;
                            ownerModel.UpdateState(CharacterState.Idle);
                        }
                        _stuckInPlaceTime = 0.0f;
                        _savedLastStuckInPlacePosition = ownerModel.Position;
                    }
                }
            }
        }

        private void GoToAttackTarget()
        {
            if (ownerModel.CurrentTargetedTarget != null)
            {
                var pathSetType = navigationAgent.SetDestinationStrictly(ownerModel.CurrentTargetedTarget.Position);
                if (pathSetType == PathSetType.Invalid || !navigationAgent.HasPath)
                {
                    navigationAgent.Stop();
                    ownerModel.MoveDirection = Vector2.zero;
                    ownerModel.UpdateState(CharacterState.RefindTarget);
                }
                else
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
                            ownerModel.MoveDirection = (targetPosition - ownerModel.Position).normalized;
                            ownerModel.MovePosition = targetPosition;
                        }
                    }
                }
            }
            else
            {
                var targetPosition = ownerModel.FormationPosition;
                ownerModel.MovementStrategyType = MovementStrategyType.GoToFormation;
                ownerModel.MoveDirection = (targetPosition - ownerModel.Position).normalized;
                ownerModel.MovePosition = targetPosition;
            }
        }

        private void OnMovementStrategyChanged(MovementStrategyType movementStrategyType)
        {
            _hasStartedMovingRandomlyInBase = false;
            ownerModel.MoveDirection = Vector2.zero;
            navigationAgent.Stop();
            navigationAgent.maxSpeed = moveSpeed;
            ResetStuckTime();
        }

        private void ResetStuckTime()
        {
            _stuckInObstacleTime = 0.0f;
            _stuckInPlaceTime = 0.0f;
            _savedLastStuckInPlacePosition = ownerModel.Position;
        }

        #endregion Class Methods
    }
}