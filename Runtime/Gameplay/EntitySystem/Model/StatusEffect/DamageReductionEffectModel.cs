using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class DamageReductionStatusEffectModel : DurationStatusEffectModel
    {
        #region Members

        private float _increasedDamageReduction;

        #endregion Members

        #region Properties

        public override StatusEffectType StatusEffectType => StatusEffectType.DamageReductionBuff;
        public override bool IsStackable => false;
        public float IncreasedDamageReduction => _increasedDamageReduction;

        #endregion Properties

        #region Class Methods

        public DamageReductionStatusEffectModel(float increasedDamageReduction, float duration, float chance = 1.0f) : base(duration, chance)
            => _increasedDamageReduction = increasedDamageReduction;

        public override StatusEffectModel Clone()
            => MemberwiseClone() as DamageReductionStatusEffectModel;

        #endregion Class Methods
    }
}