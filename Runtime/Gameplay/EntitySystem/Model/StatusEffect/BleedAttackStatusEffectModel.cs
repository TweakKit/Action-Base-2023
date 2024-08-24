using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class BleedAttackStatusEffectModel : DurationStatusEffectModel
    {
        #region Members

        private float _bleedDamageByAttackPercent;

        #endregion Members

        #region Properties

        public override StatusEffectType StatusEffectType => StatusEffectType.BleedAttack;
        public override bool IsStackable => false;
        public float BleedDamageByAttackPercent => _bleedDamageByAttackPercent;

        #endregion Properties

        #region Class Methods

        public BleedAttackStatusEffectModel(float bleedDamageByAttackPercent, float duration, float chance = 1.0f) : base(duration, chance)
            => _bleedDamageByAttackPercent = bleedDamageByAttackPercent;

        public override StatusEffectModel Clone()
            => MemberwiseClone() as BleedAttackStatusEffectModel;

        #endregion Class Methods
    }
}