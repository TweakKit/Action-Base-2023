using System;
using Runtime.Config;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public abstract class SkillModel : IComparable<SkillModel>
    {
        #region Properties

        public abstract SkillType SkillType { get; }
        public abstract bool CanBeCanceled { get; }
        public SkillTargetType TargetType { get; protected set; }
        public int SkillLevel { get; protected set; }
        public float CastRange { get; protected set; }
        public float Cooldown { get; set; }
        public float CurrentCooldown { get; set; }
        public float OriginalCooldown { get; protected set; }
        public int AttackCountTrigger { get; protected set; }
        public int InitialAttackCountTrigger { get; set; }
        public bool TriggerWhenCompletedAttack { get; set; }
        public int CurrentAttackCountTrigger { get; set; }
        public bool IsAutoRefreshCoolDown { get; set; }
        public bool IsSkillTakeBreakByCooldown { get; protected set; }
        public bool IsSkillTakeBreakByAttackCount => !IsSkillTakeBreakByCooldown;

        #endregion Properties

        #region Class Methods

        public SkillModel(SkillData skillData)
        {
            TargetType = skillData.configItem.TargetType;
            CastRange = skillData.configItem.castRange;
            Cooldown = skillData.configItem.cooldown;
            SkillLevel = skillData.configItem.dataId;
            CurrentCooldown = 0;
            AttackCountTrigger = skillData.configItem.attackCountTrigger;
            InitialAttackCountTrigger = skillData.configItem.initialAttackCountTrigger;
            TriggerWhenCompletedAttack = skillData.configItem.triggerWhenCompletedAttack;
            CurrentAttackCountTrigger = InitialAttackCountTrigger;
            IsAutoRefreshCoolDown = false;
            IsSkillTakeBreakByCooldown = Cooldown > 0 && AttackCountTrigger == 0;
            OriginalCooldown = Cooldown;
        }

        public virtual bool IsReady(bool isHeroBoss)
        {
            if (IsSkillTakeBreakByAttackCount)
                return CurrentAttackCountTrigger >= AttackCountTrigger;
            else
                return CurrentCooldown <= 0;
        }

        public int CompareTo(SkillModel other)
        {
            int skillTypeValue = (int)SkillType;
            int otherSkillTypeValue = (int)other.SkillType;
            return otherSkillTypeValue.CompareTo(skillTypeValue);
        }

        #endregion Class Methods
    }

    public class SkillData
    {
        #region Members

        public SkillDataConfigItem configItem;

        #endregion Members

        #region Class Methods

        public SkillData(SkillDataConfigItem configItem)
            => this.configItem = configItem;

        #endregion Class Methods
    }
}