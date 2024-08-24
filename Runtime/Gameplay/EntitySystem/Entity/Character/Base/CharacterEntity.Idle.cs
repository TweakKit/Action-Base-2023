namespace Runtime.Gameplay.EntitySystem
{
    public abstract partial class CharacterEntity<T> : Entity<T> where T : CharacterModel
    {
        #region Class Methods

        protected virtual partial void PartialValidateIdle() { }
        protected virtual partial void PartialInitializeIdle() { }
        protected virtual partial void PartialDisposeIdle() { }
        protected virtual partial void PartialUpdateIdle() { }

        #endregion Class Methods
    }
}