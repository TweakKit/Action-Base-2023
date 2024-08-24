namespace Runtime.Gameplay.EntitySystem
{
    public abstract partial class CharacterEntity<T> : Entity<T> where T : CharacterModel
    {
        #region Class Methods

        protected virtual partial void PartialValidateDie() { }
        protected virtual partial void PartialDisposeDie() { }

        protected virtual partial void PartialInitializeDie()
             => ownerModel.DeathEvent += OnDeathOnDie;

        protected virtual void OnDeathOnDie(DamageSource damageSource)
             => ownerModel.ReactionChangedEvent.Invoke(CharacterReactionType.JustDie);

        #endregion Class Methdos
    }
}