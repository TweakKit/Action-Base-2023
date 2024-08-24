using Runtime.Definition;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class GargoyleStatusEffectModel : DurationStatusEffectModel
    {
        #region Members

        private float _counterDamagePercent;
        private float _increaseDamageReductionByPercent;
        private float _duration;
        private Transform _creatorTransform;

        #endregion Members

        #region Properties

        public override StatusEffectType StatusEffectType => StatusEffectType.Gargoyle;
        public override bool IsStackable => false;
        public float CounterDamagePercent => _counterDamagePercent;
        public float IncreaseDamageReductionByPercent => _increaseDamageReductionByPercent;
        public float Duration => _duration;
        public Transform CreatorTransform => _creatorTransform;

        #endregion Properties

        #region Class Methods

        public GargoyleStatusEffectModel(Transform creatorTransform, float counterDamagePercent,float increaseDamageReductionByPercent,
                                         float duration, float chance = 1.0f)
            : base(duration, chance)
        {
            _counterDamagePercent = counterDamagePercent;
            _increaseDamageReductionByPercent = increaseDamageReductionByPercent;
            _duration = duration;
            _creatorTransform = creatorTransform;
        }

        public override StatusEffectModel Clone()
            => MemberwiseClone() as GargoyleStatusEffectModel;

        #endregion Class Methods
    }
}