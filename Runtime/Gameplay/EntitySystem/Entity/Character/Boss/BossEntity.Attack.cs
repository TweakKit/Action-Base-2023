using UnityEngine;
using Runtime.Definition;
using Cysharp.Threading.Tasks;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class BossEntity : EnemyEntity
    {
        #region Members

        private bool _hasSkills;
        private float _skillMaxCastRange;
        private int _currentlyUsedSkillIndex;
        private ISkillStrategy[] _skillStrategies;

        #endregion Members

        #region Class Methods

        protected override void PartialInitializeAttack()
        {
            base.PartialInitializeAttack();
            var bossModel = ownerModel as BossModel;
            _skillMaxCastRange = -1;
            _currentlyUsedSkillIndex = -1;
            var skillModels = bossModel.SkillModels;
            skillModels.Sort();
            if (skillModels != null && skillModels.Count > 0)
            {
                _hasSkills = true;
                _skillStrategies = new ISkillStrategy[skillModels.Count];
                var cooldownReduction = ownerModel.GetTotalStatValue(StatType.CooldownReduction);
                for (int i = 0; i < skillModels.Count; i++)
                {
                    if (skillModels[i].IsSkillTakeBreakByCooldown)
                        skillModels[i].Cooldown = skillModels[i].Cooldown * (1 - cooldownReduction);

                    if (skillModels[i].CastRange > 0 && skillModels[i].CastRange > _skillMaxCastRange)
                        _skillMaxCastRange = skillModels[i].CastRange;

                    var skillStrategy = SkillStrategyFactory.GetSkillStrategy(skillModels[i].SkillType);
                    skillStrategy.Init(skillModels[i], ownerModel, transform);
                    _skillStrategies[i] = skillStrategy;
                }
            }
            else _hasSkills = false;
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

        protected override void TriggerAtttack()
        {
            if (ownerModel.IsBlockControl)
                return;
            var readySkillIndex = GetReadySkillIndex();
            if (readySkillIndex != -1)
            {
                _currentlyUsedSkillIndex = readySkillIndex;
                RunSkillAsync(_skillStrategies[_currentlyUsedSkillIndex]).Forget();
            }
            else RunAttackAsync().Forget();
        }

        protected override async UniTask RunAttackAsync()
        {
            await base.RunAttackAsync();
            UpdateSkillsFromAttack();
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

        #endregion Class Methods
    }
}