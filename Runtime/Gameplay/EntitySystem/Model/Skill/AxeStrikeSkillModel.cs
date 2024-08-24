using Runtime.Config;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class AxeStrikeSkillModel : SkillModel
    {
        #region Properties

        public override SkillType SkillType => SkillType.AxeStrike;
        public override bool CanBeCanceled => true;
        public float DamageFactorValue { get; private set; }

        #endregion Properties

        #region Class Methods

        public AxeStrikeSkillModel(SkillData skillData) : base(skillData)
        {
            var skillConfig = skillData.configItem as AxeStrikeSkillDataConfigItem;
            DamageFactorValue = skillConfig.damageFactorValue;
        }

        #endregion Class Methods
    }
}