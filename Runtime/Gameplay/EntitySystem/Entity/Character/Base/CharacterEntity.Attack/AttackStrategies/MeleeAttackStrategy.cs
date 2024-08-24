using System.Threading;
using UnityEngine;
using Runtime.Definition;
using Cysharp.Threading.Tasks;

namespace Runtime.Gameplay.EntitySystem
{
    public class MeleeAttackStrategy : AttackStrategy
    {
        #region Class Methods

        protected override float GetOriginalNormalAttackAnimationTime()
        {
            var characterWeaponAnimationType = CharacterWeaponAnimationType.MeleeAttack;
            return characterWeaponActionPlayer.GetAnimationTime(characterWeaponAnimationType);
        }

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
            PlaySpecialSoundAtk(cancellationToken);
            characterWeaponActionPlayer.Play(characterWeaponPlayedData);
            await UniTask.WaitUntil(() => !isPlayingAnimation, cancellationToken: cancellationToken);
        }

        protected virtual void DamageToTarget(CancellationToken cancellationToken)
        {
            if (IsTargetInAttackRange())
            {
                var damageInfo = GetDamageInfo(ownerCharacterModel.CurrentAttackedTarget.Model);
                var damageDirection = (ownerCharacterModel.CurrentAttackedTarget.Position - ownerCharacterModel.Position).normalized;
                ownerCharacterModel.CurrentAttackedTarget.GetHit(damageInfo, damageDirection);
            }
        }

        protected bool IsTargetInAttackRange()
        {
            if (!ownerCharacterModel.CurrentAttackedTarget.IsDead)
            {
                var comparedRange = 0.0f;
                if (ownerCharacterModel.CurrentAttackedTarget.IsObject)
                    comparedRange = Constant.HEROES_INTERACT_OBJECTS_DISTANCE;
                else
                    comparedRange = ownerCharacterModel.GetTotalStatValue(StatType.AttackRange);
                comparedRange += ownerCharacterModel.CurrentAttackedTarget.Model.BodyBoundRadius;
                var targetPosition = ownerCharacterModel.CurrentAttackedTarget.Position;
                var sqrDistanceBetween = Vector2.SqrMagnitude(ownerCharacterModel.Position - targetPosition);
                return sqrDistanceBetween <= comparedRange * comparedRange;
            }
            else return false;
        }

        protected DamageInfo GetDamageInfo(EntityModel affectedEntityModel)
        {
            var damageInfo = ownerCharacterModel.GetDamageInfo(DamageSource.FromAttack, 1, null, affectedEntityModel);
            return damageInfo;
        }

        #endregion Class Methods
    }
}