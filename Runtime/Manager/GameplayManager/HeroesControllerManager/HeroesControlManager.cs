using System.Threading;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Runtime.Manager.Input;
using Runtime.Gameplay.EntitySystem;
using Runtime.Definition;
using Runtime.Message;
using Runtime.Common.Singleton;
using Runtime.Navigation;
using Runtime.Utilities;
using Runtime.Manager.Data;
using Cysharp.Threading.Tasks;

namespace Runtime.Gameplay.Manager
{
    public sealed partial class HeroesControlManager : MonoSingleton<HeroesControlManager>
    {
        #region Members

        [Header("--- GROUP CENTER HOLDER TRANSFORM ---")]
        [SerializeField]
        private Transform _heroesGroupCenterHolder;

        [Header("--- VISUAL INDICATOR ---")]
        [SerializeField]
        private HeroesGroupIndicator _heroesGroupIndicator;

        private bool _canControl;
        private bool _hasReturnedMovementControl;
        private bool _hasStoppedMovementControl;
        private bool _keepMainHeroAsMovementBiasHero;
        private IVector3Input _movementInput;
        private Vector2 _heroesGroupCenterPoint;
        private float _heroesGroupExploitRadius;
        private float _heroesGroupFormationRadius;
        private HeroModel _mainHeroModel;
        private List<HeroModelTransform> _heroModelTransforms;

        #endregion Members

        #region Properties

        public Transform HeroesGroupCenterHolder => _heroesGroupCenterHolder;
        public HeroModel MovementBiasHeroModel { get; private set; }
        public bool IsInRestStation { get; private set; }
        public Vector2 HeroGroupCenterPoint => _heroesGroupCenterPoint;

        #endregion Properties

        #region API Methods

        protected override void Awake()
        {
            base.Awake();
            InitTargetsDetector();
        }

        private void Update()
        {
            if (_canControl)
            {
                var input = _movementInput.Input;
                if (input != Vector3.zero)
                {
                    if (!_mainHeroModel.IsInHardCCStatus)
                    {
                        _hasStoppedMovementControl = false;
                        if (!_hasReturnedMovementControl)
                        {
                            _hasReturnedMovementControl = true;
                            StartMovementControl();
                        }
                        UpdateHeroesGroupPositions();
                        ApplyFormation(input);
                    }
                }
                else
                {
                    if (!_hasStoppedMovementControl)
                    {
                        _hasReturnedMovementControl = false;
                        StopMovementControl();
                        StopFormation();
                    }
                    _hasStoppedMovementControl = true;
                }
            }
        }

        #endregion API Methods

        #region Class Methods

        public void HandleDataLoaded(CancellationToken cancellationToken)
        {
            _canControl = false;
            _hasReturnedMovementControl = true;
            _hasStoppedMovementControl = false;
            _keepMainHeroAsMovementBiasHero = false;
            _heroesGroupIndicator.Hide();
            _movementInput = InputManager.Instance.MovementInput;
            _heroModelTransforms = EntitiesManager.Instance.HeroModelTransforms;
        }

        public void SetInitialInRestStationStatus(bool isInRestStation)
            => IsInRestStation = isInRestStation;

        public void SetInRestStation(bool isInRestStation, bool isBase, string idRest, Vector2 centerRest)
        {
            IsInRestStation = isInRestStation;
            if (IsInRestStation)
                EntitiesManager.CurrentBattleIndex += 1;
            var directionToRestStation = (centerRest - EntitiesManager.Instance.MainHeroModel.Position).normalized;
            Messenger.Publish(new ChangeRestStateMessage(IsInRestStation, isBase, idRest, directionToRestStation));
        }

        public void HandleHeroDied()
        {
            _mainHeroModel = EntitiesManager.Instance.MainHeroModel;
            _canControl = _mainHeroModel != null;
            if (_mainHeroModel != null)
            {
                UpdateHeroesGroupPositions();
                CalculateHeroesGroupExploitSize();
                CalculateHeroesGroupFormationSize();
            }
        }

