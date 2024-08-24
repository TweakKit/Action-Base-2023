using System;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class EntityTargetDetector : MonoBehaviour
    {
        #region Properties

        private Action<IInteractable> TargetEnteredAction { get; set; } = null;
        private Action<IInteractable> TargetExitedAction { get; set; } = null;
        private Action<Collider2D> ColliderEnteredAction { get; set; } = null;
        private Action<Collider2D> ColliderExitedAction { get; set; } = null;

        #endregion Properties

        #region API Methods

        private void OnTriggerEnter2D(Collider2D collider)
        {
            ColliderEnteredAction?.Invoke(collider);
            var interractable = collider.GetComponent<IInteractable>();
            if (interractable != null && !interractable.Model.IsDead)
                TargetEnteredAction?.Invoke(interractable);
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            ColliderExitedAction?.Invoke(collider);
            var interractable = collider.GetComponent<IInteractable>();
            if (interractable != null)
                TargetExitedAction?.Invoke(interractable);
        }

        #endregion API Methods

        #region Class Methods

        /// <summary>
        /// For the collider whose shape is manually configured, meaning there is no calculation at runtime
        /// taken into account for the shape properties.
        /// <param name="targetEnteredAction">Action callback fired when a target has entered the collider.</param>
        /// <param name="targetExitedAction">Action callback fired when a target has exited the collider.</param>
        /// </summary>
        public void Init(Action<IInteractable> targetEnteredAction = null, Action<IInteractable> targetExitedAction = null,
                         Action<Collider2D> colliderEnteredAction = null, Action<Collider2D> colliderExitedAction = null)
        {
            TargetEnteredAction = targetEnteredAction;
            TargetExitedAction = targetExitedAction;
            ColliderEnteredAction = colliderEnteredAction;
            ColliderExitedAction = colliderExitedAction;
        }

        /// <summary>
        /// For the collider whose shape is circle, pass in the detect range as the detect diameter for the collider.
        /// </summary>
        /// <param name="detectRange">The detect diameter.</param>
        /// <param name="targetEnteredAction">Action callback fired when a target has entered the collider.</param>
        /// <param name="targetExitedAction">Action callback fired when a target has exited the collider.</param>
        /// </summary>
        public void Init(float detectRange, Action<IInteractable> targetEnteredAction, Action<IInteractable> targetExitedAction,
                        Action<Collider2D> colliderEnteredAction = null, Action<Collider2D> colliderExitedAction = null)
        {
            Init(targetEnteredAction, targetExitedAction, colliderEnteredAction, colliderExitedAction);
            var circleCollider = gameObject.GetComponent<CircleCollider2D>();
            circleCollider.radius = detectRange;
        }

        public void UpdateDetectRange(float newDetectRange)
        {
            var circleCollider = gameObject.GetComponent<CircleCollider2D>();
            circleCollider.radius = newDetectRange;
        }

        public void Enable() => gameObject.SetActive(true);
        public void Disable() => gameObject.SetActive(false);

        #endregion Class Methods
    }
}