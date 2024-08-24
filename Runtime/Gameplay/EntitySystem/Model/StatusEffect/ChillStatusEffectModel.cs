using System.Collections.Generic;
using Runtime.Definition;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class ChillStatusEffectModel : DurationStatusEffectModel
    {
        #region Members

        private float _decreasedSpeed;
        private int _countStack;

        #endregion Members

        #region Properties

        public override StatusEffectType StatusEffectType => StatusEffectType.Chill;
        public override bool IsStackable => true;
        public float DecreasedSpeed => _decreasedSpeed;

        #endregion Properties

        #region Class Methods

        public ChillStatusEffectModel(float decreasedSpeed, float duration, float chance = 1.0f) : base(duration, chance)
        {
            _decreasedSpeed = decreasedSpeed;
            SetMaxStack(Constant.COUNT_STACK_CHILL_TO_FREEZE);
            _countStack = 0;
        }

        public override void Stack(StatusEffectModel stackedStatusEffectModel)
        {
            var stackedChillStatusEffectModel = stackedStatusEffectModel as ChillStatusEffectModel;
            duration = Mathf.Max(duration, stackedChillStatusEffectModel.duration);
            _countStack = stackedChillStatusEffectModel._countStack + 1;
            if (_countStack < MaxStack)
            {
                bonusDuration += stackedChillStatusEffectModel.bonusDuration;
                _decreasedSpeed += stackedChillStatusEffectModel.DecreasedSpeed;
            }
        }

        public override StatusEffectModel Clone()
            => MemberwiseClone() as ChillStatusEffectModel;

        #endregion Class Methods
    }
}