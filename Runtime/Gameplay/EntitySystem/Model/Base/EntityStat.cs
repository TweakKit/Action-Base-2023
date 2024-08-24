using System;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public struct EntityStat
    {
        #region Members

        private float _baseValue;
        private float _baseMultiply;
        private float _baseBonus;

        #endregion Members

        #region Properties

        public float TotalValue => (_baseValue + _baseBonus) * _baseMultiply;
        public float BaseValue => _baseValue;

        #endregion Properties

        #region Class Methods

        public EntityStat(float baseValue)
        {
            _baseValue = baseValue;
            _baseMultiply = 1.0f;
            _baseBonus = 0.0f;
        }

        public void SetBaseValue(float value)
            => _baseValue = value;

        public void BuffValue(float value, StatModifyType statModifyType)
        {
            switch (statModifyType)
            {
                case StatModifyType.BaseBonus:
                    _baseBonus += value;
                    break;

                case StatModifyType.BaseMultiply:
                    _baseMultiply += value;
                    break;
            }
        }

        public void DebuffValue(float value, StatModifyType statModifyType)
        {
            switch (statModifyType)
            {
                case StatModifyType.BaseBonus:
                    _baseBonus = (float)Math.Round(_baseBonus - value, 2);
                    break;

                case StatModifyType.BaseMultiply:
                    _baseMultiply = (float)Math.Round(_baseMultiply - value, 2);
                    break;
            }
        }

        #endregion Class Methods
    }
}