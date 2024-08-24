using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class ChillStatusEffect : DurationStatusEffect<ChillStatusEffectModel>
    {
        #region Properties

        protected override float Duration => ownerModel.Duration;

        #endregion Properties

        #region Class Methods

        protected override void StartAffect(CharacterModel receiverModel)
        {
            base.StartAffect(receiverModel);
            receiverModel.DebuffStat(StatType.AttackSpeed, ownerModel.DecreasedSpeed, StatModifyType.BaseMultiply);
            receiverModel.DebuffStat(StatType.MoveSpeed, ownerModel.DecreasedSpeed, StatModifyType.BaseMultiply);
        }

        protected override void FinishAffect(CharacterModel receiverModel)
        {
            base.FinishAffect(receiverModel);
            receiverModel.BuffStat(StatType.AttackSpeed, ownerModel.DecreasedSpeed, StatModifyType.BaseMultiply);
            receiverModel.BuffStat(StatType.MoveSpeed, ownerModel.DecreasedSpeed, StatModifyType.BaseMultiply);
        }

        #endregion Class Methods
    }
}