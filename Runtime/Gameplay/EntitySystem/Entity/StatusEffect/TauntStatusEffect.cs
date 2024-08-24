using System;

namespace Runtime.Gameplay.EntitySystem
{
    [Serializable]
    public class TauntStatusEffect : DurationStatusEffect<TauntStatusEffectModel>
    {
        #region Members

        private IInteractable _targetAttackInteractable;

        #endregion Members

        #region Properties

        protected override float Duration => ownerModel.Duration;

        #endregion Properties

        #region Class methods

        protected override void InitData(EntityModel senderModel)
        {
            base.InitData(senderModel);
            _targetAttackInteractable = ownerModel.CreatorIInteractable;
        }

        protected override void StartAffect(CharacterModel receiverModel)
        {
            base.StartAffect(receiverModel);
            receiverModel.StartGettingTaunt();
            receiverModel.UpdateTargetedTarget(_targetAttackInteractable);
            receiverModel.UpdateAttackedTarget(_targetAttackInteractable);
        }

        protected override void FinishAffect(CharacterModel receiverModel)
        {
            base.FinishAffect(receiverModel);
            receiverModel.StopGettingTaunt();
            _targetAttackInteractable = null;
        }

        protected override bool CheckStopAffect(CharacterModel receiverModel)
        {
            return base.CheckStopAffect(receiverModel) &&
                   _targetAttackInteractable != null && _targetAttackInteractable.IsDead;
        }

        #endregion Class methods
    }
}