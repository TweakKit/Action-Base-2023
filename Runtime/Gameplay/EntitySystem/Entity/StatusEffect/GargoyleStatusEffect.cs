using Runtime.Definition;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public class GargoyleStatusEffect : DurationStatusEffect<GargoyleStatusEffectModel>
    {
        #region Members

        private CharacterSpriteAnimationPlayer _characterSpriteAnimationPlayer;
        private CharacterWeaponSpriteAnimationActionPlayer _characterWeaponSpriteAnimationActionPlayer;
        private CharacterSkillSpriteAnimationActionPlayer _characterSkillSpriteAnimationActionPlayer;

        private readonly string _idleNormalAnimationName = "IdleNormal";
        private readonly string _idleBuffAnimationName = "IdleBuff";
        private readonly string _moveNormalAnimationName = "MoveNormal";
        private readonly string _moveBuffAnimationName = "MoveBuff";
        private readonly string _attackNormalAnimationName = "AttackNormal";
        private readonly string _attackBuffAnimationName = "AttackBuff";
        private readonly string _precastSkill2NormalAnimationName = "Precast_Skill_2_Normal";
        private readonly string _precastSkill2BuffAnimationName = "Precast_Skill_2_Buff";
        private readonly string _castSkill2NormalAnimationName = "Cast_Skill_2_Normal";
        private readonly string _castSkill2BuffAnimationName = "Cast_Skill_2_Buff";
        private readonly string _backswingSkill2NormalAnimationName = "Backswing_Skill_2_Normal";
        private readonly string _backswingSkill2BuffAnimationName = "Backswing_Skill_2_Buff";

        #endregion Members

        #region Properties

        protected override float Duration => ownerModel.Duration;
        protected float CounterDamagePercent => ownerModel.CounterDamagePercent;
        protected float IncreaseDamageReductionByPercent => ownerModel.IncreaseDamageReductionByPercent;

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
            base.StartAffect(receiverModel);
            receiverModel.BuffStat(StatType.DamageReduction, IncreaseDamageReductionByPercent, StatModifyType.BaseBonus);
            receiverModel.BuffStat(StatType.CounterDamage, ownerModel.CounterDamagePercent, StatModifyType.BaseBonus);
            receiverModel.StartGettingGargoyle();
            SetAnimationBuffCreator();
        }

        protected override void FinishAffect(CharacterModel receiverModel)
        {
            base.FinishAffect(receiverModel);
            receiverModel.DebuffStat(StatType.DamageReduction, IncreaseDamageReductionByPercent, StatModifyType.BaseBonus);
            receiverModel.DebuffStat(StatType.CounterDamage, ownerModel.CounterDamagePercent, StatModifyType.BaseBonus);
            receiverModel.StopGettingGargoyle();
            ResetAnimtionCreator();
        }

        public void SetAnimationBuffCreator()
        {
            if (_characterSpriteAnimationPlayer != null && _characterWeaponSpriteAnimationActionPlayer != null && _characterSkillSpriteAnimationActionPlayer != null)
            {
                _characterSpriteAnimationPlayer.SetNewAnimationName(CharacterAnimationState.Idle, _idleBuffAnimationName);
                _characterSpriteAnimationPlayer.SetNewAnimationName(CharacterAnimationState.Move, _moveBuffAnimationName);

                _characterWeaponSpriteAnimationActionPlayer.SetNewAnimationName(CharacterWeaponAnimationType.MeleeAttack, _attackBuffAnimationName);

                _characterSkillSpriteAnimationActionPlayer.SetNewAnimationName(SkillType.RockThrow, SkillActionPhase.Precast, _precastSkill2BuffAnimationName);
                _characterSkillSpriteAnimationActionPlayer.SetNewAnimationName(SkillType.RockThrow, SkillActionPhase.Cast, _castSkill2BuffAnimationName);
                _characterSkillSpriteAnimationActionPlayer.SetNewAnimationName(SkillType.RockThrow, SkillActionPhase.Backswing, _backswingSkill2BuffAnimationName);
            }
        }

        public void ResetAnimtionCreator()
        {
            if (_characterSpriteAnimationPlayer != null && _characterWeaponSpriteAnimationActionPlayer != null && _characterSkillSpriteAnimationActionPlayer != null)
            {
                _characterSpriteAnimationPlayer.SetNewAnimationName(CharacterAnimationState.Idle, _idleNormalAnimationName);
                _characterSpriteAnimationPlayer.SetNewAnimationName(CharacterAnimationState.Move, _moveNormalAnimationName);

                _characterWeaponSpriteAnimationActionPlayer.SetNewAnimationName(CharacterWeaponAnimationType.MeleeAttack, _attackNormalAnimationName);

                _characterSkillSpriteAnimationActionPlayer.SetNewAnimationName(SkillType.RockThrow, SkillActionPhase.Precast, _precastSkill2NormalAnimationName);
                _characterSkillSpriteAnimationActionPlayer.SetNewAnimationName(SkillType.RockThrow, SkillActionPhase.Cast, _castSkill2NormalAnimationName);
                _characterSkillSpriteAnimationActionPlayer.SetNewAnimationName(SkillType.RockThrow, SkillActionPhase.Backswing, _backswingSkill2NormalAnimationName);
            }
        }

        #endregion Class Methods
    }
}