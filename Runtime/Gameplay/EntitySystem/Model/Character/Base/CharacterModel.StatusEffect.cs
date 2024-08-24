using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Runtime.Definition;
using Runtime.Manager.Data;

namespace Runtime.Gameplay.EntitySystem
{
    [Flags]
    public enum CharacterStatus
    {
        None = 0,
        Stunned = 1 << 0,
        KnockedUp = 1 << 1,
        Taunted = 1 << 2,
        Freezed = 1 << 3,
        Invincibilited = 1 << 4,
        DecreasedTakeHp = 1 << 5,
        Gargoyled = 1 << 6,
        GetMoreDamaged = 1 << 7,
        PullToCenter = 1 << 8,
        HardCC = Stunned | KnockedUp | Freezed | PullToCenter
    }

    public abstract partial class CharacterModel : EntityModel
    {
        #region Members

        protected List<StatusEffectType> statusEffectsStackData = new();
        protected CharacterStatus characterStatus = CharacterStatus.None;

        #endregion Members

        #region Properties

        public bool IsInHardCCStatus
            => (characterStatus & CharacterStatus.HardCC) > 0;

        public bool IsInBleedStatus
        {
            get
            {
                var bleedAttackStatusEffectCount = GetStatusEffectStackCount(StatusEffectType.BleedAttack);
                return bleedAttackStatusEffectCount > 0;
            }
        }

        #endregion Properties

        #region Class Methods

        public void StackStatusEffect(StatusEffectType statusEffectType)
            => statusEffectsStackData.Add(statusEffectType);

        public void ClearStatusEffectStack(StatusEffectType statusEffectType)
        {
            for (int i = statusEffectsStackData.Count - 1; i >= 0; i--)
                if (statusEffectsStackData[i] == statusEffectType)
                    statusEffectsStackData.RemoveAt(i);
        }

        public int GetStatusEffectStackCount(StatusEffectType statusEffectType)
            => statusEffectsStackData.Count(x => x == statusEffectType);

        public bool CheckContainStatusEffectInStack(StatusEffectType statusEffectType)
           => statusEffectsStackData.Any(x => x == statusEffectType);

        public virtual void StartGettingKnockUp()
        {
            characterStatus |= CharacterStatus.KnockedUp;
            HardCCImpactedEvent.Invoke(StatusEffectType.KnockUp);
            UpdateState(CharacterState.HardCC);
        }

        public virtual void GettingKnockUp(Vector2 knockUpPosition)
            => SetTransformMovePosition(knockUpPosition);

        public virtual void StopGettingKnockUp()
        {
            characterStatus &= ~CharacterStatus.KnockedUp;
            StopGettingHardCC();
        }

        public virtual void GettingPullToCenter(Vector2 newPosition)
            => SetTransformMovePosition(newPosition);

        public virtual void StartGettingPullToCenter()
        {
            characterStatus |= CharacterStatus.PullToCenter;
            HardCCImpactedEvent.Invoke(StatusEffectType.PullToCenter);
            UpdateState(CharacterState.HardCC);
        }

        public virtual void StopGettingPullToCenter()
        {
            characterStatus &= ~CharacterStatus.PullToCenter;
            StopGettingHardCC();
        }

        public virtual void StartGettingStun()
        {
            characterStatus |= CharacterStatus.Stunned;
            HardCCImpactedEvent.Invoke(StatusEffectType.Stun);
            UpdateState(CharacterState.HardCC);
        }

        public virtual void StopGettingStun()
        {
            characterStatus &= ~CharacterStatus.Stunned;
            StopGettingHardCC();
        }

        public virtual void StartGettingFreeze()
        {
            characterStatus |= CharacterStatus.Freezed;
            HardCCImpactedEvent.Invoke(StatusEffectType.Freeze);
            UpdateState(CharacterState.HardCC);
        }

        public virtual void StopGettingFreeze()
        {
            characterStatus &= ~CharacterStatus.Freezed;
            StopGettingHardCC();
        }

        public virtual void StartGettingTaunt()
            => characterStatus |= CharacterStatus.Taunted;

        public virtual void StopGettingTaunt()
            => characterStatus &= ~CharacterStatus.Taunted;

        protected virtual void StopGettingHardCC()
        {
            if (!IsInHardCCStatus)
                UpdateState(CharacterState.Idle);
        }

        public virtual void StartGettingInvicibility()
            => characterStatus |= CharacterStatus.Invincibilited;

        public virtual void StopGettingInvicibility()
            => characterStatus &= ~CharacterStatus.Invincibilited;

        public virtual void StartGettingDecreaseTakeHp(float decreaseTakeHpPercent)
        {
            characterStatus |= CharacterStatus.DecreasedTakeHp;
            DecreaseTakeHpPercent = decreaseTakeHpPercent;
        }

        public virtual void StopGettingDecreaseTakeHp()
        {
            characterStatus &= ~CharacterStatus.DecreasedTakeHp;
            DecreaseTakeHpPercent = 0;
        }

        public virtual void StartGettingGargoyle()
        {
            characterStatus |= CharacterStatus.Gargoyled;
        }

        public virtual void StopGettingGargoyle()
        {
            characterStatus &= ~CharacterStatus.Gargoyled;
        }

        public virtual void StartGettingGetMoreDamage()
        {
            characterStatus |= CharacterStatus.GetMoreDamaged;
        }

        public virtual void StopGettingGetMoreDamage()
        {
            characterStatus &= ~CharacterStatus.GetMoreDamaged;
        }

        #endregion Class Methods
    }
}