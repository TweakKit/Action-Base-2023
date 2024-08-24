using System;
using System.Collections.Generic;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    [Serializable]
    public class CharacterLevelStats
    {
        #region Members

        public float moveSpeed;
        public float attackDamage;
        public float hp;
        public float critChance;
        public float critDamage;
        public float attackRange;
        public float evasion;
        public float fixedDamageReduction;
        public float damageReduction;
        public float lifeSteal;
        public float attackSpeed;
        public float counterDamage;
        public float getMoreDamage;
        public float shieldPoint;

        #endregion Members
    }

    public abstract class CharacterStats
    {
        #region Members

        protected Dictionary<StatType, CharacterStat> statsDictionary;

        #endregion Members

        #region Properties

        public List<StatType> StatTypes
        {
            get
            {
                var statTypes = new List<StatType>();
                foreach (var stat in statsDictionary)
                    statTypes.Add(stat.Key);
                return statTypes;
            }
        }

        #endregion Properties

        #region Class Methods

        public CharacterStats(CharacterLevelStats characterLevelStats)
        {
            statsDictionary = new();
            statsDictionary.Add(StatType.MoveSpeed, new CharacterStat(characterLevelStats.moveSpeed));
            statsDictionary.Add(StatType.AttackDamage, new CharacterStat(characterLevelStats.attackDamage));
            statsDictionary.Add(StatType.HealthPoint, new CharacterStat(characterLevelStats.hp));
            statsDictionary.Add(StatType.CritChance, new CharacterStat(characterLevelStats.critChance));
            statsDictionary.Add(StatType.CritDamage, new CharacterStat(characterLevelStats.critDamage));
            statsDictionary.Add(StatType.AttackRange, new CharacterStat(characterLevelStats.attackRange));
            statsDictionary.Add(StatType.Evasion, new CharacterStat(characterLevelStats.evasion));
            statsDictionary.Add(StatType.FixedDamageReduction, new CharacterStat(characterLevelStats.fixedDamageReduction));
            statsDictionary.Add(StatType.DamageReduction, new CharacterStat(characterLevelStats.damageReduction));
            statsDictionary.Add(StatType.LifeSteal, new CharacterStat(characterLevelStats.lifeSteal));
            statsDictionary.Add(StatType.AttackSpeed, new CharacterStat(characterLevelStats.attackSpeed));
            statsDictionary.Add(StatType.CounterDamage, new CharacterStat(characterLevelStats.counterDamage));
            statsDictionary.Add(StatType.GetMoreDamage, new CharacterStat(characterLevelStats.getMoreDamage));
            statsDictionary.Add(StatType.ShieldPoint, new CharacterStat(characterLevelStats.shieldPoint));
        }

        public void AddBaseValue(StatType statType, float value)
        {
            if (statType != StatType.None)
                statsDictionary[statType].AddBaseValue(value);
        }

        public void SetScaleBaseValue(StatType statType, float scaleFactor)
        {
            if (statType != StatType.None)
                statsDictionary[statType].SetScaleBaseValue(scaleFactor);
        }

        public void AddBonusValue(StatType statType, float value, StatModifyType statModifyType)
        {
            if (statType != StatType.None)
                statsDictionary[statType].AddBonusValue(value, statModifyType);
        }

        public float GetStatTotalValue(StatType statType)
            => statsDictionary[statType].TotalValue;

        public void SetStatsScaleFactor(float statScaleFactor)
        {
            foreach (var key in statsDictionary.Keys)
                statsDictionary[key].SetScaleBaseValue(statScaleFactor);
        }

        public void SetStatsScaleFactorByStatType(StatType statType,float statScaleFactor)
        {
            if (statsDictionary.ContainsKey(statType))
                statsDictionary[statType].SetScaleBaseValue(statScaleFactor);
        }

        #endregion Class Methods

        #region Owner Classes

        public class CharacterStat
        {
            #region Members

            private float _baseValue;
            private float _baseBonus;
            private float _baseMultiply;

            #endregion Members

            #region Properties

            public float BaseValue => _baseValue;
            public float TotalValue => (_baseValue + _baseBonus) * _baseMultiply;

            #endregion Properties

            #region Class Methods

            public CharacterStat(float value)
            {
                _baseValue = value;
                _baseBonus = 0;
                _baseMultiply = 1;
            }

            public void AddBaseValue(float value)
                => _baseValue += value;

            public void SetScaleBaseValue(float scaleFactor)
                => _baseValue *= scaleFactor;

            public void AddBonusValue(float value, StatModifyType statModifyType)
            {
                switch (statModifyType)
                {
                    case StatModifyType.BaseBonus:
                        _baseBonus += value;
                        break;

                    case StatModifyType.BaseMultiply:
                        _baseMultiply += value;
                        break;
                }
            }

            #endregion Class Methods
        }

        #endregion Owner Classes
    }
}