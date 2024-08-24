using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class AttackBuffStatusEffect : DurationStatusEffect<AttackBuffStatusEffectModel>
    {
        #region Properties

        protected override float Duration => ownerModel.Duration;
        private StatModifyType StatModifyType => ownerModel.StatModifyType;

        #endregion Properties

        #region Class Methods

        protected override void StartAffect(CharacterModel receiverModel)
        {
            base.StartAffect(receiverModel);
            receiverModel.BuffStat(StatType.AttackDamage, ownerModel.IncreasedAttack, StatModifyType);
        }

        protected override void FinishAffect(CharacterModel receiverModel)
        {
            base.FinishAffect(receiverModel);
            receiverModel.DebuffStat(StatType.AttackDamage, ownerModel.IncreasedAttack, StatModifyType);
        }

        #endregion Class Methods
    }
}