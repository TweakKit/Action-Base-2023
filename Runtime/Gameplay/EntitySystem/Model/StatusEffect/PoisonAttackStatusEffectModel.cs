using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class PoisonAttackStatusEffectModel : DurationStatusEffectModel
    {
        #region Members

        private float _poisonDamageByAttackPercent;

        #endregion Members

        #region Properties

        public override StatusEffectType StatusEffectType => StatusEffectType.PoisonAttack;
        public override bool IsStackable => false;
        public float PoisonDamageByAttackPercent => _poisonDamageByAttackPercent;

        #endregion Properties

        #region Class Methods

        public PoisonAttackStatusEffectModel(float poisonDamageByAttackPercent, float duration, float chance = 1.0f) : base(duration, chance)
            => _poisonDamageByAttackPercent = poisonDamageByAttackPercent;

        public override StatusEffectModel Clone()
            => MemberwiseClone() as PoisonAttackStatusEffectModel;

        #endregion Class Methods
    }
}