using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class FixedDamageReductionNonDurationStatusEffectModel : NonDurationStatusEffectModel
    {
        #region Members

        private float _increasedFixedDamageReduction;

        #endregion Members

        #region Properties

        public override StatusEffectType StatusEffectType => StatusEffectType.FixedDamageReductionNonDuration;
        public override bool IsStackable => false;
        public override bool IsOneShot => false;
        public float IncreasedFixedDamageReduction => _increasedFixedDamageReduction;

        #endregion Properties

        #region Class Methods

        public FixedDamageReductionNonDurationStatusEffectModel(float increasedFixedDamageReduction, float chance = 1.0f) : base(chance)
            => _increasedFixedDamageReduction = increasedFixedDamageReduction;

        public override StatusEffectModel Clone()
            => MemberwiseClone() as FixedDamageReductionNonDurationStatusEffectModel;

        #endregion Class Methods
    }
}