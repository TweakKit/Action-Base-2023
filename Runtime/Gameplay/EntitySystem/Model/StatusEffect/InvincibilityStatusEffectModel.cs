using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class InvincibilityStatusEffectModel : DurationStatusEffectModel
    {
        #region Properties

        public override StatusEffectType StatusEffectType => StatusEffectType.Invincibility;
        public override bool IsStackable => false;

        #endregion Properties

        #region Class Methods

        public InvincibilityStatusEffectModel(float duration, float chance = 1.0f) : base(duration, chance) { }

        public override StatusEffectModel Clone()
            => MemberwiseClone() as InvincibilityStatusEffectModel;

        #endregion Class Methods
    }
}