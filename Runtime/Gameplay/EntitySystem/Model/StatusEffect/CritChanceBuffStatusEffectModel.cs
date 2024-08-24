using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class CritChanceBuffStatusEffectModel : DurationStatusEffectModel
    {
        #region Members

        private float _increasedCritChance;

        #endregion Members

        #region Properties

        public override StatusEffectType StatusEffectType => StatusEffectType.CritChanceBuff;
        public override bool IsStackable => false;
        public float IncreasedCritChance => _increasedCritChance;

        #endregion Properties

        #region Class Methods

        public CritChanceBuffStatusEffectModel(float increasedCritChance, float duration, float chance = 1.0f) : base(duration, chance)
            => _increasedCritChance = increasedCritChance;

        public override StatusEffectModel Clone()
            => MemberwiseClone() as CritChanceBuffStatusEffectModel;

        #endregion Class Methods
    }
}