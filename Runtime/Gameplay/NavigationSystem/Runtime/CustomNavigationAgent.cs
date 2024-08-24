using System.Collections.Generic;
using UnityEngine;
using Runtime.Gameplay.EntitySystem;
using Runtime.Definition;
using Runtime.Gameplay.Manager;

namespace Runtime.Navigation
{
    [DisallowMultipleComponent]
    public class CustomNavigationAgent : MonoBehaviour
    {
        #region Members

        [Header("--- STEERING ---")]
        [Tooltip("The max speed.")]
        public float maxSpeed = 3.5f;

        [Tooltip("The max steering force applied. Works like acceleration.")]
        public float maxForce = 10f;

        [Tooltip("The distance to stop at from the end point.")]
        public float stoppingDistance = 0.1f;

        [Tooltip("The distance to start slowing down.")]
        public float slowingDistance = 1;

        [Tooltip("The lookahead distance for slowing down and agent avoidance. Set to 0 to eliminate the slowdown but the avoidance too, as well as increase performance.")]
        public float lookAheadDistance = 1;

        [Header("--- AVOIDANCE ---")]
        [Tooltip("The avoidance radius of the agent. 0 for no avoidance.")]
        public float avoidRadius = 0;

        [Tooltip("The max time in seconds where the agent is actively avoiding before considered stuck.")]
        public float avoidanceConsiderStuckedTime = 3f;

        [Tooltip("The max remaining path distance which will be considered reached, when the agent is stuck.")]
        public float avoidanceConsiderReachedDistance = 1f;

#if UNITY_EDITOR
        [Header("--- DEBUGGING ---")]
        [Tooltip("Will debug the path (gizmos). Disable for performance.")]
        public bool debugPath = true;
#endif

        private Vector2 _lastValidCloserEdgePoint;
        private Vector2 _currentVelocity = Vector2.zero;
        private int _requests = 0;
        private List<Vector2> _activePath = new List<Vector2>();
        private bool _isAvoiding;
        private float _avoidingElapsedTime;
        private EntityModel _ownerEntityModel;
        private EntityType _entityType;
        private static List<CustomNavigationAgent> s_allAgents = new List<CustomNavigationAgent>();

        #endregion Members

        #region Properties

        public bool IsInNavigationBound
        {
            get
            {
                return NavigationManager.Instance.IsInNavigationBound(Position);
            }
        }

        public bool HasPath
        {
            get
            {
                return ActivePath.Count > 0;
            }
        }

        public float RemainingDistance
        {
            get
            {
                if (!HasPath)
                    return 0;

                float distance = Vector2.Distance(Position, ActivePath[0]);
                for (int i = 0; i < ActivePath.Count; i++)
                    distance += Vector2.Distance(ActivePath[i], ActivePath[i == ActivePath.Count - 1 ? i : i + 1]);
                return distance;
            }
        }

        public Vector2 DestinationPoint
        {
            get
            {
                return HasPath ? ActivePath[^1] : Position;
            }
        }

        public Vector2 MovingDirection
        {
            get { return HasPath ? _currentVelocity.normalized : Vector2.zero; }
        }

        private Vector2 Position
        {
            get
            {
                return _ownerEntityModel.Position;
            }
            set
            {
                transform.position = value;
                _ownerEntityModel.Position = value;
            }
        }

        private List<Vector2> ActivePath
        {
            get
            {
                return _activePath;
            }
            set
            {
                _activePath = value;
                if (_activePath.Count > 0 && _activePath[0] == Position)
                    _activePath.RemoveAt(0);
            }
        }

        private Vector2 NextPoint
        {
            get
            {
                return HasPath ? ActivePath[0] : Position;
            }
        }

        private bool IsAreaRestricted
        {
            get
            {
                return _entityType == EntityType.Hero;
            }
        }

        private bool CanFindCloserPointOnInvalid
        {
            get
            {
                return _entityType == EntityType.Hero;
            }
        }

        private bool NeedToCheckValidStartPointInFogAreas
        {
            get
            {
                return _entityType != EntityType.Hero;
            }
        }

        private bool AlsoClaimValidStartPointInFogAreas
        {
            get
            {
                return _entityType == EntityType.Hero;
            }
        }

        private CustomNavigationMap CurrentNavigationMap
            => NavigationManager.Instance.GetNavigationMap(Position);

        #endregion Properties

        #region API Methods

        private void OnEnable()
            => s_allAgents.Add(this);

