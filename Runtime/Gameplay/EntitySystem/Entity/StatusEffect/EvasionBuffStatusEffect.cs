using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class EvasionBuffStatusEffect : DurationStatusEffect<EvasionBuffStatusEffectModel>
    {
        #region Properties

        protected override float Duration => ownerModel.Duration;

        #endregion Properties

        #region Class Methods

        protected override void StartAffect(CharacterModel receiverModel)
        {
            base.StartAffect(receiverModel);
            receiverModel.BuffStat(StatType.Evasion, ownerModel.IncreasedEvasion, StatModifyType.BaseBonus);
        }

        protected override void FinishAffect(CharacterModel receiverModel)
        {
            base.FinishAffect(receiverModel);
            receiverModel.DebuffStat(StatType.Evasion, ownerModel.IncreasedEvasion, StatModifyType.BaseBonus);
        }

        #endregion Class Methods
    }
}