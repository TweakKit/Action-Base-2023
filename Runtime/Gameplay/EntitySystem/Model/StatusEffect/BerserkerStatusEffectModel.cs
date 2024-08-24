using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class BerserkerStatusEffectModel : DurationStatusEffectModel
    {
        #region Members

        private float _increasedAttackSpeed;
        private float _increasedAttackDamage;
        private float _increasedLifeSteal;

        #endregion Members

        #region Properties

        public override StatusEffectType StatusEffectType => StatusEffectType.Berserker;
        public override bool IsStackable => false;
        public float IncreasedAttackSpeed => _increasedAttackSpeed;
        public float IncreasedAttackDamage => _increasedAttackDamage;
        public float IncreasedLifeSteal => _increasedLifeSteal;

        #endregion Properties

        #region Class Methods

        public BerserkerStatusEffectModel(float increasedAttackSpeed, float increasedAttackDamage, float increasedLifeSteal,
                                          float duration, float chance = 1.0f)
            : base(duration, chance)
        {
            _increasedAttackSpeed = increasedAttackSpeed;
            _increasedAttackDamage = increasedAttackDamage;
            _increasedLifeSteal = increasedLifeSteal;
        }

        public override StatusEffectModel Clone()
            => MemberwiseClone() as BerserkerStatusEffectModel;

        #endregion Class Methods
    }
}