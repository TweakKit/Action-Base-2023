namespace Runtime.Gameplay.EntitySystem
{
    public class StunStatusEffect : DurationStatusEffect<StunStatusEffectModel>
    {
        #region Members

        private float _stunDuration;

        #endregion Members

        #region Properties

        protected override float Duration => _stunDuration;

        #endregion Properties

        #region Class methods

        protected override void InitData(EntityModel senderModel)
        {
            base.InitData(senderModel);
            _stunDuration = ownerModel.Duration;
        }

        protected override void StartAffect(CharacterModel receiverModel)
        {
            base.StartAffect(receiverModel);
            receiverModel.StartGettingStun();
        }

        protected override void FinishAffect(CharacterModel receiverModel)
        {
            base.FinishAffect(receiverModel);
            receiverModel.StopGettingStun();
        }

        #endregion Class methods
    }
}