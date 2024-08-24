using System;
using UnityEngine;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    [Serializable]
    public class HealAttackStatusEffect : OneShotStatusEffect<HealAttackStatusEffectModel>
    {
        #region Members

        private float _healAmount;

        #endregion Members

        #region Class Methods

        public override void Init(StatusEffectModel statusEffectModel, EntityModel senderModel, CharacterModel receiverModel, Vector2 statusEffectDirection)
        {
            base.Init(statusEffectModel, senderModel, receiverModel, statusEffectDirection);
        }

        protected override void InitData(EntityModel senderModel)
        {
            base.InitData(senderModel);
            var attackDamage = senderModel.GetTotalStatValue(StatType.AttackDamage);
            _healAmount = attackDamage * ownerModel.HpIncreaseByAttackPercent;
        }

        protected override void StartAffect(CharacterModel receiverModel)
        {
            base.StartAffect(receiverModel);
            receiverModel.BuffHp(_healAmount);
        }

        #endregion Class Methods
    }
}