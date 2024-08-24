using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class SlowStatusEffect : DurationStatusEffect<SlowStatusEffectModel>
    {
        #region Properties

        protected override float Duration => ownerModel.Duration;

        #endregion Properties

        #region Class Methods

        protected override void StartAffect(CharacterModel receiverModel)
        {
            base.StartAffect(receiverModel);
            receiverModel.DebuffStat(StatType.MoveSpeed, ownerModel.DecreasedMoveSpeed, StatModifyType.BaseMultiply);
        }

        protected override void FinishAffect(CharacterModel receiverModel)
        {
            base.FinishAffect(receiverModel);
            receiverModel.BuffStat(StatType.MoveSpeed, ownerModel.DecreasedMoveSpeed, StatModifyType.BaseMultiply);
        }

        #endregion Class Methods
    }
}