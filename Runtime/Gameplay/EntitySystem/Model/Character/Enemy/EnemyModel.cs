using Runtime.Common.Resource;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class EnemyModel : CharacterModel
    {
        #region Members

        protected ResourceData[] diedResourcesData;
        protected ResourceData expResourcesData;
        protected float activatedSqrRange;
        protected float detectedSqrRange;
        protected float campBuffSqrRange;
        protected bool isImmortal;
        protected int zoneLevel;
        protected bool isElite;
        protected float dropEquipmentRate;
        protected bool markRepspawnable;
        protected long currentBattleIndex;
        protected bool ignoreQuest;

        #endregion Members

        #region Properties

        public override EntityType EntityType => EntityType.Enemy;
        public ResourceData[] DiedResourcesData => diedResourcesData;
        public ResourceData ExpResourcesData => expResourcesData;
        public float ActivatedSqrRange => activatedSqrRange + campBuffSqrRange;
        public float DetectedSqrRange => detectedSqrRange + campBuffSqrRange;
        public bool IsImmortal => isImmortal;
        public int ZoneLevel => zoneLevel;
        public float DropEquipmentRate => dropEquipmentRate;
        public bool IsElite => isElite;
        public bool MarkRepspawnable => markRepspawnable;
        public long CurrentBattleIndex => currentBattleIndex;
        public bool IgnoreQuest => ignoreQuest;

        public void SetCurrentBattleIndex(long battleIndex)
            => this.currentBattleIndex = battleIndex;

        #endregion Properties

        #region Class Methods

        public EnemyModel(uint enemyUId, string enemyId, bool isImmortal, EnemyModelData enemyModelData, int zoneLevel,
                          bool isElite, float dropEquipmentRate, bool markRepspawnable)
            : base(enemyUId, enemyId, enemyModelData)
        {
            this.isImmortal = isImmortal;
            this.respawnDelay = enemyModelData.RespawnDelay;
            this.diedResourcesData = enemyModelData.DiedResourcesData;
            this.expResourcesData = enemyModelData.ExpResourcesData;
            this.activatedSqrRange = enemyModelData.ActivatedSqrRange;
            this.detectedSqrRange = enemyModelData.DetectedSqrRange;
            this.zoneLevel = zoneLevel;
            this.isElite = isElite;
            this.dropEquipmentRate = dropEquipmentRate;
            this.markRepspawnable = markRepspawnable;
            this.campBuffSqrRange = 0.0f;
            this.ignoreQuest = false;
        }

        public void BuffCampRange(float rangeSqrBuff)
           => campBuffSqrRange = rangeSqrBuff;

        public void ResetCampRange()
           => campBuffSqrRange = 0.0f;
        
        public void SetMarkRespawnable(bool canRespawn) 
            => markRepspawnable = canRespawn;

        public void SetIgnoreQuest(bool isIgnoreQuest) 
            => ignoreQuest = isIgnoreQuest;

        #endregion Class Methods
    }

    public class EnemyModelData : CharacterModelData
    {
        #region Members

        protected ResourceData[] diedResourcesData;
        protected ResourceData expResourcesData;
        protected float activatedSqrRange;
        protected float detectedSqrRange;

        #endregion Members

        #region Properties

        public ResourceData[] DiedResourcesData => diedResourcesData;
        public ResourceData ExpResourcesData => expResourcesData;
        public float ActivatedSqrRange => activatedSqrRange;
        public float DetectedSqrRange => detectedSqrRange;

        #endregion Properties

        #region Class Methods

        public EnemyModelData(string enemyId, string enemyVisualId, int detectedPriority, float activatedSqrRange,
                              float detectedSqrRange, AttackType attackType, ResourceData[] diedResourcesData,
                              ResourceData expResourcesData, float respawnDelay, EnemyLevelModel enemyLevelModel)
            : base(enemyId, enemyVisualId, detectedPriority, attackType, respawnDelay, enemyLevelModel)
        {
            this.diedResourcesData = diedResourcesData;
            this.expResourcesData = expResourcesData;
            this.activatedSqrRange = activatedSqrRange;
            this.detectedSqrRange = detectedSqrRange;
        }

        public void SetDiedResourcesData(ResourceData[] resourcesData) 
            => diedResourcesData = resourcesData;

        public void SetExpResourcesData(ResourceData resourceExpData)
            => expResourcesData = resourceExpData;
        
        #endregion Class Methods
    }

    public class EnemyLevelModel : CharacterLevelModel
    {
        #region Class Methods

        public EnemyLevelModel(int level, CharacterStats characterStats)
            : base(level, characterStats) { }

        #endregion Class Methods
    }
}