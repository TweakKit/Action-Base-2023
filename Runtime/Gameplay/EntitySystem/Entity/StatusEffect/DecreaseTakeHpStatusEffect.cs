namespace Runtime.Gameplay.EntitySystem
{
    public class DecreaseTakeHpStatusEffect :  DurationStatusEffect<DecreaseTakeHpStatusEffectModel>
    {
        #region Members

        private float _decreaseTakeHpDuration;
        private float _decreaseTakeHpPercent;

        #endregion Members

        #region Properties

        protected override float Duration => _decreaseTakeHpDuration;

        #endregion Properties

        #region Class methods

        protected override void InitData(EntityModel senderModel)
        {
            base.InitData(senderModel);
            _decreaseTakeHpDuration = ownerModel.Duration;
            _decreaseTakeHpPercent = ownerModel.DecreaseTakeHpPercent;
        }

        protected override void StartAffect(CharacterModel receiverModel)
        {
            base.StartAffect(receiverModel);
            receiverModel.StartGettingDecreaseTakeHp(_decreaseTakeHpPercent);
        }

        protected override void FinishAffect(CharacterModel receiverModel)
        {
            base.FinishAffect(receiverModel);
            receiverModel.StopGettingDecreaseTakeHp();
        }

        #endregion Class methods
    }
}