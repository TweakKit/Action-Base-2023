using UnityEngine;
using Runtime.Navigation;
using Runtime.Common.Singleton;
using Runtime.Gameplay.Map;
using Runtime.Manager.Data;

namespace Runtime.Gameplay.Manager
{
    public class NavigationManager : MonoSingleton<NavigationManager>
    {
        #region Members

        [SerializeField]
        private CustomNavigationMap _navigationMap;
        [SerializeField]
        private BoxCollider2D _navigationBoundCollider;
        [SerializeField]
        [Min(0.1f)]
        private float _targetDistanceThreshold;
        private Transform _targetHeroTransform;
        private bool _canUpdate;
        private Vector3 _lastCheckPosition;
        private Vector2 _navigationBoundTopRightPoint;
        private Vector2 _navigationBoundBottomLeftPoint;

        #endregion Members

        #region API Methods

        protected override void Awake()
        {
            base.Awake();
            _lastCheckPosition = _navigationMap.transform.position;
        }

        private void Update()
        {
            if (_canUpdate)
            {
                if (!DataManager.Transitioned.IsOnCamping)
                {
                    var regenerateNavigationMap = CheckRegenerateNavigationMap();
                    if (regenerateNavigationMap)
                        GenerateNavigationAtTargetHero();
                }
            }
        }

        #endregion API Methods

        #region Class Methods

        public void HandleHeroesSpawned()
        {
            if (_targetHeroTransform == null)
            {
                _canUpdate = true;
                _targetHeroTransform = EntitiesManager.Instance.MainHeroTransform;
                GenerateNavigationAtTargetHero();
            }
            else CheckToGenerateNavgitionMap();
        }

        public void TriggerUpdate()
            => GenerateNavigationAtTargetHero();

        public void HandleHeroDied()
                => CheckToGenerateNavgitionMap();

        public void HandleHeroesMoveIntoBase()
            => CheckToGenerateNavgitionMap();

        public CustomNavigationMap GetNavigationMap(Vector2 checkedPosition)
        {
            var isPositionValid = IsInNavigationBound(checkedPosition);
            if (isPositionValid)
                return _navigationMap;
            else
                return null;
        }

        public bool IsInNavigationBound(Vector2 pointToCheck)
        {
            return pointToCheck.x > _navigationBoundBottomLeftPoint.x &&
                   pointToCheck.x < _navigationBoundTopRightPoint.x &&
                   pointToCheck.y > _navigationBoundBottomLeftPoint.y &&
                   pointToCheck.y < _navigationBoundTopRightPoint.y;
        }

        public bool IsWalkable(Vector2 position)
        {
            var navigationMap = GetNavigationMap(position);
            return navigationMap != null && navigationMap.IsPointValid(position);
        }

        public void GenerateNavigationAtPosition(Vector2 position)
            => GenerateNavigationMap(position);

        public void GenerateNavigationAtTargetHero()
            => GenerateNavigationMap(_targetHeroTransform.position);

        private void GenerateNavigationMap(Vector2 position)
        {
            _lastCheckPosition = position;
            _navigationMap.transform.position = _lastCheckPosition;
            _navigationMap.ClearObstacles();
            var navigationObstacles = MapManager.Instance.GetNavigationObstacles();
            foreach (var navigationObstacle in navigationObstacles)
            {
                if (!navigationObstacle.isDisabled)
                    _navigationMap.AddObstacle(navigationObstacle);
            }
            _navigationMap.Initialize();
            InitNavigationBoundPoints();
        }

        private bool CheckRegenerateNavigationMap()
        {
            var sqrDistance = Vector2.SqrMagnitude(_targetHeroTransform.position - _lastCheckPosition);
            return sqrDistance >= _targetDistanceThreshold * _targetDistanceThreshold;
        }

        private void InitNavigationBoundPoints()
        {
            var colliderCenter = (Vector2)_navigationBoundCollider.transform.position + _navigationBoundCollider.offset;
            var colliderHalfSize = _navigationBoundCollider.size * 0.5f;
            _navigationBoundTopRightPoint = new Vector2(colliderCenter.x + colliderHalfSize.x, colliderCenter.y + colliderHalfSize.y);
            _navigationBoundBottomLeftPoint = new Vector2(colliderCenter.x - colliderHalfSize.x, colliderCenter.y - colliderHalfSize.y);
        }

        private void CheckToGenerateNavgitionMap()
        {
            _targetHeroTransform = EntitiesManager.Instance.MainHeroTransform;
            if (_targetHeroTransform != null)
            {
                var regenerateNavigationMap = CheckRegenerateNavigationMap();
                if (regenerateNavigationMap)
                {
                    _canUpdate = true;
                    GenerateNavigationAtTargetHero();
                }
            }
            else _canUpdate = false;
        }

        #endregion Class Methods
    }
}