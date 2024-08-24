namespace Runtime.Gameplay.EntitySystem
{
    public partial class PVPHeroEntity : HeroEntity
    {
        #region Class Methods

        public override bool CanGetAffected(EntityModel interactingModel, DamageSource damageSource)
        {
            if (!ownerModel.IsDead)
                return ownerModel.IsHeroBoss != interactingModel.IsHeroBoss;
            else
                return false;
        }

        #endregion Class Methods
    }
}