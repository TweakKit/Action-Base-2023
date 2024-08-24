using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class ShieldStatusEffect : DurationStatusEffect<ShieldStatusEffectModel>
    {
        #region Properties

        protected override float Duration => ownerModel.Duration;

        #endregion Properties

        #region Class Methods

        protected override void StartAffect(CharacterModel receiverModel)
        {
            base.StartAffect(receiverModel);
            var hp = receiverModel.GetTotalStatValue(StatType.HealthPoint);
            var shieldPointBuff = receiverModel.MaxHp * ownerModel.ShieldByHpPercent;
            receiverModel.BuffStat(StatType.ShieldPoint, shieldPointBuff, StatModifyType.BaseBonus);
        }

        protected override void FinishAffect(CharacterModel receiverModel)
        {
            base.FinishAffect(receiverModel);
            var shieldPointBuff = receiverModel.MaxHp * ownerModel.ShieldByHpPercent;
            receiverModel.DebuffStat(StatType.ShieldPoint, shieldPointBuff, StatModifyType.BaseBonus);
        }

        #endregion Class Methods
    }
}