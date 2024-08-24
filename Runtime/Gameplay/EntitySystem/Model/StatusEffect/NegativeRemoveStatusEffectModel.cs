using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class NegativeRemoveStatusEffectModel : StatusEffectModel
    {
        #region Properties

        public override StatusEffectType StatusEffectType => StatusEffectType.NegativeStatusEffectRemove;
        public override bool IsStackable => false;
        public override bool IsOneShot => true;

        #endregion Properties

        #region Class Methods

        public NegativeRemoveStatusEffectModel(float chance = 1.0f) : base(chance) { }

        public override StatusEffectModel Clone()
            => MemberwiseClone() as NegativeRemoveStatusEffectModel;

        #endregion Class Methods
    }
}