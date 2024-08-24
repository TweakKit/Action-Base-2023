using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class EvasionBuffStatusEffectModel : DurationStatusEffectModel
    {
        #region Members

        private float _increasedEvasion;

        #endregion Members

        #region Properties

        public override StatusEffectType StatusEffectType => StatusEffectType.EvasionBuff;
        public override bool IsStackable => false;
        public float IncreasedEvasion => _increasedEvasion;

        #endregion Properties

        #region Class Methods

        public EvasionBuffStatusEffectModel(float increasedEvasion, float duration, float chance = 1.0f) : base(duration, chance)
            => _increasedEvasion = increasedEvasion;

        public override StatusEffectModel Clone()
            => MemberwiseClone() as EvasionBuffStatusEffectModel;

        #endregion Class Methods
    }
}