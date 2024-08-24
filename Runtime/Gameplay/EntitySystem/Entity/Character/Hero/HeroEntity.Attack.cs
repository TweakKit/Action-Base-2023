using System;
using System.Collections.Generic;
using Runtime.Definition;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class HeroEntity : CharacterEntity<HeroModel>
    {
        #region Members

        private bool _hasSkills;
        private float _skillMaxCastRange;
        private int _currentlyUsedSkillIndex;
        private ISkillStrategy[] _skillStrategies;

        #endregion Members

        #region Properties

        /// <summary>
        /// The attack range of the character.
        /// The returned attack range should be the smallest range of all ranges (skill range, attack range, interaction range,..).
        /// Because it's used to determine where the character should stop chasing the target.
        /// </summary>
        protected override float AttackRange
        {
            get
            {
                if (ownerModel.CurrentTargetedTarget.IsObject)
                    return Constant.HEROES_INTERACT_OBJECTS_DISTANCE;
                else
                    return ownerModel.GetTotalStatValue(StatType.AttackRange);
            }
        }

        #endregion Properties

        #region Class Methods

        protected override void PartialInitializeAttack()
        {
            base.PartialInitializeAttack();
            attackStrategy = AttackStrategyFactory.GetAttackStrategy(ownerModel.EntityType.IsHero(), ownerModel.AttackType);
            attackStrategy.Init(ownerModel, transform);
            _skillMaxCastRange = -1;
            _currentlyUsedSkillIndex = -1;
            var skillModels = ownerModel.SkillModels;
            skillModels.Sort();
            if (skillModels != null && skillModels.Count > 0)
            {
                _hasSkills = true;
                _skillStrategies = new ISkillStrategy[skillModels.Count];
                var cooldownReduction = ownerModel.GetTotalStatValue(StatType.CooldownReduction);
                for (int i = 0; i < skillModels.Count; i++)
                {
                    if (skillModels[i].IsSkillTakeBreakByCooldown)
                        skillModels[i].Cooldown *= (1 - cooldownReduction);

                    if (skillModels[i].CastRange > 0 && skillModels[i].CastRange > _skillMaxCastRange)
                        _skillMaxCastRange = skillModels[i].CastRange;

                    var skillStrategy = SkillStrategyFactory.GetSkillStrategy(skillModels[i].SkillType);
                    skillStrategy.Init(skillModels[i], ownerModel, transform);
                    _skillStrategies[i] = skillStrategy;
                }
            }
            else _hasSkills = false;

            ownerModel.SkillStrategyUpdatedEvent += OnSkillStrategyUpdated;
            ownerModel.ReactionChangedEvent += OnReactionChangedOnAttack;
        }

        protected override void PartialDisposeAttack()
        {
            base.PartialDisposeAttack();
            if (_hasSkills)
            {
                foreach (var skillStrategy in _skillStrategies)
                    skillStrategy.Dispose();
            }
        }

        protected override bool CanTriggerAttack()
        {
            if (ownerModel.CurrentTargetedTarget.IsObject)
            {
                if (attackStrategy != null)
                {
                    if (attackStrategy.CheckCanAttack())
                        return IsTargetInAttackTriggeredRange(AttackRange);
                }

                return false;
            }
            else
            {
                var readySkillIndex = GetReadySkillIndex();
                if (readySkillIndex != -1)
                {
                    return true;
                }
                else if (attackStrategy != null)
                {
                    if (attackStrategy.CheckCanAttack())
                        return IsTargetInAttackTriggeredRange(AttackRange);
                }

                return false;
            }
        }

        protected override void TriggerAtttack()
        {
            if (ownerModel.IsBlockControl)
                return;
            if (ownerModel.CurrentTargetedTarget.IsObject)
            {
                RunAttackAsync().Forget();
            }
            else
            {
                var readySkillIndex = GetReadySkillIndex();
                if (readySkillIndex != -1)
                {
                    _currentlyUsedSkillIndex = readySkillIndex;
                    RunSkillAsync(_skillStrategies[_currentlyUsedSkillIndex]).Forget();
                }
                else
                {
                    RunAttackAsync().Forget();
                }
            }
        }

        protected override async UniTask RunAttackAsync()
        {
            await base.RunAttackAsync();
            UpdateSkillsFromAttack();
            RunSkillCanTriggerCompletedAttack();
        }

        protected virtual void RunSkillCanTriggerCompletedAttack()
        {
            if (ownerModel.CheckCanAttack)
            {
                if (ownerModel.CurrentTargetedTarget != null)
                {
                    var readySkillIndex = GetReadySkillIndex();
                    if (readySkillIndex != -1)
                    {
                        bool isTriggerWhenCompletedAttack = _skillStrategies[readySkillIndex].CheckCanTriggerSkillWhenCompletedAttack();
                        if (isTriggerWhenCompletedAttack)
                        {
                            _currentlyUsedSkillIndex = readySkillIndex;
                            RunSkillAsync(_skillStrategies[_currentlyUsedSkillIndex]).Forget();
                        }
                    }
                }
            }
        }
        
        private async UniTaskVoid RunSkillAsync(ISkillStrategy skillStrategy)
        {
            ownerModel.IsUsingSkill = true;
            await skillStrategy.OperateSkillAsync();
            ownerModel.IsUsingSkill = false;
            ownerModel.UpdateState(CharacterState.Idle);
        }

        protected override void StopActionByAllMeans()
        {
            base.StopActionByAllMeans();
            if (ownerModel.IsUsingSkill)
            {
                ownerModel.IsUsingSkill = false;
                _skillStrategies[_currentlyUsedSkillIndex].Cancel();
            }
        }

        protected override void StopAttacking()
        {
            var hasOperatedAttackPoint = attackStrategy.Cancel();
            if (hasOperatedAttackPoint)
                UpdateSkillsFromAttack();
        }

        private void UpdateSkillsFromAttack()
        {
            if (!ownerModel.CurrentAttackedTarget.IsObject && _hasSkills)
            {
                foreach (var skillStrategy in _skillStrategies)
                    skillStrategy.GetUpdatedFromAttack();
            }
        }

        private int GetReadySkillIndex()
        {
            if (_hasSkills)
            {
                for (int i = 0; i < _skillStrategies.Length; i++)
                {
                    if (_skillStrategies[i].CheckCanUseSkill())
                    {
                        if (_skillStrategies[i].CastRange > 0)
                        {
                            var isTargetInCastRange = IsTargetInAttackTriggeredRange(_skillStrategies[i].CastRange);
                            if (isTargetInCastRange)
                                return i;
                        }
                        else
                        {
                            var isTargetInAttackRange = IsTargetInAttackTriggeredRange(AttackRange);
                            if (isTargetInAttackRange)
                                return i;
                        }
                    }
                }
            }

            return -1;
        }

        private void OnSkillStrategyUpdated(List<SkillModel> updatedSkillModels)
        {
            StopActionByAllMeans();

            if (_hasSkills)
            {
                foreach (var skillStrategy in _skillStrategies)
                    skillStrategy.Dispose();
            }

            _skillMaxCastRange = -1;
            _currentlyUsedSkillIndex = -1;

            updatedSkillModels.Sort();
            if (updatedSkillModels != null && updatedSkillModels.Count > 0)
            {
                _hasSkills = true;
                _skillStrategies = new ISkillStrategy[updatedSkillModels.Count];
                var cooldownReduction = ownerModel.GetTotalStatValue(StatType.CooldownReduction);
                for (int i = 0; i < updatedSkillModels.Count; i++)
                {
                    if (updatedSkillModels[i].IsSkillTakeBreakByCooldown)
                        updatedSkillModels[i].Cooldown *= (1 - cooldownReduction);

                    if (updatedSkillModels[i].CastRange > 0 && updatedSkillModels[i].CastRange > _skillMaxCastRange)
                        _skillMaxCastRange = updatedSkillModels[i].CastRange;

                    var skillStrategy = SkillStrategyFactory.GetSkillStrategy((SkillType)updatedSkillModels[i].SkillType);
                    skillStrategy.Init(updatedSkillModels[i], ownerModel, transform);
                    _skillStrategies[i] = skillStrategy;
                }
            }
            else _hasSkills = false;
        }

        private void OnReactionChangedOnAttack(CharacterReactionType characterReactionType)
        {
            switch (characterReactionType)
            {
                case CharacterReactionType.justMoveInGroup:
                    StopActionByAllMeans();
                    break;
            }
        }

        #endregion Class Methods
    }
}