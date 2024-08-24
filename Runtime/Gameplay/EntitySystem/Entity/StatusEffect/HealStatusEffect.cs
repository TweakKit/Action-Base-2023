using System;

namespace Runtime.Gameplay.EntitySystem
{
    [Serializable]
    public class HealStatusEffect : OneShotStatusEffect<HealStatusEffectModel>
    {
        #region Class Methods

        protected override void StartAffect(CharacterModel receiverModel)
        {
            base.StartAffect(receiverModel);
            receiverModel.BuffHp(ownerModel.HealAmount);
        }

        #endregion Class Methods
    }
}