namespace Runtime.Gameplay.EntitySystem
{
    public partial class PVPHeroModel : HeroModel
    {
        #region Class Methods

        public override void StopGettingTaunt()
            => characterStatus &= ~CharacterStatus.Taunted;

        #endregion Class Methods
    }
}