using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class DecreaseTakeHpStatusEffectModel : DurationStatusEffectModel
    {
        #region Members

        private float _decreaseTakeHpPercent;

        #endregion Members
        
        #region Properties

        public override StatusEffectType StatusEffectType => StatusEffectType.DecreaseTakeHp;
        public override bool IsStackable => false;
        public float DecreaseTakeHpPercent => _decreaseTakeHpPercent;

        #endregion Properties

        #region Class Methods

        public DecreaseTakeHpStatusEffectModel(float decreaseTakeHpPercent, float duration, float chance = 1.0f) : base(duration, chance)
        {
            _decreaseTakeHpPercent = decreaseTakeHpPercent;
        }

        public override StatusEffectModel Clone()
            => MemberwiseClone() as DecreaseTakeHpStatusEffectModel;

        #endregion Class Methods
    }
}