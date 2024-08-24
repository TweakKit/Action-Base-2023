using Runtime.Definition;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class GokuStatusEffect : DurationStatusEffect<GokuStatusEffectModel>
    {
        #region Members

        private CharacterSpriteAnimationPlayer _characterSpriteAnimationPlayer;
        private CharacterWeaponSpriteAnimationActionPlayer _characterWeaponSpriteAnimationActionPlayer;
        private CharacterSkillSpriteAnimationActionPlayer _characterSkillSpriteAnimationActionPlayer;

        private readonly string _idleNormalAnimationName = "Idle";
        private readonly string _idleBuffAnimationName = "Idle_Buff";
        private readonly string _moveNormalAnimationName = "Move";
        private readonly string _moveBuffAnimationName = "Move_Buff";
        private readonly string _attackNormalAnimationName = "Attack";
        private readonly string _attackBuffAnimationName = "Attack_Buff";
        private readonly string _chopNormalAnimationName = "Chop";
        private readonly string _chopBuffAnimationName = "Chop_Buff";
        private readonly string _miningNormalAnimationName = "Mining";
        private readonly string _miningBuffAnimationName = "Mining_Buff";
        private readonly string _skill1NormalAnimationName = "Skill_1";
        private readonly string _skill1BuffAnimationName = "Skill_1_Buff";

        #endregion Members

        #region Properties

        protected override float Duration => ownerModel.Duration;
        protected float IncreaseAtkPercent => ownerModel.IncreaseAtkPercent;
        protected float IncreaseAtkSpeedPercent => ownerModel.IncreaseAtkSpeedPercent;
        protected float IncreaseCritChance => ownerModel.IncreaseCritChance;
        protected bool IsDamageBehind => ownerModel.IsDamageBehind;

        #endregion Properties

        #region Class Methods

        protected override void InitData(EntityModel senderModel)
        {
            base.InitData(senderModel);
            _characterSpriteAnimationPlayer = ownerModel.CreatorTransform.GetComponentInChildren<CharacterSpriteAnimationPlayer>();
            _characterWeaponSpriteAnimationActionPlayer = ownerModel.CreatorTransform.GetComponentInChildren<CharacterWeaponSpriteAnimationActionPlayer>();
            _characterSkillSpriteAnimationActionPlayer = ownerModel.CreatorTransform.GetComponentInChildren<CharacterSkillSpriteAnimationActionPlayer>();
#if UNITY_EDITOR
            if (_characterSpriteAnimationPlayer == null || _characterWeaponSpriteAnimationActionPlayer == null || _characterSkillSpriteAnimationActionPlayer == null)
                Debug.LogError("Null animation player");
#endif
        }

        protected override void StartAffect(CharacterModel receiverModel)
        {
            SetAnimationBuffCreator();
            base.StartAffect(receiverModel);
            receiverModel.BuffStat(StatType.AttackDamage, IncreaseAtkPercent, StatModifyType.BaseMultiply);
            receiverModel.BuffStat(StatType.AttackSpeed, IncreaseAtkSpeedPercent, StatModifyType.BaseMultiply);
            receiverModel.BuffStat(StatType.CritChance, IncreaseCritChance, StatModifyType.BaseBonus);
            receiverModel.CanDealDamageBehindTarget = IsDamageBehind;
        }

        protected override void FinishAffect(CharacterModel receiverModel)
        {
            ResetAnimtionCreator();
            base.FinishAffect(receiverModel);
            receiverModel.DebuffStat(StatType.AttackDamage, IncreaseAtkPercent, StatModifyType.BaseMultiply);
            receiverModel.DebuffStat(StatType.AttackSpeed, IncreaseAtkSpeedPercent, StatModifyType.BaseMultiply);
            receiverModel.DebuffStat(StatType.CritChance, IncreaseCritChance, StatModifyType.BaseBonus);
            receiverModel.CanDealDamageBehindTarget = false;
        }

        public void SetAnimationBuffCreator()
        {
            if (_characterSpriteAnimationPlayer != null && _characterWeaponSpriteAnimationActionPlayer != null && _characterSkillSpriteAnimationActionPlayer != null)
            {
                _characterSpriteAnimationPlayer.SetNewAnimationName(CharacterAnimationState.Idle, _idleBuffAnimationName);
                _characterSpriteAnimationPlayer.SetNewAnimationName(CharacterAnimationState.Move, _moveBuffAnimationName);

                _characterWeaponSpriteAnimationActionPlayer.SetNewAnimationName(CharacterWeaponAnimationType.MeleeAttack, _attackBuffAnimationName);
                _characterWeaponSpriteAnimationActionPlayer.SetNewAnimationName(CharacterWeaponAnimationType.ChopAttack, _chopBuffAnimationName);
                _characterWeaponSpriteAnimationActionPlayer.SetNewAnimationName(CharacterWeaponAnimationType.MiningAttack, _miningBuffAnimationName);

                _characterSkillSpriteAnimationActionPlayer.SetNewAnimationName(SkillType.Kame, SkillActionPhase.Cast, _skill1BuffAnimationName);
            }
        }

        public void ResetAnimtionCreator()
        {
            if (_characterSpriteAnimationPlayer != null && _characterWeaponSpriteAnimationActionPlayer != null && _characterSkillSpriteAnimationActionPlayer != null)
            {
                _characterSpriteAnimationPlayer.SetNewAnimationName(CharacterAnimationState.Idle, _idleNormalAnimationName);
                _characterSpriteAnimationPlayer.SetNewAnimationName(CharacterAnimationState.Move, _moveNormalAnimationName);

                _characterWeaponSpriteAnimationActionPlayer.SetNewAnimationName(CharacterWeaponAnimationType.MeleeAttack, _attackNormalAnimationName);
                _characterWeaponSpriteAnimationActionPlayer.SetNewAnimationName(CharacterWeaponAnimationType.ChopAttack, _chopNormalAnimationName);
                _characterWeaponSpriteAnimationActionPlayer.SetNewAnimationName(CharacterWeaponAnimationType.MiningAttack, _miningNormalAnimationName);

                _characterSkillSpriteAnimationActionPlayer.SetNewAnimationName(SkillType.Kame, SkillActionPhase.Cast, _skill1NormalAnimationName);
            }
        }

        #endregion Class Methods
    }

    
}