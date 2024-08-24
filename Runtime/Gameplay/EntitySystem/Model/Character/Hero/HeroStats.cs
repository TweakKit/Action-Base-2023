using System;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    [Serializable]
    public class HeroLevelStats : CharacterLevelStats
    {
        #region Members

        public float chopDamage;
        public float chopSpeed;
        public float miningDamage;
        public float miningSpeed;
        public float cooldownReduction;

        #endregion Members
    }

    public class HeroStats : CharacterStats
    {
        #region Members

        private long _powerIgnoreEquipmentValue;

        #endregion Members

        #region Class Methods

        public HeroStats(CharacterLevelStats characterLevelStats) : base(characterLevelStats)
        {
            var heroLevelStats = characterLevelStats as HeroLevelStats;
            statsDictionary.Add(StatType.CooldownReduction, new CharacterStat(heroLevelStats.cooldownReduction));
            statsDictionary.Add(StatType.ChopDamage, new CharacterStat(heroLevelStats.chopDamage));
            statsDictionary.Add(StatType.ChopSpeed, new CharacterStat(heroLevelStats.chopSpeed));
            statsDictionary.Add(StatType.MiningDamage, new CharacterStat(heroLevelStats.miningDamage));
            statsDictionary.Add(StatType.MiningSpeed, new CharacterStat(heroLevelStats.miningSpeed));
        }

        public void SetPowerIgnoreEquipmentValue(long powerIgnoreEquipmentValue)
            => this._powerIgnoreEquipmentValue = powerIgnoreEquipmentValue;

        public long GetPowerIgnoreEquipmentValue()
            => _powerIgnoreEquipmentValue;

        #endregion Class Methods
    }
}