using System;
using System.Threading;
using UnityEngine;
using Runtime.Manager.Pool;
using Cysharp.Threading.Tasks;

namespace Runtime.Gameplay.EntitySystem
{
    public class MeleeSpawnDarkFlailAttackStrategy : MeleeAttackStrategy
    {
        #region Members

        private static readonly string[] s_flailPrefabNames = new string[4]
        {
            "3003_flail_up_right_attack",
            "3003_flail_up_left_attack",
            "3003_flail_down_left_attack",
            "3003_flail_down_right_attack",
        };

        #endregion Members

        #region Class Methods

        protected override async UniTask TriggerAttackAsync(CancellationToken cancellationToken)
        {
            var isPlayingAnimation = true;
            var characterWeaponPlayedData = new CharacterWeaponPlayedData
            (
                animationType: CharacterWeaponAnimationType.MeleeAttack,
                speedMultiplier: normalAttackAnimationSpeedMultiplier,
                operatedPointTriggeredCallbackAction: () =>
                {
                    TriggerAttackPointOperation();
                    DamageToTarget(cancellationToken);
                },
                endActionCallbackAction: () => {
                    isPlayingAnimation = false;
                }
            );
            characterWeaponActionPlayer.Play(characterWeaponPlayedData);
            await UniTask.WaitUntil(() => !isPlayingAnimation, cancellationToken: cancellationToken);
        }

        protected override void DamageToTarget(CancellationToken cancellationToken)
            => SpawnFlailObjectAsync(cancellationToken).Forget();

        private async UniTask SpawnFlailObjectAsync(CancellationToken cancellationToken, Action finishedAction = null)
        {
            var flailPrefabName = GetCorrectFlailPrefabName();
            PlaySpecialSoundAtk(cancellationToken);
            Vector2 flailPosition = ownerCharacterModel.CurrentAttackedTarget.Position;
            var flailObject = await PoolManager.Instance.Get(flailPrefabName, cancellationToken: cancellationToken);
            flailObject.transform.position = flailPosition;
            var damageBox = flailObject.GetComponent<FlailDamageBox>();
            damageBox.Init(ownerCharacterModel, DamageSource.FromAttack, true, 1, null, finishedAction: finishedAction);
        }

        private string GetCorrectFlailPrefabName()
        {
            var targetPosition = ownerCharacterModel.CurrentAttackedTarget.Position;
            var creatorPosition = ownerCharacterModel.Position;
            var deltaX = targetPosition.x - creatorPosition.x;
            var deltaY = targetPosition.y - creatorPosition.y;
            var angle = Mathf.Atan2(deltaY, deltaX);
            var angleDegrees = angle * Mathf.Rad2Deg;
            if (angleDegrees >= 0.0f && angleDegrees <= 90.0f)
                return s_flailPrefabNames[0];
            else if (angleDegrees > 90.0f && angleDegrees <= 180.0f)
                return s_flailPrefabNames[1];
            else if (angleDegrees < 0.0f && angleDegrees >= -90.0f)
                return s_flailPrefabNames[3];
            else if (angleDegrees < -90.0f && angleDegrees >= -180.0f)
                return s_flailPrefabNames[2];
            else
                return s_flailPrefabNames[0];
        }

        #endregion Class Methods
    }
}