        public void HandleHeroApproachGate(ChangeRestStateMessage changeRestStateMessage)
        {
            var newHeroMovementStrategyType = changeRestStateMessage.IsInRestStation
                                            ? MovementStrategyType.Spread
                                            : MovementStrategyType.Follow;

            if (changeRestStateMessage.IsInRestStation)
            {
                _keepMainHeroAsMovementBiasHero = false;
            }
            else
            {
                var mainPosition = EntitiesManager.Instance.MainHeroModel.Position;
                var directionToRestStation = changeRestStateMessage.DirectionToRestStation;
                for (int i = 0; i < _heroModelTransforms.Count; i++)
                {
                    if (_heroModelTransforms[i].Model != _mainHeroModel)
                    {
                        if (ScreenUtility.IsPositionOffScreen(_heroModelTransforms[i].Model.Position))
                        {
                            var count = 0;
                            while (true)
                            {
                                count++;
                                var heroHeadOffBasePosition = mainPosition + directionToRestStation * count;
                                if (ScreenUtility.IsPositionOffScreen(heroHeadOffBasePosition))
                                {
                                    count++;
                                    heroHeadOffBasePosition = mainPosition + directionToRestStation * count;
                                    var transformedPoints = TransformUtility.GetPositionsAroundPoint(heroHeadOffBasePosition);
                                    _heroModelTransforms[i].Model.SetTransformMovePosition(transformedPoints[Random.Range(0, transformedPoints.Count)]);
                                    break;
                                }
                            }
                        }
                    }
                }
                _keepMainHeroAsMovementBiasHero = true;
            }

            for (int i = 0; i < _heroModelTransforms.Count; i++)
                _heroModelTransforms[i].Model.MovementStrategyType = newHeroMovementStrategyType;
        }

        public void HandleBaseMainBuildingFeatureUnlocked()
        {
            var heroCurrentMovementStrategyType = _mainHeroModel.MovementStrategyType;
            var newHeroMovementStrategyType = heroCurrentMovementStrategyType == MovementStrategyType.Spread
                                            ? MovementStrategyType.Follow
                                            : MovementStrategyType.Spread;

            for (int i = 0; i < _heroModelTransforms.Count; i++)
                _heroModelTransforms[i].Model.MovementStrategyType = newHeroMovementStrategyType;

            _keepMainHeroAsMovementBiasHero = false;
        }

        public void HandleHeroesSpawned()
        {
            var initialMainHeroModel = EntitiesManager.Instance.MainHeroModel;
            if (_mainHeroModel != initialMainHeroModel)
            {
                _movementInput.Reset();
                _canControl = true;
                _mainHeroModel = initialMainHeroModel;
                _keepMainHeroAsMovementBiasHero = false;
                MovementBiasHeroModel = _mainHeroModel;

                var currentMovementStrategyType = _mainHeroModel.MovementStrategyType;
                if (currentMovementStrategyType == MovementStrategyType.Spread)
                {
                    var currentState = _mainHeroModel.CharacterState;
                    foreach (var heroModelTransform in EntitiesManager.Instance.HeroModelTransforms)
                    {
                        if (heroModelTransform.Model != _mainHeroModel)
                        {
                            heroModelTransform.Model.MovementStrategyType = currentMovementStrategyType;
                            heroModelTransform.Model.UpdateState(currentState);
                            heroModelTransform.Model.SetFollowingModel(_mainHeroModel);
                        }
                        else heroModelTransform.Model.SetFollowingModel(null);
                    }
                }
                else
                {
                    foreach (var heroModelTransform in EntitiesManager.Instance.HeroModelTransforms)
                    {
                        if (heroModelTransform.Model != _mainHeroModel)
                            heroModelTransform.Model.SetFollowingModel(_mainHeroModel);
                        else
                            heroModelTransform.Model.SetFollowingModel(null);
                    }
                }
            }
            UpdateHeroesGroupPositions();
            CheckUpdateHeroesMissingTarget();
            CalculateHeroesGroupExploitSize();
            CalculateHeroesGroupFormationSize();
        }

        public async UniTask HandleHeroesPreMoveToNextFloorAsync(CancellationToken cancellationToken)
        {
            _canControl = false;
            _movementInput.Disable();
            await UniTask.CompletedTask;
        }

        public async UniTask HandleHeroesAfterMoveToNextFloorAsync(Vector2[] floorPoints, CancellationToken cancellationToken)
        {
            _canControl = true;
            _movementInput.Enable();
            var randomMoveFloorPointIndex = Random.Range(0, floorPoints.Length);
            for (int i = 0; i < _heroModelTransforms.Count; i++)
            {
                var nextPoint = floorPoints[++randomMoveFloorPointIndex % floorPoints.Length];
                _heroModelTransforms[i].Model.SetTransformMovePosition(nextPoint);
                _heroModelTransforms[i].Model.HasCompletedFormation = false;
                _heroModelTransforms[i].Model.MovementStrategyType = MovementStrategyType.GoToFormation;
                _heroModelTransforms[i].Model.FormationPosition = _heroModelTransforms[i].Model.MovePosition;
            }
            UpdateHeroesGroupPositions();
            await UniTask.Yield(cancellationToken);
            UpdateAllHeroesTargets();
        }

