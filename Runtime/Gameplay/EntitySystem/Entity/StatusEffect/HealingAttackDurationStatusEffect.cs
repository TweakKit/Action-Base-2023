using UnityEngine;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class HealingAttackDurationStatusEffect : PerIntervalDurationStatusEffect<HealingAttackDurationStatusEffectModel>
    {
        #region Members

        private float _healAmount;
        private EntityModel _senderModel;

        #endregion Members

        #region Properties

        protected override float Interval => 1.0f;
        protected override float Duration => ownerModel.Duration;

        #endregion Properties

        #region Class Methods

        public override void Init(StatusEffectModel statusEffectModel, EntityModel senderModel, CharacterModel receiverModel, Vector2 statusEffectDirection)
        {
            base.Init(statusEffectModel, senderModel, receiverModel, statusEffectDirection);
            _senderModel = senderModel;
        }

        protected override void InitData(EntityModel senderModel)
        {
            base.InitData(senderModel);
            var attackDamage = senderModel.GetTotalStatValue(StatType.AttackDamage);
            _healAmount = attackDamage * ownerModel.HealingByAttackPercent;
        }

        protected override void AffectPerInterval(CharacterModel receiverModel)
        {
            base.AffectPerInterval(receiverModel);
            receiverModel.BuffHp(_healAmount);
        }

        #endregion Class Methods
    }
}