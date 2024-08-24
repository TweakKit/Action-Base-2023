using Runtime.Definition;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class GetMoreDamageStatusEffectModel : DurationStatusEffectModel
    {
        #region Members

        private float _getMoreDamagePercent;
        private float _duration;

        #endregion Members

        #region Properties

        public override StatusEffectType StatusEffectType => StatusEffectType.GetMoreDamage;
        public override bool IsStackable => false;
        public float GetMoreDamagePercent => _getMoreDamagePercent;
        public float Duration => _duration;

        #endregion Properties

        #region Class Methods

        public GetMoreDamageStatusEffectModel(float getMoreDamagePercent,
                                              float duration, float chance = 1.0f)
            : base(duration, chance)
        {
            _getMoreDamagePercent = getMoreDamagePercent;
            _duration = duration;
        }

        public override StatusEffectModel Clone()
            => MemberwiseClone() as GetMoreDamageStatusEffectModel;

        #endregion Class Methods
    }
}