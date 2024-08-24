using System.Collections.Generic;
using UnityEngine;
using Runtime.Config;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class PVPHeroModel : HeroModel
    {
        #region Members

        private PVPHeroBuffData _pvpHeroBuffData;
        private bool _isHeroOpponent;
        private float _damageDealtStatistics;
        private float _damageHealedStatistics;
        private float _damageReceivedStatistics;

        #endregion Members

        #region Properties

        public override bool IsHeroBoss => _isHeroOpponent;
        public PVPHeroBuffData PVPHeroBuffData => _pvpHeroBuffData;
        public float DamageDealtStatistics => _damageDealtStatistics;
        public float DamageHealedStatistics => _damageHealedStatistics;
        public float DamageReceivedStatistics => _damageReceivedStatistics;

        #endregion Properties

        #region Class Methods

        public PVPHeroModel(uint heroUId, string heroId, Vector2 formationPosition, HeroModelData heroModelData, HeroModel followingModel,
                            MovementStrategyType movementStrategyType, PVPHeroBuffData pvpHeroBuffData, bool isHeroOpponent)
            : base(heroUId, heroId, formationPosition, heroModelData, followingModel, movementStrategyType)
        {
            _pvpHeroBuffData = pvpHeroBuffData;
            _isHeroOpponent = isHeroOpponent;
            _damageDealtStatistics = 0;
            _damageHealedStatistics = 0;
            _damageReceivedStatistics = 0;
        }

        public void ReFillStatisticsValue(float damageDealtStatistics, float damageHealedStatistics, float damageReceivedStatistics)
        {
            _damageDealtStatistics += damageDealtStatistics;
            _damageHealedStatistics += damageHealedStatistics;
            _damageReceivedStatistics += damageReceivedStatistics;
        }

        public void AddDamageDealtStatistics(float value)
        {
            if (value > 0)
                _damageDealtStatistics += value;
        }
        
        public void AddDamageReceivedStatistics(float value)
        {
            if (value > 0)
                _damageReceivedStatistics += value;
        }
        
        public void AddDamageHealedStatistics(float value)
        {
            if (value > 0)
                _damageHealedStatistics += value;
        }

        public override float DebuffHp(DamageInfo damageInfo)
        {
            var debuffValue = base.DebuffHp(damageInfo);
            if (damageInfo.creatorModel is PVPHeroModel pvpHeroModel)
                pvpHeroModel.AddDamageDealtStatistics(debuffValue);

            return debuffValue;
        }

        #endregion Class Methods
    }

    public struct PVPHeroBuffData
    {
        #region Members

        public int firstBranchSkillTreeHighestUnlocked;
        public int secondBranchSkillTreeHighestUnlocked;
        public bool hasSlotBuffStat;
        public BuffStatItem slotBuffStatItem;
        public List<BuffTargetStatItem> teamBuffTargetStatItems;
        public List<BuffStatItem> pvpBuildingBuffItems;
        public List<EquipmentStat> equipmentStatBuffs;

        #endregion Members

        #region Struct Methods

        public PVPHeroBuffData(int firstBranchSkillTreeHighestUnlocked, int secondBranchSkillTreeHighestUnlocked, bool hasSlotBuffStat,
                               BuffStatItem slotBuffStatItem, List<BuffTargetStatItem> teamBuffTargetStatItems,
                               List<BuffStatItem> pvpBuildingBuffItems, List<EquipmentStat> equipmentStatBuffs)
        {
            this.firstBranchSkillTreeHighestUnlocked = firstBranchSkillTreeHighestUnlocked;
            this.secondBranchSkillTreeHighestUnlocked = secondBranchSkillTreeHighestUnlocked;
            this.hasSlotBuffStat = hasSlotBuffStat;
            this.slotBuffStatItem = slotBuffStatItem;
            this.teamBuffTargetStatItems = teamBuffTargetStatItems;
            this.pvpBuildingBuffItems = pvpBuildingBuffItems;
            this.equipmentStatBuffs = equipmentStatBuffs;
        }

        #endregion Struct Methods
    }
}