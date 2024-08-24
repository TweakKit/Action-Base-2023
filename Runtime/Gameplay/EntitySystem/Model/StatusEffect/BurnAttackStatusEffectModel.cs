using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class BurnAttackStatusEffectModel : DurationStatusEffectModel
    {
        #region Members

        private float _burnDamageByAttackPercent;

        #endregion Members

        #region Properties

        public override StatusEffectType StatusEffectType => StatusEffectType.BurnAttack;
        public override bool IsStackable => false;
        public float BurnDamageByAttackPercent => _burnDamageByAttackPercent;

        #endregion Properties

        #region Class Methods

        public BurnAttackStatusEffectModel(float BurnDamageByAttackPercent, float duration, float chance = 1.0f) : base(duration, chance)
            => _burnDamageByAttackPercent = BurnDamageByAttackPercent;

        public override StatusEffectModel Clone()
            => MemberwiseClone() as BurnAttackStatusEffectModel;

        #endregion Class Methods
    }
}