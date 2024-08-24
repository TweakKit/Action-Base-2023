using System.Threading;
using Runtime.Audio;
using Runtime.Definition;
using Cysharp.Threading.Tasks;

namespace Runtime.Gameplay.EntitySystem
{
    public class RangedInteractAttackStrategy : RangedAttackStrategy
    {
        #region Members

        private float _originalChopAttackAnimationTime;
        private float _originalMiningAttackAnimationTime;
        private float _chopAttackAnimationSpeedMultiplier;
        private float _miningAttackAnimationSpeedMultiplier;
        private float _chopAttackCooldownTime;
        private float _miningAttackCooldownTime;

        #endregion Members

        #region Class Methods

        public override void Init(CharacterModel ownerCharacterModel, UnityEngine.Transform ownerCharacterTransform)
        {
            base.Init(ownerCharacterModel, ownerCharacterTransform);
            if (characterWeaponActionPlayer != null)
            {
                _originalChopAttackAnimationTime = characterWeaponActionPlayer.GetAnimationTime(CharacterWeaponAnimationType.ChopAttack);
                var chopAttackSpeed = ownerCharacterModel.GetTotalStatValue(StatType.ChopSpeed);
                InitAttackRelatedValues(out _chopAttackAnimationSpeedMultiplier, out _chopAttackCooldownTime, chopAttackSpeed, _originalChopAttackAnimationTime);

                _originalMiningAttackAnimationTime = characterWeaponActionPlayer.GetAnimationTime(CharacterWeaponAnimationType.MiningAttack);
                var miningAttackSpeed = ownerCharacterModel.GetTotalStatValue(StatType.MiningSpeed);
                InitAttackRelatedValues(out _miningAttackAnimationSpeedMultiplier, out _miningAttackCooldownTime, miningAttackSpeed, _originalMiningAttackAnimationTime);
            }
        }

        protected override async UniTask TriggerAttackAsync(CancellationToken cancellationToken)
        {
            var isPlayingAnimation = true;
            if (ownerCharacterModel.CurrentAttackedTarget.IsCharacter)
            {
                var characterWeaponAttackPlayedData = new CharacterWeaponPlayedData
                (
                    animationType: CharacterWeaponAnimationType.RangedAttack,
                    speedMultiplier: normalAttackAnimationSpeedMultiplier,
                    operatedPointTriggeredCallbackAction: () => {
                        TriggerAttackPointOperation();
                        PlaySoundRangeAttack(cancellationToken);
                        FireProjectileTowardsTarget(ownerCharacterModel.CurrentAttackedTarget, cancellationToken);
                    },
                    endActionCallbackAction: () => {
                        isPlayingAnimation = false;
                    }
                );
                characterWeaponActionPlayer.Play(characterWeaponAttackPlayedData);
            }
            else if (ownerCharacterModel.CurrentAttackedTarget.IsObjectTree)
            {
                var characterWeaponAttackPlayedData = new CharacterWeaponPlayedData
                (
                    animationType: CharacterWeaponAnimationType.ChopAttack,
                    speedMultiplier: _chopAttackAnimationSpeedMultiplier,
                    operatedPointTriggeredCallbackAction: () => {
                        TriggerAttackPointOperation();
                        AudioController.Instance.PlaySoundEffectAsync(AudioConstants.GENERAL_CHOP, cancellationToken).Forget();
                        DamageToObject();
                    },
                    endActionCallbackAction: () => {
                        isPlayingAnimation = false;
                    }
                );
                characterWeaponActionPlayer.Play(characterWeaponAttackPlayedData);
            }
            else if (ownerCharacterModel.CurrentAttackedTarget.IsObjectCrystal)
            {
                var characterWeaponAttackPlayedData = new CharacterWeaponPlayedData
                (
                    animationType: CharacterWeaponAnimationType.MiningAttack,
                    speedMultiplier: _miningAttackAnimationSpeedMultiplier,
                    operatedPointTriggeredCallbackAction: () => {
                        TriggerAttackPointOperation();
                        AudioController.Instance.PlaySoundEffectAsync(AudioConstants.GENERAL_MINE_EMERALD, cancellationToken).Forget();
                        DamageToObject();
                    },
                    endActionCallbackAction: () => {
                        isPlayingAnimation = false;
                    }
                );
                characterWeaponActionPlayer.Play(characterWeaponAttackPlayedData);
            }
            await UniTask.WaitUntil(() => !isPlayingAnimation, cancellationToken: cancellationToken);
        }

        protected override void InitAttackCooldownTime()
        {
            if (ownerCharacterModel.CurrentAttackedTarget.IsObjectTree)
                attackCooldownTime = _chopAttackCooldownTime;
            else if (ownerCharacterModel.CurrentAttackedTarget.IsObjectCrystal)
                attackCooldownTime = _miningAttackCooldownTime;
            else
                base.InitAttackCooldownTime();
        }

        protected override void OnStatChangedOnAttack(StatType statType, float updatedValue)
        {
            if (statType == StatType.ChopSpeed)
            {
                var chopAttackSpeed = updatedValue;
                InitAttackRelatedValues(out _chopAttackAnimationSpeedMultiplier, out _chopAttackCooldownTime, chopAttackSpeed, _originalChopAttackAnimationTime);
            }
            else if (statType == StatType.MiningSpeed)
            {
                var miningAttackSpeed = updatedValue;
                InitAttackRelatedValues(out _miningAttackAnimationSpeedMultiplier, out _miningAttackCooldownTime, miningAttackSpeed, _originalMiningAttackAnimationTime);
            }
            else base.OnStatChangedOnAttack(statType, updatedValue);
        }

        protected virtual void PlaySoundRangeAttack(CancellationToken cancellationToken)
        {
            AudioController.Instance.PlaySoundEffectAsync(AudioConstants.GENERAL_ATK_RANGE, cancellationToken).Forget();
        }

        private void DamageToObject()
        {
            var finalDamage = 0.0f;
            if (ownerCharacterModel.CurrentAttackedTarget.IsObjectTree)
                finalDamage = ownerCharacterModel.GetTotalStatValue(StatType.ChopDamage);
            else if (ownerCharacterModel.CurrentAttackedTarget.IsObjectCrystal)
                finalDamage = ownerCharacterModel.GetTotalStatValue(StatType.MiningDamage);
            var damageInfo = new DamageInfo(DamageSource.FromInteraction, finalDamage, null, ownerCharacterModel, ownerCharacterModel.CurrentAttackedTarget.Model);
            var damageDirection = (ownerCharacterModel.CurrentAttackedTarget.Position - ownerCharacterModel.Position).normalized;
            ownerCharacterModel.CurrentAttackedTarget.GetHit(damageInfo, damageDirection);
        }

        #endregion Class Methods
    }
}