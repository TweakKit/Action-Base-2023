using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class DamageReductionNonDurationStatusEffectModel : NonDurationStatusEffectModel
    {
        #region Members

        private float _increasedDamageReduction;

        #endregion Members

        #region Properties

        public override StatusEffectType StatusEffectType => StatusEffectType.DamageReductionNonDuration;
        public override bool IsStackable => false;
        public override bool IsOneShot => false;
        public float IncreasedDamageReduction => _increasedDamageReduction;

        #endregion Properties

        #region Class Methods

        public DamageReductionNonDurationStatusEffectModel(float increasedDamageReduction, float chance = 1.0f) : base(chance)
            => _increasedDamageReduction = increasedDamageReduction;

        public override StatusEffectModel Clone()
            => MemberwiseClone() as DamageReductionNonDurationStatusEffectModel;

        #endregion Class Methods
    }
}