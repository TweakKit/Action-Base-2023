namespace Runtime.Gameplay.EntitySystem
{
    public class FreezeStatusEffect : DurationStatusEffect<FreezeStatusEffectModel>
    {
        #region Members

        private float _freezeDuration;

        #endregion Members

        #region Properties

        protected override float Duration => _freezeDuration;

        #endregion Properties

        #region Class methods

        protected override void InitData(EntityModel senderModel)
        {
            base.InitData(senderModel);
            _freezeDuration = ownerModel.Duration;
        }

        protected override void StartAffect(CharacterModel receiverModel)
        {
            base.StartAffect(receiverModel);
            receiverModel.StartGettingFreeze();
        }

        protected override void FinishAffect(CharacterModel receiverModel)
        {
            base.FinishAffect(receiverModel);
            receiverModel.StopGettingFreeze();
        }

        #endregion Class methods
    }
}