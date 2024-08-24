using System;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    [Serializable]
    public class FixedDamageReductionNonDurationStatusEffect : NonDurationStatusEffect<FixedDamageReductionNonDurationStatusEffectModel>
    {
        #region Members

        #endregion Members

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

        protected override bool CheckStopAffect(CharacterModel receiverModel)
        {
            return base.CheckStopAffect(receiverModel) || 
                   !receiverModel.IsInGoddessAuraAreaAffect; 
        }

        #endregion Class Methods
    }
}