        public void HandleHeroesMoveIntoBase()
        {
            var newMainHeroModel = EntitiesManager.Instance.MainHeroModel;
            if (_mainHeroModel != newMainHeroModel)
                _mainHeroModel = newMainHeroModel;
        }

        public void HandleHeroesStatsUpdated(CancellationToken cancellationToken)
            => RunHeroesStatsUpdatedAsync(cancellationToken).Forget();

        public void HandleCardBuffForHeroes(CardBuffType cardBuffType, float buffValue, StatModifyType buffModifyType, CancellationToken cancellationToken)
        {
            foreach (var heroModelTransform in _heroModelTransforms)
            {
                var heroModel = heroModelTransform.Model;
                if (cardBuffType == CardBuffType.HealTeam)
                {
                    var healAmount = buffValue * heroModel.MaxHp;
                    heroModel.BuffHp(healAmount);
                }
                else
                {
                    var statType = cardBuffType.ConvertToStatType();
                    heroModel.BuffStat(statType, buffValue, buffModifyType);
                }
            }
        }

        public void HandleMapFogUnlocked(CancellationToken cancellationToken)
            => CheckUpdateHeroesMissingTarget();

        public void HandleGameFlowStopped()
        {
            _hasReturnedMovementControl = false;
            _hasStoppedMovementControl = true;
            _movementInput.Reset();
            if (_movementInput.Input != Vector3.zero && _mainHeroModel != null)
            {
                StopMovementControl();
                StopFormation();
            }
        }

        private async UniTask RunHeroesStatsUpdatedAsync(CancellationToken cancellationToken)
        {
            var heroFragments = DataManager.Local.HeroFragments;
            foreach (var heroModelTransform in _heroModelTransforms)
            {
                var heroModel = heroModelTransform.Model;
                var heroId = heroModel.EntityId;
                var updatedHeroFragment = heroFragments.FirstOrDefault(x => x.id == heroId);
                var updatedHeroLevel = updatedHeroFragment.level;

                // Update hero's stats.
                var updatedHeroStats = await GameplayDataManager.Instance.GetHeroStatsAsync(heroId, updatedHeroLevel, cancellationToken);
                heroModel.UpdateStats(updatedHeroStats);

                // Update hero's skills.
                var updatedHeroSkillIdentities = await GameplayDataManager.Instance.GetHeroSkillIdentitiesAsync(heroId, updatedHeroLevel, cancellationToken);
                if (updatedHeroSkillIdentities.Count > 0)
                {
                    var updatedHeroSkillModels = await GameplayDataManager.Instance.GetHeroSkillModelsAsync(updatedHeroSkillIdentities, cancellationToken);
                    heroModel.SkillStrategyUpdatedEvent.Invoke(updatedHeroSkillModels);
                }
            }
        }

        public Vector2 GetHeroesGroupCenterPosition()
        {
            var groupCenterPosition = Vector2.zero;
            foreach (var heroModelTransform in _heroModelTransforms)
                groupCenterPosition += heroModelTransform.Model.Position;
            groupCenterPosition /= _heroModelTransforms.Count;
            return groupCenterPosition;
        }

        private void StartMovementControl()
        {
            for (int i = 0; i < _heroModelTransforms.Count; i++)
                _heroModelTransforms[i].Model.ReactionChangedEvent.Invoke(CharacterReactionType.justMoveInGroup);
            Messenger.Publish(new HeroesControllingStatusUpdatedMessage(HeroesControllingType.MoveInGroup));
        }

        private void StopMovementControl()
        {
            UpdateAllHeroesTargets();
            Messenger.Publish(new HeroesControllingStatusUpdatedMessage(HeroesControllingType.DissolveToFormation));
        }

