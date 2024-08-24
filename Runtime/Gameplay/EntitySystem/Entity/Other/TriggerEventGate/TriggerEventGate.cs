using UnityEngine;
using Runtime.Message;
using Runtime.Extensions;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class TriggerEventGate : MonoBehaviour
    {
        #region Members

        [SerializeField]
        private GateEventTriggeredType _gateEventTriggeredType;
        private float _enterDotValue;
        private bool _hasCompletedTriggerExit;

        #endregion Members

        #region API Methods

        private void Awake()
            => _hasCompletedTriggerExit = true;

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (_hasCompletedTriggerExit)
            {
                var interractable = collider.gameObject.GetComponent<IInteractable>();
                if (interractable != null && interractable.IsMainHero)
                {
                    TriggerEnterWithMainHero(interractable);
                    _hasCompletedTriggerExit = false;
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            var interractable = collider.gameObject.GetComponent<IInteractable>();
            if (interractable != null && interractable.IsMainHero)
            {
                TriggerExitWithMainHero(interractable);
                _hasCompletedTriggerExit = true;
            }
        }

        #endregion API Methods

        #region Class Methods

        protected virtual void TriggerEnterWithMainHero(IInteractable interractable)
        {
            var heroMainPosition = interractable.Model.Position;
            _enterDotValue = GetDotValue(heroMainPosition);
            Messenger.Publish(new GateEventTriggeredMessage(_gateEventTriggeredType, true, false));
        }

        protected virtual void TriggerExitWithMainHero(IInteractable interractable)
        {
            var heroMainPosition = interractable.Model.Position;
            var exitDotValue = GetDotValue(heroMainPosition);
            var isRevertingDirection = IsRevertingDirection(exitDotValue);
            Messenger.Publish(new GateEventTriggeredMessage(_gateEventTriggeredType, false, !isRevertingDirection));
        }

        private float GetDotValue(Vector3 targetPosition)
        {
            var transformDirectionAngle = transform.eulerAngles.z;
            var transformDirection = transformDirectionAngle.ToDirection();
            var targetToTransformCenterDirection = (transform.position - targetPosition).normalized;
            var dotValue = Vector2.Dot(transformDirection, targetToTransformCenterDirection);
            return dotValue;
        }

        private bool IsRevertingDirection(float exitDotValue)
        {
            return Mathf.Sign(_enterDotValue) > 0 && Mathf.Sign(exitDotValue) > 0 ||
                   Mathf.Sign(_enterDotValue) < 0 && Mathf.Sign(exitDotValue) < 0;
        }

        #endregion Class Methods
    }
}