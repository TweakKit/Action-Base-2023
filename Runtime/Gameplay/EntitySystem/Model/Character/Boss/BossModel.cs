using System.Collections.Generic;
using Runtime.Definition;
using Runtime.Common.Resource;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class BossModel : EnemyModel
    {
        #region Members

        protected List<SkillModel> skillModels;
        protected bool ignoreSpawnChest;

        #endregion Members

        #region Properties

        public override EntityType EntityType => EntityType.Boss;
        public List<SkillModel> SkillModels => skillModels;
        public bool IgnoreSpawnChest => ignoreSpawnChest;

        #endregion Properties

        #region Class Methods

        public BossModel(uint bossUId, string bossId, bool isImmortal, BossModelData bossModelData,
                         int zoneLevel, float dropEquipmentRate, bool markRepspawnable)
            : base(bossUId, bossId, isImmortal, bossModelData, zoneLevel, false, dropEquipmentRate, markRepspawnable)
        {
            isElite = true;
            this.skillModels = bossModelData.SkillModels;
            this.ignoreSpawnChest = false;
        }
        
        public void SetIgnoreSpawnChest(bool isIgnoreSpawnChest) 
            => ignoreSpawnChest = isIgnoreSpawnChest;

        #endregion Class Methods
    }

    public class BossModelData : EnemyModelData
    {
        #region Members

        protected List<SkillModel> skillModels;

        #endregion Members

        #region Properties

        public List<SkillModel> SkillModels => skillModels;

        #endregion Properties

        #region Class Methods

        public BossModelData(string enemyId, string enemyVisualId, int detectedPriority, float attivatedSqrRange,
                             float detectedSqrRange, AttackType attackType, ResourceData[] diedResourcesData,
                             ResourceData expResourcesData, float respawnDelay, List<SkillModel> skillModels, BossLevelModel bossLevelModel)
            : base(enemyId, enemyVisualId, detectedPriority, attivatedSqrRange, detectedSqrRange,
                   attackType, diedResourcesData, expResourcesData, respawnDelay, bossLevelModel)
            => this.skillModels = skillModels;

        #endregion Class Methods
    }

    public class BossLevelModel : EnemyLevelModel
    {
        #region Class Methods

        public BossLevelModel(int level, CharacterStats characterStats)
            : base(level, characterStats) { }

        #endregion Class Methods
    }
}