        private void ApplyFormation(Vector2 direction)
        {
            if (_mainHeroModel.MovementStrategyType == MovementStrategyType.Spread)
            {
                var targetPosition = _mainHeroModel.Position + direction;
                _mainHeroModel.MoveDirection = direction;
                _mainHeroModel.MovePosition = targetPosition;
            }
            else
            {
                var targetPosition = MovementBiasHeroModel.Position + direction * _heroesGroupFormationRadius * 2.0f;
                for (int i = 0; i < _heroModelTransforms.Count; i++)
                {
                    if (_heroModelTransforms[i].Model.IsInHardCCStatus)
                    {
                        var headDirection = (targetPosition - _heroModelTransforms[i].Model.Position).normalized;
                        _heroModelTransforms[i].Model.MovePosition = _heroModelTransforms[i].Model.Position + headDirection;
                    }
                    else
                    {
                        var headDirection = (targetPosition - _heroModelTransforms[i].Model.Position).normalized;
                        _heroModelTransforms[i].Model.MoveDirection = headDirection;
                        _heroModelTransforms[i].Model.MovePosition = _heroModelTransforms[i].Model.Position + headDirection;
                        _heroModelTransforms[i].Model.MovementStrategyType = MovementStrategyType.Follow;
                        _heroModelTransforms[i].Model.UpdateState(CharacterState.Move);
                    }
                }
            }
        }

        private void StopFormation()
        {
            if (_mainHeroModel.MovementStrategyType == MovementStrategyType.Spread)
            {
                _mainHeroModel.MoveDirection = Vector2.zero;
            }
            else
            {
                var groupCenterHeroModel = MovementBiasHeroModel;
                if (!_keepMainHeroAsMovementBiasHero)
                {
                    var groupCenterPosition = GetHeroesGroupCenterPosition();
                    float closestSqrDistance = float.MaxValue;

                    for (int i = 0; i < _heroModelTransforms.Count; i++)
                    {
                        var sqrDistanceBetween = (_heroModelTransforms[i].Model.Position - groupCenterPosition).sqrMagnitude;
                        if (closestSqrDistance > sqrDistanceBetween)
                        {
                            closestSqrDistance = sqrDistanceBetween;
                            groupCenterHeroModel = _heroModelTransforms[i].Model;
                        }
                    }
                }
                var circleFormationShape = new CircleFormationShape(_heroesGroupFormationRadius, Constant.HEROES_FORMATION_MIN_DISTANCE);
                var formationAlignedPositions = FormationArranger.GetAlignedPositions(_heroModelTransforms.Count - 1, circleFormationShape);
                for (int i = 0; i < _heroModelTransforms.Count; i++)
                {
                    if (!_heroModelTransforms[i].Model.IsInHardCCStatus)
                    {
                        var formationPosition = Vector2.zero;
                        var formationDirection = Vector2.zero;
                        if (_heroModelTransforms[i].Model != groupCenterHeroModel)
                        {
                            Vector2 closestFormationAlignedPosition = Vector2.zero;
                            var highestDotValue = float.MinValue;
                            var heroDirectionFromCenterGroupPosition = (_heroModelTransforms[i].Model.Position - _heroesGroupCenterPoint).normalized;
                            foreach (var formationAlignedPosition in formationAlignedPositions)
                            {
                                var direction = formationAlignedPosition.normalized;
                                var dotValue = Vector2.Dot(direction, heroDirectionFromCenterGroupPosition);
                                if (dotValue > highestDotValue)
                                {
                                    highestDotValue = dotValue;
                                    closestFormationAlignedPosition = formationAlignedPosition;
                                }
                            }
                            formationAlignedPositions.Remove(closestFormationAlignedPosition);
                            formationPosition = groupCenterHeroModel.Position + closestFormationAlignedPosition;
                            formationDirection = (formationPosition - _heroModelTransforms[i].Model.Position).normalized;
                        }
                        else
                        {
                            formationPosition = groupCenterHeroModel.Position;
                            formationDirection = Vector2.zero;
                        }
                        _heroModelTransforms[i].Model.HasCompletedFormation = false;
                        _heroModelTransforms[i].Model.MovementStrategyType = MovementStrategyType.GoToFormation;
                        _heroModelTransforms[i].Model.MoveDirection = formationDirection;
                        _heroModelTransforms[i].Model.MovePosition = formationPosition;
                        _heroModelTransforms[i].Model.FormationPosition = formationPosition;
                    }
                }
            }
            ShowHeroesGroupIndicator();
        }

        private void ShowHeroesGroupIndicator()
        {
            if (_mainHeroModel.MovementStrategyType == MovementStrategyType.Spread)
            {
                _heroesGroupCenterPoint = _mainHeroModel.Position;
                _heroesGroupIndicator.Show(_heroesGroupExploitRadius, _heroesGroupCenterPoint);
            }
            else
            {
                var heroesGroupCenterPoint = Vector2.zero;
                foreach (var heroModelTransform in _heroModelTransforms)
                    heroesGroupCenterPoint += heroModelTransform.Model.FormationPosition;
                heroesGroupCenterPoint /= _heroModelTransforms.Count;
                _heroesGroupCenterPoint = heroesGroupCenterPoint;
                _heroesGroupIndicator.Show(_heroesGroupExploitRadius, _heroesGroupCenterPoint);
            }
            UpdateHeroesGroupCenterHolder();
        }

