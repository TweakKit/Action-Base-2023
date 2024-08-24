using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class KnockUpStatusEffectModel : StatusEffectModel
    {
        #region Members

        private float _knockUpHeight;
        private float _knockUpVelocity;

        #endregion Members

        #region Properties

        public override StatusEffectType StatusEffectType => StatusEffectType.KnockUp;
        public override bool IsStackable => false;
        public override bool IsOneShot => true;
        public float KnockUpHeight => _knockUpHeight;
        public float KnockUpVelocity => _knockUpVelocity;

        #endregion Properties

        #region Class Methods

        public KnockUpStatusEffectModel(float knockUpHeight, float knockUpVelocity, float chance = 1.0f) : base(chance)
        {
            _knockUpHeight = knockUpHeight;
            _knockUpVelocity = knockUpVelocity;
        }

        public override StatusEffectModel Clone()
            => MemberwiseClone() as KnockUpStatusEffectModel;

        #endregion Class Methods
    }
}