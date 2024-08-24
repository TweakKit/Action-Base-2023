using UnityEngine;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class BleedAttackStatusEffect : PerIntervalDurationStatusEffect<BleedAttackStatusEffectModel>
    {
        #region Members

        private float _bleedDamage;
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
            _bleedDamage = attackDamage * ownerModel.BleedDamageByAttackPercent;
        }

        protected override void AffectPerInterval(CharacterModel receiverModel)
        {
            base.AffectPerInterval(receiverModel);
            var damageInfo = new DamageInfo(DamageSource.FromSkill, _bleedDamage, null, _senderModel, receiverModel);
            receiverModel.DebuffHp(damageInfo);
        }

        #endregion Class Methods
    }
}