        private void UpdateHeroesGroupPositions()
        {
            if (IsInRestStation)
            {
                var hasUnlockedBaseMainBuilding = DataManager.Local.HasUnlockedBaseMainBuilding;
                if (hasUnlockedBaseMainBuilding)
                {
                    MovementBiasHeroModel = _mainHeroModel;
                    _heroesGroupCenterPoint = _mainHeroModel.Position;
                    UpdateHeroesGroupCenterHolder();
                    return;
                }
            }

            if (_keepMainHeroAsMovementBiasHero)
            {
                MovementBiasHeroModel = _mainHeroModel;
                _heroesGroupCenterPoint = _mainHeroModel.Position;
                foreach (var heroModelTransform in _heroModelTransforms)
                {
                    if (MovementBiasHeroModel != heroModelTransform.Model)
                    {
                        var sqrDistanceBetween = Vector2.SqrMagnitude(heroModelTransform.Model.Position - MovementBiasHeroModel.Position);
                        if (sqrDistanceBetween < Constant.HEROES_COLLIDE_DISTANCE_SQR_THRESHOLD)
                        {
                            _keepMainHeroAsMovementBiasHero = false;
                            break;
                        }
                    }
                }
            }
            else
            {
                // Update the group center point.
                var heroesGroupCenterPoint = Vector2.zero;
                foreach (var heroModelTransform in _heroModelTransforms)
                    heroesGroupCenterPoint += heroModelTransform.Model.Position;
                heroesGroupCenterPoint /= _heroModelTransforms.Count;
                _heroesGroupCenterPoint = heroesGroupCenterPoint;

                // Find the movement bias hero model.
                var highestDotValue = float.MinValue;
                HeroModel newMovementBiasHeroModel = MovementBiasHeroModel;
                foreach (var heroModelTransform in _heroModelTransforms)
                {
                    var direction = (heroModelTransform.Model.Position - _heroesGroupCenterPoint).normalized;
                    var dotValue = Vector2.Dot(direction, _movementInput.Input);
                    if (dotValue > highestDotValue && !heroModelTransform.Model.IsInHardCCStatus)
                    {
                        highestDotValue = dotValue;
                        newMovementBiasHeroModel = heroModelTransform.Model;
                    }
                }
                var newMovementBiasHeroModelToCenterDirection = (newMovementBiasHeroModel.Position - _heroesGroupCenterPoint).normalized;
                var oldMovementBiasHeroModelToCenterDirection = (MovementBiasHeroModel.Position - _heroesGroupCenterPoint).normalized;
                if (Vector2.Dot(newMovementBiasHeroModelToCenterDirection, oldMovementBiasHeroModelToCenterDirection) < Constant.HEROES_FORMATION_GROUP_MOVEMENT_BIAS_DOT_VALUE)
                    MovementBiasHeroModel = newMovementBiasHeroModel;
            }
            UpdateHeroesGroupCenterHolder();
        }

        private void CalculateHeroesGroupExploitSize()
        {
            int numberOfHeroes = _heroModelTransforms.Count;
            // Since this is a formula, it can be acceptable to be hard-coded.
            _heroesGroupExploitRadius = 3.0f * (0.15f * (numberOfHeroes - 1) + 1);
            UpdateTargetObjectDetectionRange();
        }

        private void CalculateHeroesGroupFormationSize()
        {
            int numberOfHeroes = _heroModelTransforms.Count;
            // Since this is a formula, it can be acceptable to be hard-coded.
            _heroesGroupFormationRadius = 0.5f * (0.5f * (numberOfHeroes - 1) + 1);
            UpdateTargetEnemyDetectionRange();
        }

        private void UpdateHeroesGroupCenterHolder()
        {
            if (!DataManager.Transitioned.IsOnCamping)
            {
                _heroesGroupCenterHolder.position = _heroesGroupCenterPoint;
            }
        }

        private partial void InitTargetsDetector();
        private partial void UpdateTargetEnemyDetectionRange();
        private partial void UpdateTargetObjectDetectionRange();

        #endregion Class Methods
    }
}