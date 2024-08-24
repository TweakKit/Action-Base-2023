using Runtime.Gameplay.Manager;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class HeroModel : CharacterModel
    {
        #region Class Methods

        protected override void StopGettingHardCC()
        {
            if (!IsInHardCCStatus)
            {
                if (followingModel != null)
                    SetForceMovementStrategyType(followingModel.MovementStrategyType);
                else
                    UpdateState(CharacterState.Idle);
            }
        }

        public override void StopGettingTaunt()
        {
            base.StopGettingTaunt();
            HeroesControlManager.Instance.RefindTargetForHero(this, null);
        }

        #endregion Class Methods
    }
}