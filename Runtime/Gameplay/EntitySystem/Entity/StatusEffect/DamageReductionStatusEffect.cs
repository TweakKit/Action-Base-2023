using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class DamageReductionStatusEffect : DurationStatusEffect<DamageReductionStatusEffectModel>
    {
        #region Properties

        protected override float Duration => ownerModel.Duration;

        #endregion Properties

        #region Class Methods

        protected override void StartAffect(CharacterModel receiverModel)
        {
            base.StartAffect(receiverModel);
            receiverModel.BuffStat(StatType.DamageReduction, ownerModel.IncreasedDamageReduction, StatModifyType.BaseBonus);
        }

        protected override void FinishAffect(CharacterModel receiverModel)
        {
            base.FinishAffect(receiverModel);
            receiverModel.DebuffStat(StatType.DamageReduction, ownerModel.IncreasedDamageReduction, StatModifyType.BaseBonus);
        }

        #endregion Class Methods
    }
}