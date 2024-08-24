using System;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    [Serializable]
    public class DamageReductionNonDurationStatusEffect : NonDurationStatusEffect<DamageReductionNonDurationStatusEffectModel>
    {
        #region Members

        #endregion Members

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

        protected override bool CheckStopAffect(CharacterModel receiverModel)
        {
            return base.CheckStopAffect(receiverModel) || 
                   !receiverModel.IsInDamageReductionAreaAffect; 
        }

        #endregion Class Methods
    }
}