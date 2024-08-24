using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class ShieldStatusEffectModel : DurationStatusEffectModel
    {
        #region Members

        private float _shieldByHpPercent;

        #endregion Members

        #region Properties

        public override StatusEffectType StatusEffectType => StatusEffectType.Shield;
        public override bool IsStackable => false;
        public float ShieldByHpPercent => _shieldByHpPercent;

        #endregion Properties

        #region Class Methods

        public ShieldStatusEffectModel(float shieldByHpPercent, float duration, float chance = 1.0f) : base(duration, chance)
            => _shieldByHpPercent = shieldByHpPercent;

        public override StatusEffectModel Clone()
            => MemberwiseClone() as ShieldStatusEffectModel;

        #endregion Class Methods
    }
}