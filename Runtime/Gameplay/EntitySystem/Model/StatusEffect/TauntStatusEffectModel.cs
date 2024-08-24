using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class TauntStatusEffectModel : DurationStatusEffectModel
    {
        #region Members

        private IInteractable _creatorIInteractable;

        #endregion Members

        #region Properties

        public override StatusEffectType StatusEffectType => StatusEffectType.Taunt;
        public override bool IsStackable => false;
        public IInteractable CreatorIInteractable => _creatorIInteractable;

        #endregion Properties

        #region Class Methods

        public TauntStatusEffectModel(IInteractable creatorIInteractable, float duration, float chance = 1.0f) : base(duration, chance)
            => _creatorIInteractable = creatorIInteractable;

        public override StatusEffectModel Clone()
            => MemberwiseClone() as TauntStatusEffectModel;

        #endregion Class Methods
    }
}