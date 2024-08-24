using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class FreezeStatusEffectModel : DurationStatusEffectModel
    {
        #region Properties

        public override StatusEffectType StatusEffectType => StatusEffectType.Freeze;
        public override bool IsStackable => false;

        #endregion Properties

        #region Class Methods

        public FreezeStatusEffectModel(float duration, float chance = 1.0f) : base(duration, chance) { }

        public override StatusEffectModel Clone()
            => MemberwiseClone() as FreezeStatusEffectModel;

        #endregion Class Methods
    }
}