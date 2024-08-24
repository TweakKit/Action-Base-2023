using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class FixedDamageReductionStatusEffectModel : DurationStatusEffectModel
    {
        #region Members

        private float _increasedFixedDamageReduction;

        #endregion Members

        #region Properties

        public override StatusEffectType StatusEffectType => StatusEffectType.FixedDamageReductionBuff;
        public override bool IsStackable => false;
        public float IncreasedFixedDamageReduction => _increasedFixedDamageReduction;

        #endregion Properties

        #region Class Methods

        public FixedDamageReductionStatusEffectModel(float increasedFixedDamageReduction, float duration, float chance = 1.0f) : base(duration, chance)
            => _increasedFixedDamageReduction = increasedFixedDamageReduction;

        public override StatusEffectModel Clone()
            => MemberwiseClone() as FixedDamageReductionStatusEffectModel;

        #endregion Class Methods
    }
}