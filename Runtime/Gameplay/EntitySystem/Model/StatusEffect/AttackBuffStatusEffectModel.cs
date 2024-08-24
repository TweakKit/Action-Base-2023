using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class AttackBuffStatusEffectModel : DurationStatusEffectModel
    {
        #region Properties

        public override StatusEffectType StatusEffectType => StatusEffectType.AttackBuff;
        public override bool IsStackable => false;
        public float IncreasedAttack { get; }

        public StatModifyType StatModifyType { get; }

        #endregion Properties

        #region Class Methods

        public AttackBuffStatusEffectModel(float increasedAttack, float duration, float chance = 1.0f, StatModifyType statModifyType = StatModifyType.BaseBonus)
            : base(duration, chance)
        {
            IncreasedAttack = increasedAttack;
            StatModifyType = statModifyType;
        }

        public override StatusEffectModel Clone()
            => MemberwiseClone() as AttackBuffStatusEffectModel;

        #endregion Class Methods
    }
}