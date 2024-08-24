using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class SlowStatusEffectModel : DurationStatusEffectModel
    {
        #region Members

        private float _decreasedMoveSpeed;

        #endregion Members

        #region Properties

        public override StatusEffectType StatusEffectType => StatusEffectType.Slow;
        public override bool IsStackable => false;
        public float DecreasedMoveSpeed => _decreasedMoveSpeed;

        #endregion Properties

        #region Class Methods

        public SlowStatusEffectModel(float decreasedMoveSpeed, float duration, float chance = 1.0f) : base(duration, chance)
            => _decreasedMoveSpeed = decreasedMoveSpeed;

        public override StatusEffectModel Clone()
            => MemberwiseClone() as SlowStatusEffectModel;

        #endregion Class Methods
    }
}