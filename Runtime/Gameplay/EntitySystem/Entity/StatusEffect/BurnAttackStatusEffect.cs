using UnityEngine;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class BurnAttackStatusEffect : PerIntervalDurationStatusEffect<BurnAttackStatusEffectModel>
    {
        #region Members

        private float _burnDamage;
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
            _burnDamage = attackDamage * ownerModel.BurnDamageByAttackPercent;
        }

        protected override void AffectPerInterval(CharacterModel receiverModel)
        {
            base.AffectPerInterval(receiverModel);
            var damageInfo = new DamageInfo(DamageSource.FromSkill, _burnDamage, null, _senderModel, receiverModel);
            receiverModel.DebuffHp(damageInfo);
        }

        #endregion Class Methods
    }
}