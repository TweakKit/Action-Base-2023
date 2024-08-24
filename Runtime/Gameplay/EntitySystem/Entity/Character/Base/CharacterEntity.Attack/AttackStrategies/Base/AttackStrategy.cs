using System;
using System.Threading;
using UnityEngine;
using Runtime.Definition;
using Runtime.Audio;
using Runtime.Extensions;
using Cysharp.Threading.Tasks;

namespace Runtime.Gameplay.EntitySystem
{
    public abstract class AttackStrategy : IAttackStrategy
    {
        #region Members

        protected bool isReady;
        protected bool hasOperatedAttackPoint;
        protected float originalNormalAttackAnimationTime;
        protected float normalAttackAnimationSpeedMultiplier;
        protected float normalAttackCooldownTime;
        protected float attackCooldownTime;
        protected CharacterModel ownerCharacterModel;
        protected ICharacterWeaponActionPlayer characterWeaponActionPlayer;
        protected CancellationTokenSource executeAttackCancellationTokenSource;

        #endregion Members

        #region Class Methods

        public virtual void Init(CharacterModel ownerCharacterModel, Transform ownerCharacterTransform)
        {
            characterWeaponActionPlayer = ownerCharacterTransform.GetComponentInChildren<ICharacterWeaponActionPlayer>(true);
            if (characterWeaponActionPlayer != null)
            {
                characterWeaponActionPlayer.Init();
                originalNormalAttackAnimationTime = GetOriginalNormalAttackAnimationTime();
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogError($"Require a character weapon animation for this behavior!");
                return;
            }
#else
            else return;
#endif
            this.ownerCharacterModel = ownerCharacterModel;
            var attackSpeed = ownerCharacterModel.GetTotalStatValue(StatType.AttackSpeed);
            InitAttackRelatedValues(out normalAttackAnimationSpeedMultiplier, out normalAttackCooldownTime, attackSpeed, originalNormalAttackAnimationTime);
            isReady = true;
            hasOperatedAttackPoint = false;
            executeAttackCancellationTokenSource = new CancellationTokenSource();
            ownerCharacterModel.StatChangedEvent += OnStatChangedOnAttack;
        }

        public virtual bool CheckCanAttack()
           => isReady;

        public virtual async UniTask OperateAttackAsync()
        {
            isReady = false;
            hasOperatedAttackPoint = false;
            InitAttackCooldownTime();
            if (executeAttackCancellationTokenSource != null)
                await TriggerAttackAsync(executeAttackCancellationTokenSource.Token);
            RunAttackCooldownAsync().Forget();
        }

        public void Dispose()
        {
            if (executeAttackCancellationTokenSource != null)
            {
                executeAttackCancellationTokenSource.Cancel();
                executeAttackCancellationTokenSource.Dispose();
                executeAttackCancellationTokenSource = null;
            }
        }

        public bool Cancel()
        {
            executeAttackCancellationTokenSource.Cancel();
            executeAttackCancellationTokenSource.Dispose();
            executeAttackCancellationTokenSource = new CancellationTokenSource();
            if (hasOperatedAttackPoint)
            {
                isReady = false;
                RunAttackCooldownAsync().Forget();
                return true;
            }
            else
            {
                isReady = true;
                return false;
            }
        }

        protected abstract float GetOriginalNormalAttackAnimationTime();
        protected abstract UniTask TriggerAttackAsync(CancellationToken cancellationToken);

        protected virtual void TriggerAttackPointOperation()
            => hasOperatedAttackPoint = true;

        protected virtual void InitAttackCooldownTime()
            => attackCooldownTime = normalAttackCooldownTime;

        protected virtual void OnStatChangedOnAttack(StatType statType, float updatedValue)
        {
            if (statType == StatType.AttackSpeed)
            {
                var attackSpeed = updatedValue;
                InitAttackRelatedValues(out normalAttackAnimationSpeedMultiplier, out normalAttackCooldownTime, attackSpeed, originalNormalAttackAnimationTime);
            }
        }

        protected virtual void PlaySpecialSoundAtk(CancellationToken cancellationToken)
        {
            if (Constant.IsVisualBoss(ownerCharacterModel.EntityVisualId))
            {
                var nameSound = string.Format(AudioConstants.BOSS_ATK, ownerCharacterModel.EntityId.ToSnakeCase());
                AudioController.Instance.PlaySoundEffectAsync(nameSound, cancellationToken).Forget();
            }
        }

        protected void InitAttackRelatedValues(out float attackAnimationSpeedMultiplier, out float attackCooldownTime,
                                               float attackSpeed, float originalAttackAnimationTime)
        {
            var attackDuration = 1.0f / attackSpeed;
            attackAnimationSpeedMultiplier = attackSpeed < originalAttackAnimationTime
                                           ? 1.0f
                                           : attackSpeed;
            var attackAnimationTime = attackSpeed < originalAttackAnimationTime
                                        ? originalAttackAnimationTime
                                        : originalAttackAnimationTime * attackDuration;
            attackCooldownTime = attackDuration - attackAnimationTime;
        }

        private async UniTask RunAttackCooldownAsync()
        {
            if (executeAttackCancellationTokenSource != null)
                await UniTask.Delay(TimeSpan.FromSeconds(attackCooldownTime), cancellationToken: executeAttackCancellationTokenSource.Token);
            isReady = true;
        }

        #endregion Class Methods
    }
}