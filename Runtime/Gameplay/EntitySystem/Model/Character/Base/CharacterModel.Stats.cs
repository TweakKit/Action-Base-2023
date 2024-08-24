using System.Collections.Generic;
using UnityEngine;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public abstract partial class CharacterModel : EntityModel
    {
        #region Members

        protected Dictionary<StatType, EntityStat> statsDictionary;

        #endregion Members

        #region Class Methods

        public override float GetTotalStatValue(StatType statType)
            => statsDictionary[statType].TotalValue;

        public void UpdateStats(CharacterStats characterStats)
        {
            foreach (var statType in characterStats.StatTypes)
            {
                var updatedStat = statsDictionary[statType];
                var statTotalValue = characterStats.GetStatTotalValue(statType);
                if (updatedStat.BaseValue != statTotalValue)
                {
                    updatedStat.SetBaseValue(statTotalValue);
                    statsDictionary[statType] = updatedStat;
                    StatChangedEvent.Invoke(statType, updatedStat.TotalValue);
                }
            }
        }
        
        public void BuffShield(float value, DamageSource damageSource)
        {
            var buffValue = value;
            if (currentDefense + value >= MaxDefense)
                buffValue = MaxDefense - currentDefense;

            currentDefense += buffValue;
            ShieldChangedEvent.Invoke(buffValue, damageSource);
        }

        public void BuffHp(float value)
        {
            var buffValue = value;
            if (currentHp + value >= MaxHp)
                buffValue = MaxHp - currentHp;

            if (!CanChangeHealthPoint())
                buffValue = 0;

            if (IsOnDecreaseTakeHpStatus())
            {
                buffValue *= DecreaseTakeHpPercent;
            }

            currentHp += buffValue;
            HealthChangedEvent.Invoke(buffValue, DamageSource.None);
        }
        
        public void DebuffHpUnknowDamageSource(float value)
        {
            if (!IsDead)
            {
                var debuffValue = value;
                if (Constant.IsAlwaysTakeOneDamageValueCharacter(EntityId))
                    debuffValue = 1;

                if (currentHp + debuffValue <= 0)
                    debuffValue = currentHp;

                if (!CanChangeHealthPoint())
                    debuffValue = 0;

                currentHp -= debuffValue;
                HealthChangedEvent.Invoke(-debuffValue, DamageSource.None);
                if (IsDead)
                    DeathEvent.Invoke(DamageSource.None);
            }
        }

        public void RestoreHp(float restoreMaxHpPercent = 1.0f)
            => BuffHp(maxHp * restoreMaxHpPercent);

        public void DeductHp(float deductedMaxHpPercent, DamageSource damageSource)
        {
            var value = maxHp * deductedMaxHpPercent;
            var debuffValue = value;
            if (currentHp + value <= 0)
                debuffValue = currentHp;

            currentHp -= debuffValue;
            HealthChangedEvent.Invoke(-debuffValue, damageSource);

            if (IsDead)
                DeathEvent.Invoke(damageSource);
        }

        public void BuffStat(StatType statType, float value, StatModifyType statModifyType)
        {
            if (statType == StatType.HealthPoint)
            {
                BuffMaxHp(value, statModifyType);
                return;
            }
            
            else if (statType == StatType.ShieldPoint)
            {
                BuffMaxDefense(value, statModifyType);
                StatChangedEvent.Invoke(statType, MaxDefense);
                return;
            }

            if (!statsDictionary.ContainsKey(statType))
                statsDictionary.Add(statType, new EntityStat(0));

            var buffedStat = statsDictionary[statType];
            buffedStat.BuffValue(value, statModifyType);
            statsDictionary[statType] = buffedStat;
            StatChangedEvent.Invoke(statType, statsDictionary[statType].TotalValue);
        }

        public void DebuffStat(StatType statType, float value, StatModifyType statModifyType)
        {
            if (statType == StatType.HealthPoint)
            {
                DebuffMaxHp(value, statModifyType);
                return;
            }
            else if (statType == StatType.ShieldPoint)
            {
                DebuffDefense(value, statModifyType);
                StatChangedEvent.Invoke(statType, MaxDefense);
                return;
            }

            if (!statsDictionary.ContainsKey(statType))
                statsDictionary.Add(statType, new EntityStat(0));

            var debuffedStat = statsDictionary[statType];
            debuffedStat.DebuffValue(value, statModifyType);
            statsDictionary[statType] = debuffedStat;
            StatChangedEvent.Invoke(statType, statsDictionary[statType].TotalValue);
        }
        
        protected virtual void BuffMaxDefense(float value, StatModifyType statModifyType)
        {
            switch (statModifyType)
            {
                case StatModifyType.BaseBonus:
                    baseBonusDefense += value;
                    break;

                case StatModifyType.BaseMultiply:
                    baseMultiplyDefense += value;
                    break;
            }

            currentDefense = MaxDefense;
        }

        protected virtual void DebuffDefense(float value, StatModifyType statModifyType)
        {
            switch (statModifyType)
            {
                case StatModifyType.BaseBonus:
                    baseBonusDefense -= value;
                    break;

                case StatModifyType.BaseMultiply:
                    baseMultiplyDefense -= value;
                    break;
            }

            currentDefense = Mathf.Min(currentDefense, MaxDefense);
        }

        protected void BuffMaxHp(float value, StatModifyType statModifyType)
        {
            switch (statModifyType)
            {
                case StatModifyType.BaseBonus:
                    baseBonusHp += value;
                    break;

                case StatModifyType.BaseMultiply:
                    baseMultiplyHp += value;
                    break;
            }

            currentHp = MaxHp;
        }

        protected void DebuffMaxHp(float value, StatModifyType statModifyType)
        {
            switch (statModifyType)
            {
                case StatModifyType.BaseBonus:
                    baseBonusHp -= value;
                    break;

                case StatModifyType.BaseMultiply:
                    baseMultiplyHp -= value;
                    break;
            }

            currentHp = Mathf.Min(currentHp, MaxHp);
        }

        protected partial void InitStats(CharacterStats characterStats)
        {
            statsDictionary = new Dictionary<StatType, EntityStat>();
            foreach (var statType in characterStats.StatTypes)
            {
                var statTotalValue = characterStats.GetStatTotalValue(statType);
                statsDictionary.Add(statType, new EntityStat(statTotalValue));
            }
        }

        #endregion Class Methods
    }
}