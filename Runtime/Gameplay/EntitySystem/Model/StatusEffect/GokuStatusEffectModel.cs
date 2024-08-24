using Runtime.Definition;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class GokuStatusEffectModel : DurationStatusEffectModel
    {
        #region Members

        private float _increaseAtkPercent;
        private float _increaseAtkSpeedPercent;
        private float _increaseCritChance;
        private bool _isDamageBehind;
        private float _duration;
        private Transform _creatorTransform;

        #endregion Members

        #region Properties

        public override StatusEffectType StatusEffectType => StatusEffectType.Goku;
        public override bool IsStackable => false;
        public float IncreaseAtkPercent => _increaseAtkPercent;
        public float IncreaseAtkSpeedPercent => _increaseAtkSpeedPercent;
        public float IncreaseCritChance => _increaseCritChance;
        public bool IsDamageBehind => _isDamageBehind;
        public float Duration => _duration;
        public Transform CreatorTransform => _creatorTransform;
        
        #endregion Properties

        #region Class Methods

        public GokuStatusEffectModel(Transform creatorTransform, 
                                     float increaseAtkPercent,
                                     float increaseAtkSpeedPercent,
                                     float increaseCritChance,
                                     bool isDamageBehind,
                                     float duration, 
                                     float chance = 1.0f)
                                     : base(duration, chance)
        {
            _increaseAtkPercent = increaseAtkPercent;
            _increaseAtkSpeedPercent = increaseAtkSpeedPercent;
            _increaseCritChance = increaseCritChance;
            _isDamageBehind = isDamageBehind;
            _duration = duration;
            _creatorTransform = creatorTransform;
        }
        
        public override StatusEffectModel Clone()
            => MemberwiseClone() as GokuStatusEffectModel;
        
        #endregion Class Methods
    }
}