        private void OnDisable()
            => s_allAgents.Remove(this);

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!HasPath)
                return;

            if (debugPath)
            {
                Gizmos.color = new Color(1f, 1f, 1f, 0.2f);
                Gizmos.DrawLine(Position, ActivePath[0]);
                for (int i = 0; i < ActivePath.Count; i++)
                    Gizmos.DrawLine(ActivePath[i], ActivePath[(i == ActivePath.Count - 1) ? i : i + 1]);
            }
        }
#endif

        #endregion API Methods

        #region Class Methods

        /// <summary>
        /// Return true means whether the agent can be updated continually or not.
        /// </summary>
        /// <returns></returns>
        public bool UpdateAgent()
        {
            // When there is no path just restrict.
            if (!HasPath)
                return true;

            if (maxSpeed <= 0)
                return false;

            var targetVelocity = _currentVelocity;
            if (RemainingDistance < slowingDistance)
                targetVelocity += Arrive(NextPoint);
            else
                targetVelocity += Seek(NextPoint);

            // Move the agent.
            _currentVelocity = Vector2.MoveTowards(_currentVelocity, targetVelocity, maxForce * Time.deltaTime);
            _currentVelocity = Vector2.ClampMagnitude(_currentVelocity, maxSpeed);

            // Slow down if wall ahead and avoid other agents.
            LookAhead();

            Position += _currentVelocity * Time.deltaTime;

            // Check active avoidance elapsed time (= stuck).
            if (_isAvoiding && _avoidingElapsedTime >= avoidanceConsiderStuckedTime)
            {
                if (RemainingDistance > avoidanceConsiderReachedDistance)
                    OnInvalid();
                else
                    OnArrived();
            }

            // Restrict just after movement.
            Restrict();

            // Check and remove if we reached a point. Proximity distance depends.
            if (HasPath)
            {
                float proximity = ActivePath[ActivePath.Count - 1] == NextPoint ? stoppingDistance : 0.001f;
                if ((Position - NextPoint).magnitude <= proximity)
                {
                    ActivePath.RemoveAt(0);

                    // If it was last point, means the path is complete and no longer have an active path.
                    if (!HasPath)
                    {
                        OnArrived();
                        return true;
                    }
                }
            }

            if (IsInNavigationBound)
            {
                // Little trick: Check the next waypoint ahead of the current for LOS and if true consider the current reached.
                // Helps for tight corners and when agent has big innertia.
                if (ActivePath.Count > 1 && CurrentNavigationMap.CheckLOS(Position, ActivePath[1]))
                    ActivePath.RemoveAt(0);

                return true;
            }
            else return false;
        }

        public void SetOwnerModel(EntityModel entityModel)
        {
            _ownerEntityModel = entityModel;
            _entityType = _ownerEntityModel.EntityType;
        }

        public PathSetType SetDestination(Vector2 endPoint)
        {
            var startPoint = Position;

            // End point is almost the same as agent position. We consider arrived immediately.
            if ((endPoint - startPoint).magnitude < stoppingDistance)
            {
                OnArrived();
                return PathSetType.Invalid;
            }

            // Check if the start point is valid with fog areas checking.
            if (NeedToCheckValidStartPointInFogAreas && !CurrentNavigationMap.IsPointValidWithFogAreasChecking(startPoint))
            {
                OnInvalid();
                return PathSetType.Invalid;
            }

            // Check if the end point is valid.
            if (!CurrentNavigationMap.IsPointValid(endPoint))
            {
                if (CanFindCloserPointOnInvalid)
                {
                    var closerPointOnInValid = CurrentNavigationMap.GetCloserEdgePoint(endPoint, true);
                    if (closerPointOnInValid == endPoint)
                    {
                        _lastValidCloserEdgePoint = startPoint;
                        Position = _lastValidCloserEdgePoint;
                        OnInvalid();
                        return PathSetType.Invalid;
                    }
                    else return SetDestination(closerPointOnInValid);
                }
                else
                {
                    OnInvalid();
                    return PathSetType.Invalid;
                }
            }

            // If a path is pending dont calculate new path.
            if (_requests > 0)
                return PathSetType.RequestPending;

            // Compute path.
            _requests++;
            _lastValidCloserEdgePoint = endPoint;
            CurrentNavigationMap.FindPath(startPoint, endPoint, AlsoClaimValidStartPointInFogAreas, SetPath);

            return PathSetType.PathFound;
        }

        /// <summary>
        /// This function set the destination strictly, which means it's always assured that there will only be 
        /// a valid path returned if all the checks for start point, end point and the connection between them are all valid.
        /// </summary>
        public PathSetType SetDestinationStrictly(Vector2 endPoint)
        {
            var startPoint = Position;

            // End point is almost the same as agent position. We consider arrived immediately.
            if ((endPoint - startPoint).magnitude < stoppingDistance)
            {
                OnArrived();
                return PathSetType.Invalid;
            }

            // Check if the start point is valid with fog areas checking.
            if (NeedToCheckValidStartPointInFogAreas && !CurrentNavigationMap.IsPointValidWithFogAreasChecking(startPoint))
            {
                OnInvalid();
                return PathSetType.Invalid;
            }

            // Check if the end point is valid.
            if (!CurrentNavigationMap.IsPointValid(endPoint))
            {
                OnInvalid();
                return PathSetType.Invalid;
            }

            // If a path is pending dont calculate new path.
            // The prime goal will be repathed anyway.
            if (_requests > 0)
                return PathSetType.RequestPending;

            // Compute path.
            _requests++;
            _lastValidCloserEdgePoint = endPoint;
            CurrentNavigationMap.FindPath(startPoint, endPoint, AlsoClaimValidStartPointInFogAreas, SetPath);

            return PathSetType.PathFound;
        }

        public void Stop()
        {
            if (ActivePath.Count > 0)
                ActivePath.Clear();
            _currentVelocity = Vector2.zero;
            _requests = 0;
            _avoidingElapsedTime = 0;
        }

        // The callback from map for when path is ready to use.
        private void SetPath(List<Vector2> path)
        {
            // In case the agent stoped somehow, but a path was pending.
            if (_requests == 0)
                return;

            _requests--;

            if (path == null || path.Count == 0)
            {
                OnInvalid();
                return;
            }

            ActivePath = path;
        }

        // Seeking a target.
        private Vector2 Seek(Vector2 target)
        {
            var desiredVelocity = (target - Position).normalized * maxSpeed;
            var steeringVelocity = desiredVelocity - _currentVelocity;
            return steeringVelocity;
        }

        // Slowing at target's arrival.
        private Vector2 Arrive(Vector2 target)
        {
            var desiredVelocity = (target - Position).normalized * maxSpeed;
            desiredVelocity *= RemainingDistance / slowingDistance;
            var steeringVelocity = desiredVelocity - _currentVelocity;
            return steeringVelocity;
        }

        // Slowing when there is an obstacle ahead.
        private void LookAhead()
        {
            // If agent is outside dont LookAhead since that causes agent to constantly be slow.
            if (lookAheadDistance <= 0)
                return;

            var currentLookAheadDistance = Mathf.Lerp(0, lookAheadDistance, _currentVelocity.magnitude / maxSpeed);
            var lookAheadPosition = Position + _currentVelocity.normalized * currentLookAheadDistance;

            // Avoidance.
            if (avoidRadius > 0)
            {
                _isAvoiding = false;
                for (var i = 0; i < s_allAgents.Count; i++)
                {
                    var otherAgent = s_allAgents[i];
                    if (otherAgent == this || otherAgent._entityType != _entityType || otherAgent.avoidRadius <= 0)
                        continue;

                    var mlt = otherAgent.avoidRadius + this.avoidRadius;
                    var distance = (lookAheadPosition - otherAgent.Position).magnitude;
                    var str = (lookAheadPosition - otherAgent.Position).normalized * mlt;
                    var steer = Vector3.Lerp(str, Vector3.zero, distance / mlt);
                    if (!_isAvoiding)
                        _isAvoiding = steer.magnitude > 0;
                    _currentVelocity += ((Vector2)steer) * _currentVelocity.magnitude;
                }

                if (_isAvoiding)
                    _avoidingElapsedTime += Time.deltaTime;
                else
                    _avoidingElapsedTime = 0;
            }
        }

        private void OnArrived()
            => Stop();

        private void OnInvalid()
            => Stop();

        private void Restrict()
        {
            if (!IsAreaRestricted)
                return;

            if (IsInNavigationBound && !CurrentNavigationMap.IsPointValid(Position))
            {
                var closerEdgePoint = CurrentNavigationMap.GetCloserEdgePoint(Position, CanFindCloserPointOnInvalid);
                if (closerEdgePoint == Position)
                    Position = _lastValidCloserEdgePoint;
                else
                    Position = closerEdgePoint;
            }
        }

        #endregion Class Methods
    }
}