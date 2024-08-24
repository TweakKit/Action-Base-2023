using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class HealStatusEffectModel : StatusEffectModel
    {
        #region Members

        private float _healAmount;

        #endregion Members

        #region Properties

        public override StatusEffectType StatusEffectType => StatusEffectType.Heal;
        public override bool IsStackable => false;
        public override bool IsOneShot => true;
        public float HealAmount => _healAmount;

        #endregion Properties

        #region Class Methods

        public HealStatusEffectModel(float healAmount, float chance = 1.0f) : base(chance)
            => _healAmount = healAmount;

        public override StatusEffectModel Clone()
            => MemberwiseClone() as HealStatusEffectModel;

        #endregion Class Methods
    }
}