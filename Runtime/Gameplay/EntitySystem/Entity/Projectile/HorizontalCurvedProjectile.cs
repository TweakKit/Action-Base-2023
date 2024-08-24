using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Runtime.Audio;
using Runtime.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Runtime.Gameplay.EntitySystem
{
    /// <summary>
    /// This entity simulates the behavior of a curved projectile.<br/>
    /// </summary>
    public class HorizontalCurvedProjectile : Projectile
    {
        #region Members

        [SerializeField] protected float minCurveAmount = 0.5f; // Độ cong tối thiểu của quỹ đạo
        [SerializeField] protected float maxCurveAmount = 5; // Độ cong tối đa của quỹ đạo
        [SerializeField] protected float minFrequency = 0.5f; // Tần số sóng tối thiểu
        [SerializeField] protected float maxFrequency = 5; // Tần số sóng tối đa
        [SerializeField] protected float minAmplitude = 0.5f; // Độ lớn sóng tối thiểu
        [SerializeField] protected float maxAmplitude = 5; // Độ lớn sóng tối đa
        [SerializeField] protected float acceleration = 1;
        [SerializeField] protected float flyDuration;
        [SerializeField] protected string soundTrigger;
        [SerializeField] protected string soundImpact;
        [SerializeField] private float _rotationSpeed = 100f;
        [SerializeField] private bool _isIntro;

        [SerializeField] [ShowIf(nameof(_isIntro))]
        protected float maxOffsetPos = 1.5f;

        [SerializeField] [ShowIf(nameof(_isIntro))]
        protected float minOffsetPos = 0.5f;

        private float _currentDurationIntro = 0;
        protected float curveAmount = 1f; // Độ cong của quỹ đạo
        protected float frequency = 1f; // Tần số sóng
        protected float amplitude = 1f; // Độ lớn sóng
        protected Vector2 targetPosition;
        protected float currentFlyTime;
        private bool _turnRandom;
        private float _velocity;
        private bool _isOnIntro;

        #endregion Members

        #region API Methods

        protected override void Update()
        {
            if (!_isOnIntro)
            {
                if (currentFlyTime < flyDuration)
                {
                    Vector2 direction = (interactableTarget != null && !interactableTarget.IsDead)
                        ? (interactableTarget.CenterPosition - (Vector2)transform.position).normalized
                        : ((Vector2)(originalPosition + moveDirection * moveDistance) - (Vector2)transform.position).normalized;

                    float xVelocity = 0;
                    float yVelocity = 0;
                    if (_turnRandom)
                    {
                        xVelocity = moveSpeed * direction.x + curveAmount * Mathf.Cos(Time.time * frequency) * Mathf.Sign(direction.x);
                        yVelocity = moveSpeed * direction.y + amplitude * Mathf.Sin(Time.time * frequency);
                    }
                    else
                    {
                        xVelocity = moveSpeed * direction.x + curveAmount * Mathf.Sin(Time.time * frequency) * Mathf.Sign(direction.x);
                        yVelocity = moveSpeed * direction.y + amplitude * Mathf.Cos(Time.time * frequency);
                    }

                    _velocity += acceleration * Time.deltaTime;
                    transform.position += new Vector3(xVelocity, yVelocity, 0f) * Time.deltaTime * _velocity;
                    currentFlyTime += Time.deltaTime;

                    if (Vector2.SqrMagnitude(originalPosition - Position) > moveDistance * moveDistance)
                        CompleteMovement();
                }
                else CompleteMovement();

            }
                CheckRotateProjectile();
        }

        protected override void CheckRotateProjectile()
        {
            if (isRotatable)
            {
                var direction = (transform.position - previousPosition).normalized;
                if (direction == Vector3.zero) return;
                transform.up = Vector3.Slerp(transform.up, direction, Time.deltaTime * _rotationSpeed);
                previousPosition = transform.position;
            }
        }

        #endregion API Methods

        #region Class Methods

        public override void Init(float moveDistance, float damageFactorValue, DamageSource damageSource, StatusEffectModel[] damageStatusEffectModels,
                                  Vector2 moveDirection, IInteractable interactableTarget, Action completedMovingAction, ProjectileFeatureData projectileFeatureData = null)
        {
            base.Init(moveDistance, damageFactorValue, damageSource, damageStatusEffectModels, moveDirection, interactableTarget, completedMovingAction, projectileFeatureData);
            targetPosition = interactableTarget != null ? interactableTarget.Position : (Vector2)transform.position + moveDirection * moveDistance;
            currentFlyTime = 0.0f;
            _currentDurationIntro = 0.0f;
            this.interactableTarget = interactableTarget;
            curveAmount = Random.Range(minCurveAmount, maxCurveAmount);
            frequency = Random.Range(minFrequency, maxFrequency);
            amplitude = Random.Range(minAmplitude, maxAmplitude);
            _turnRandom = Random.Range(0, 2) == 0 ? true : false;
            _velocity = 0.5f;
            PlaySoundTrigger(this.GetCancellationTokenOnDestroy());
            if (_isIntro)
                Intro();
            else
                _isOnIntro = false;
        }

        public override void Init(float moveDistance, float damageFactorValue, DamageSource damageSource, StatusEffectModel[] damageStatusEffectModels,
                                  Vector2 moveDirection, Vector2 positionTarget, Action completedMovingAction, ProjectileFeatureData projectileFeatureData = null)
        {
            base.Init(moveDistance, damageFactorValue, damageSource, damageStatusEffectModels, moveDirection, positionTarget, completedMovingAction, projectileFeatureData);
            targetPosition = positionTarget;
            currentFlyTime = 0.0f;
            _currentDurationIntro = 0.0f;
            curveAmount = Random.Range(minCurveAmount, maxCurveAmount);
            frequency = Random.Range(minFrequency, maxFrequency);
            amplitude = Random.Range(minAmplitude, maxAmplitude);
            _turnRandom = Random.Range(0, 2) == 0 ? true : false;
            _velocity = 0.5f;
            PlaySoundTrigger(this.GetCancellationTokenOnDestroy());
            if (_isIntro)
                Intro();
            else
                _isOnIntro = false;
        }

        private void Intro()
        {
            if (_isIntro)
            {
                _isOnIntro = true;

                GetComponentInChildren<SpriteRenderer>().sortingOrder = 0;
                transform.rotation = Vector3.up.ToQuaternion();

                var position = transform.position;
                var spawnPosition = new Vector2(position.x + (Random.Range(-0.5f, 0.5f)), position.y);
                float rangeRandom1 = Random.Range(-maxOffsetPos, maxOffsetPos);
                float rangeRandom2 = Random.Range(0, maxOffsetPos);
                var targetX = spawnPosition.x + (rangeRandom1 > 0 ? minOffsetPos + rangeRandom1 : -minOffsetPos + rangeRandom1);
                var targetY = spawnPosition.y + rangeRandom2;
                var targetVector = new Vector2(targetX, targetY);
                var range = Vector2.Distance(targetVector, transform.position);
                var duration = 0.7f;
                transform.DOMove(targetVector, duration).SetEase(Ease.InQuad).OnComplete(() => {
                    _isOnIntro = false;
                    GetComponentInChildren<SpriteRenderer>().sortingOrder = 100;
                }).SetUpdate(true).SetAutoKill(true);
            }
        }

        protected override void DamageTarget(IInteractable interactable)
        {
            base.DamageTarget(interactable);
            PlaySoundImpact(this.GetCancellationTokenOnDestroy());
        }

        private void PlaySoundTrigger(CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(soundTrigger))
            {
                AudioController.Instance.PlaySoundEffectAsync(soundTrigger, cancellationToken).Forget();
            }
        }

        private void PlaySoundImpact(CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(this.soundImpact))
            {
                AudioController.Instance.PlaySoundEffectAsync(soundImpact, cancellationToken).Forget();
            }
        }

        #endregion Class Methods
    }
}