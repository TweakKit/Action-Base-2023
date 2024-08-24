using System;
using Runtime.Common.Resource;
using Runtime.Config;
using Runtime.Definition;
using CsvReader;

namespace Runtime.Gameplay.EntitySystem
{
    [Serializable]
    public class EnemyLevelStats : CharacterLevelStats
    {
        #region Members

        [CsvColumnFormat(ColumnFormat = "exp_{0}")]
        public ResourceData expResourcesData;

        #endregion Members
    }

    public class EnemyStats : CharacterStats
    {
        #region Class Methods

        public EnemyStats(CharacterLevelStats characterLevelStats, CharacterStatScaler characterStatScaler) 
            : base(characterLevelStats)
        {
            SetScaleBaseValue(StatType.HealthPoint, characterStatScaler.hpScaler);
            SetScaleBaseValue(StatType.AttackDamage, characterStatScaler.attackScaler);
        }

        #endregion Class Methods
    }
}