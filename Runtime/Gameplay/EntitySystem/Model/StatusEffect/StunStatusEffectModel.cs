using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class StunStatusEffectModel : DurationStatusEffectModel
    {
        #region Properties

        public override StatusEffectType StatusEffectType => StatusEffectType.Stun;
        public override bool IsStackable => false;

        #endregion Properties

        #region Class Methods

        public StunStatusEffectModel(float duration, float chance = 1.0f) : base(duration, chance) { }

        public override StatusEffectModel Clone()
            => MemberwiseClone() as StunStatusEffectModel;

        #endregion Class Methods
    }
}