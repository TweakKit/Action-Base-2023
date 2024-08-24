using System.Collections.Generic;
using UnityEngine;
using Runtime.Common.Singleton;
using Runtime.Gameplay.Manager;
using Runtime.Gameplay.EntitySystem;
using Cysharp.Threading.Tasks;

namespace Runtime.Manager.Proximity
{
    public class ProximityManager : MonoSingleton<ProximityManager>
    {
        #region Members

        [SerializeField]
        private float _evaluationFrequency = 0.5f;
        private float _enableDistance = 20.0f;
        private float _lastEvaluationAt = 0.0f;
        private Transform _proximityTarget;
        private List<IEntity> _proximityManagedEntities;

        private Vector3 _positionCheckSpawn = Vector3.zero;
        private bool _isCheckPosition = false;
        private readonly float _defaultEnableDistance = 20.0f;
        private List<IEntity> _cachedCurrentShowedEntities;

        #endregion Members

        #region API Methods

        protected override void Awake()
        {
            base.Awake();
            _proximityManagedEntities = new List<IEntity>();
        }

        private void Update()
            => EvaluateDistance();

        #endregion API Methods

        #region Class Methods

        public virtual void Add(IEntity proximityManagedEntity)
            => _proximityManagedEntities.Add(proximityManagedEntity);

        public virtual void Remove(IEntity proximityManagedEntity)
            => _proximityManagedEntities.Remove(proximityManagedEntity);

        public void HandleHeroesSpawned()
        {
            var mainHeroTransform = EntitiesManager.Instance.MainHeroTransform;
            _proximityTarget = mainHeroTransform;
        }

        public void HandleHeroDied()
        {
            var mainHeroTransform = EntitiesManager.Instance.MainHeroTransform;
            if (mainHeroTransform != _proximityTarget)
                _proximityTarget = mainHeroTransform;
        }

        public void HandleHeroesMoveIntoBase()
        {
            var initialLeaderHeroTransform = EntitiesManager.Instance.MainHeroTransform;
            if (initialLeaderHeroTransform != _proximityTarget)
                _proximityTarget = initialLeaderHeroTransform;
        }

        private void EvaluateDistance()
        {
            if (_proximityTarget == null)
                return;

            if (_isCheckPosition)
                return;

            if (UnityEngine.Time.time - this._lastEvaluationAt > this._evaluationFrequency)
                this._lastEvaluationAt = UnityEngine.Time.time;
            else
                return;

            foreach (IEntity proximityManagedEntity in _proximityManagedEntities)
            {
                float distance = Vector2.SqrMagnitude(proximityManagedEntity.Position - _proximityTarget.position);
                if (proximityManagedEntity.IsActive && (distance > _enableDistance * _enableDistance))
                    proximityManagedEntity.SetActive(false);
                else if (!proximityManagedEntity.IsActive && (distance <= _enableDistance * _enableDistance))
                    proximityManagedEntity.SetActive(true);
            }
        }

        public void AddConditionPositionCheckVisible(Vector3 position, bool isCheckPosition, bool isCustomEnableDistance = false, float valueCustomEnableDistance = 20.0f)
        {
            _enableDistance = isCustomEnableDistance ? valueCustomEnableDistance : this._defaultEnableDistance;
            _positionCheckSpawn = position;
            _isCheckPosition = isCheckPosition;
            if (isCheckPosition)
            {
                CacheCurrentShowedEntities();
                CheckActiveAroundAdditionTargetPoint().Forget();
            }
        }

        public async UniTask CheckActiveAroundAdditionTargetPoint()
        {
            while (_isCheckPosition)
            {
                foreach (IEntity proximityManagedEntity in _proximityManagedEntities)
                {
                    if (!_cachedCurrentShowedEntities.Contains(proximityManagedEntity))
                    {
                        float distance = Vector2.SqrMagnitude(proximityManagedEntity.Position - _positionCheckSpawn);
                        if (proximityManagedEntity.IsActive)
                        {
                            if (distance > _enableDistance * _enableDistance)
                            {
                                proximityManagedEntity.SetActive(false);
                                proximityManagedEntity.SetAnimationUnscaled(false);
                            }
                            else proximityManagedEntity.SetAnimationUnscaled(true);
                        }
                        else if (!proximityManagedEntity.IsActive)
                        {
                            if (distance <= _enableDistance * _enableDistance)
                            {
                                proximityManagedEntity.SetActive(true);
                                proximityManagedEntity.SetAnimationUnscaled(true);
                            }
                            else proximityManagedEntity.SetAnimationUnscaled(false);
                        }
                    }
                    else
                    {
                        if (!proximityManagedEntity.IsActive)
                        {
                            proximityManagedEntity.SetActive(true);
                            proximityManagedEntity.SetAnimationUnscaled(true);
                        }
                    }
                }
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
            foreach (IEntity proximityManagedEntity in _proximityManagedEntities)
            {
                if (proximityManagedEntity.IsActive)
                    proximityManagedEntity.SetAnimationUnscaled(false);
            }
            _cachedCurrentShowedEntities = null;
        }

        private void CacheCurrentShowedEntities()
        {
            _cachedCurrentShowedEntities = new List<IEntity>();
            foreach (var proximityManagedEntity in _proximityManagedEntities)
            {
                if (proximityManagedEntity.IsActive)
                {
                    proximityManagedEntity.SetAnimationUnscaled(true);
                    _cachedCurrentShowedEntities.Add(proximityManagedEntity);
                }
            }
        }

        #endregion Class Methods
    }
}