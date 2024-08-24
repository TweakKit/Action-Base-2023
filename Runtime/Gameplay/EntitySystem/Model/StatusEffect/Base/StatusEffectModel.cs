using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public abstract class StatusEffectModel
    {
        #region Members

        protected float chance;

        #endregion Members

        #region Propties

        public abstract StatusEffectType StatusEffectType { get; }
        public abstract bool IsStackable { get; }
        public virtual int MaxStack { get; private set; }
        public abstract bool IsOneShot { get; }
        public bool IsAffectable => UnityEngine.Random.Range(0.0f, 1.0f) <= chance;

        #endregion Propties

        #region Class Methods

        public StatusEffectModel(float chance)
            => this.chance = chance;

        public abstract StatusEffectModel Clone();
        public virtual void Stack(StatusEffectModel stackedStatusEffectModel) { }
        public virtual void SetMaxStack(int maxStack) => MaxStack = maxStack;
        public virtual void AddDuration(float addedDuration) { }

        #endregion Class Methods
    }
}