using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class HealAttackStatusEffectModel : StatusEffectModel
    {
        #region Members

        private float _hpIncreaseByAttackPercent;

        #endregion Members

        #region Properties

        public override StatusEffectType StatusEffectType => StatusEffectType.HealAttack;
        public override bool IsStackable => false;
        public override bool IsOneShot => true;
        public float HpIncreaseByAttackPercent => _hpIncreaseByAttackPercent;

        #endregion Properties

        #region Class Methods

        public HealAttackStatusEffectModel(float hpIncreaseByAttackPercent, float chance = 1.0f) : base(chance)
            => _hpIncreaseByAttackPercent = hpIncreaseByAttackPercent;

        public override StatusEffectModel Clone()
            => MemberwiseClone() as HealAttackStatusEffectModel;

        #endregion Class Methods
    }
}