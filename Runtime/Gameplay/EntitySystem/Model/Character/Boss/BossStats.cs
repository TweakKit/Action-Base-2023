using System;
using Runtime.Common.Resource;
using Runtime.Definition;
using Runtime.Config;
using CsvReader;

namespace Runtime.Gameplay.EntitySystem
{
    [Serializable]
    public class BossLevelStats : CharacterLevelStats
    {
        #region Members

        [CsvColumnFormat(ColumnFormat = "exp_{0}")]
        public ResourceData expResourcesData;

        #endregion Members
    }

    public class BossStats : CharacterStats
    {
        #region Class Methods

        public BossStats(CharacterLevelStats characterLevelStats, CharacterStatScaler characterStatScaler)
            : base(characterLevelStats)
        {
            statsDictionary.Add(StatType.CooldownReduction, new CharacterStat(0));
            SetScaleBaseValue(StatType.HealthPoint, characterStatScaler.hpScaler);
            SetScaleBaseValue(StatType.AttackDamage, characterStatScaler.attackScaler);
        }

        #endregion Class Methods
    }
}