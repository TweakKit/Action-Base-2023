using Runtime.Definition;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class GetMoreDamageStatusEffect : DurationStatusEffect<GetMoreDamageStatusEffectModel>
    {
        #region Properties

        protected override float Duration => ownerModel.Duration;
        protected float GetMoreDamageByPercent => ownerModel.GetMoreDamagePercent;

        #endregion Properties

        #region Class Methods

        protected override void StartAffect(CharacterModel receiverModel)
        {
            base.StartAffect(receiverModel);
            receiverModel.BuffStat(StatType.GetMoreDamage, GetMoreDamageByPercent, StatModifyType.BaseBonus);
            receiverModel.StartGettingGetMoreDamage();
        }

        protected override void FinishAffect(CharacterModel receiverModel)
        {
            base.FinishAffect(receiverModel);
            receiverModel.DebuffStat(StatType.GetMoreDamage, GetMoreDamageByPercent, StatModifyType.BaseBonus);
            receiverModel.StopGettingGetMoreDamage();
        }

        #endregion Class Methods
    }
}