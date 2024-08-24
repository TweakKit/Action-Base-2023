namespace Runtime.Gameplay.EntitySystem
{
    public class InvincibilityStatusEffect :  DurationStatusEffect<InvincibilityStatusEffectModel>
    {
        #region Members

        private float _invincibilityDuration;

        #endregion Members

        #region Properties

        protected override float Duration => _invincibilityDuration;

        #endregion Properties

        #region Class methods

        protected override void InitData(EntityModel senderModel)
        {
            base.InitData(senderModel);
            _invincibilityDuration = ownerModel.Duration;
        }

        protected override void StartAffect(CharacterModel receiverModel)
        {
            base.StartAffect(receiverModel);
            receiverModel.StartGettingInvicibility();
        }

        protected override void FinishAffect(CharacterModel receiverModel)
        {
            base.FinishAffect(receiverModel);
            receiverModel.StopGettingInvicibility();
        }

        #endregion Class methods
    }
}