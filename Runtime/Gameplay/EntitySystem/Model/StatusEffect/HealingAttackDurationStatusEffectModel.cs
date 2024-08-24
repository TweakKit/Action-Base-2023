using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class HealingAttackDurationStatusEffectModel : DurationStatusEffectModel
    {
        #region Members

        private float _healingByAttackPercent;

        #endregion Members

        #region Properties

        public override StatusEffectType StatusEffectType => StatusEffectType.HealingAttackDuration;
        public override bool IsStackable => false;
        public float HealingByAttackPercent => _healingByAttackPercent;

        #endregion Properties

        #region Class Methods

        public HealingAttackDurationStatusEffectModel(float healingByAttackPercent, float duration, float chance = 1.0f) : base(duration, chance)
            => _healingByAttackPercent = healingByAttackPercent;

        public override StatusEffectModel Clone()
            => MemberwiseClone() as HealingAttackDurationStatusEffectModel;

        #endregion Class Methods
    }
}