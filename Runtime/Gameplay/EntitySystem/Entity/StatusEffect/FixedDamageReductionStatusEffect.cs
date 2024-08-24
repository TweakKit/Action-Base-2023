using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class FixedDamageReductionStatusEffect : DurationStatusEffect<FixedDamageReductionStatusEffectModel>
    {
        #region Properties

        protected override float Duration => ownerModel.Duration;

        #endregion Properties

        #region Class Methods

        protected override void StartAffect(CharacterModel receiverModel)
        {
            base.StartAffect(receiverModel);
            receiverModel.BuffStat(StatType.FixedDamageReduction, ownerModel.IncreasedFixedDamageReduction, StatModifyType.BaseBonus);
        }

        protected override void FinishAffect(CharacterModel receiverModel)
        {
            base.FinishAffect(receiverModel);
            receiverModel.DebuffStat(StatType.FixedDamageReduction, ownerModel.IncreasedFixedDamageReduction, StatModifyType.BaseBonus);
        }

        #endregion Class Methods
    }
}