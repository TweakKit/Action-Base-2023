using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public class BerserkerStatusEffect : DurationStatusEffect<BerserkerStatusEffectModel>
    {
        #region Members

        private CharacterModel _berserkerCharacterModel;

        #endregion Members

        #region Properties

        protected override float Duration => ownerModel.Duration;

        #endregion Properties

        #region Class Methods

        protected override void StartAffect(CharacterModel receiverModel)
        {
            base.StartAffect(receiverModel);
            _berserkerCharacterModel = receiverModel;
            _berserkerCharacterModel.ReactionChangedEvent += OnBerserkerReactionChanged;
            receiverModel.BuffStat(StatType.AttackSpeed, ownerModel.IncreasedAttackSpeed, StatModifyType.BaseMultiply);
            receiverModel.BuffStat(StatType.AttackDamage, ownerModel.IncreasedAttackDamage, StatModifyType.BaseMultiply);
            receiverModel.BuffStat(StatType.LifeSteal, ownerModel.IncreasedLifeSteal, StatModifyType.BaseBonus);
        }

        protected override void FinishAffect(CharacterModel receiverModel)
        {
            base.FinishAffect(receiverModel);
            _berserkerCharacterModel.ReactionChangedEvent -= OnBerserkerReactionChanged;
            receiverModel.DebuffStat(StatType.AttackSpeed, ownerModel.IncreasedAttackSpeed, StatModifyType.BaseMultiply);
            receiverModel.DebuffStat(StatType.AttackDamage, ownerModel.IncreasedAttackDamage, StatModifyType.BaseMultiply);
            receiverModel.DebuffStat(StatType.LifeSteal, ownerModel.IncreasedLifeSteal, StatModifyType.BaseBonus);
        }

        private void OnBerserkerReactionChanged(CharacterReactionType characterReactionType)
        {
            if (characterReactionType == CharacterReactionType.JustFinishAttack)
            {
                var lastAttackedTarget = _berserkerCharacterModel.CurrentAttackedTarget;
                if (lastAttackedTarget.Model.EntityType.IsCharacter())
                {
                    var lastAttackedTargetCharacterModel = lastAttackedTarget.Model as CharacterModel;
                    if (lastAttackedTargetCharacterModel.IsInBleedStatus)
                    {
                        ResetDuration();
                        return;
                    }
                }
            }
        }

        #endregion Class Methods
    }
}