using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class CritChanceBuffStatusEffect : DurationStatusEffect<CritChanceBuffStatusEffectModel>
    {
        #region Properties

        protected override float Duration => ownerModel.Duration;

        #endregion Properties

        #region Class Methods

        protected override void StartAffect(CharacterModel receiverModel)
        {
            base.StartAffect(receiverModel);
            receiverModel.BuffStat(StatType.CritChance, ownerModel.IncreasedCritChance, StatModifyType.BaseBonus);
        }

        protected override void FinishAffect(CharacterModel receiverModel)
        {
            base.FinishAffect(receiverModel);
            receiverModel.DebuffStat(StatType.CritChance, ownerModel.IncreasedCritChance, StatModifyType.BaseBonus);
        }

        #endregion Class Methods
    }
}