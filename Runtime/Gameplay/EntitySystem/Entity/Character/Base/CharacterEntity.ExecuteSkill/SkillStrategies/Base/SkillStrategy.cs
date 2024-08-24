using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using Runtime.Definition;
using Cysharp.Threading.Tasks;

namespace Runtime.Gameplay.EntitySystem
{
    public abstract class SkillStrategy<T> : ISkillStrategy where T : SkillModel
    {
        #region Members

        protected T ownerSkillModel;
        protected SkillActionTransitionedData skillActionTransitionedData;
        protected ISkillAction currentSkillAction;
        protected List<ISkillAction> skillActions;
        protected CharacterModel creatorModel;
        protected Transform creatorTransform;
        protected CancellationTokenSource executeSkillCancellationTokenSource;

        #endregion Members

        #region Properties

        public float CastRange { get; private set; }

        #endregion Properties

        #region Class Methods

        public void Init(SkillModel skillModel, CharacterModel creatorModel, Transform creatorTransform)
        {
            this.ownerSkillModel = skillModel as T;
            this.creatorModel = creatorModel;
            this.creatorTransform = creatorTransform;
            skillActions = new List<ISkillAction>();
            skillActionTransitionedData = new SkillActionTransitionedData();
            executeSkillCancellationTokenSource = new CancellationTokenSource();
            CastRange = ownerSkillModel.CastRange;
            creatorModel.StatChangedEvent += OnStatChangedOnSkill;
            InitActions(ownerSkillModel);
        }

        public bool CheckCanUseSkill()
            => ownerSkillModel.IsReady(creatorModel.IsHeroBoss);

        public bool CheckCanTriggerSkillWhenCompletedAttack()
            => this.ownerSkillModel.TriggerWhenCompletedAttack;

        public void GetUpdatedFromAttack()
        {
            if (ownerSkillModel.IsSkillTakeBreakByAttackCount)
                ownerSkillModel.CurrentAttackCountTrigger++;
        }

        public async UniTask OperateSkillAsync()
        {
            foreach (var skillAction in skillActions)
                skillAction.PreExecuteAction();

            foreach (var skillAction in skillActions)
            {
                currentSkillAction = skillAction;
                if (executeSkillCancellationTokenSource != null)
                    await skillAction.ExecuteActionAsync(skillActionTransitionedData, executeSkillCancellationTokenSource.Token);
            }

            RunSkillTakeBreakAsync().Forget();
        }

        public void Dispose()
        {
            if (executeSkillCancellationTokenSource != null)
            {
                executeSkillCancellationTokenSource.Cancel();
                executeSkillCancellationTokenSource.Dispose();
                executeSkillCancellationTokenSource = null;
            }
            foreach (var skillAction in skillActions)
                skillAction.Cancel();
        }

        public void Cancel()
        {
            executeSkillCancellationTokenSource.Cancel();
            executeSkillCancellationTokenSource.Dispose();
            executeSkillCancellationTokenSource = new CancellationTokenSource();
            currentSkillAction.Cancel();
            var hasOperatedCastPoint = currentSkillAction.HasOperatedCastPoint;
            if (!hasOperatedCastPoint)
            {
                if (ownerSkillModel.IsSkillTakeBreakByAttackCount)
                    ownerSkillModel.CurrentAttackCountTrigger = ownerSkillModel.AttackCountTrigger;
                else
                    ownerSkillModel.CurrentCooldown = 0;
            }
            else RunSkillTakeBreakAsync().Forget();
        }

        protected abstract void InitActions(T skillModel);

        private async UniTask RunSkillTakeBreakAsync()
        {
            if (ownerSkillModel.IsSkillTakeBreakByCooldown)
            {
                ownerSkillModel.CurrentCooldown = ownerSkillModel.Cooldown;
                if (ownerSkillModel.IsAutoRefreshCoolDown)
                {
                    ownerSkillModel.IsAutoRefreshCoolDown = false;
                    ownerSkillModel.CurrentCooldown = 0;
                }
                while (ownerSkillModel.CurrentCooldown > 0)
                {
                    ownerSkillModel.CurrentCooldown -= Time.deltaTime;
                    if (executeSkillCancellationTokenSource != null)
                        await UniTask.Yield(executeSkillCancellationTokenSource.Token);
                }
            }
            else
            {
                ownerSkillModel.CurrentAttackCountTrigger = 0;

                if (ownerSkillModel.IsAutoRefreshCoolDown)
                {
                    ownerSkillModel.CurrentAttackCountTrigger = ownerSkillModel.AttackCountTrigger;
                }
            }
        }

        private void OnStatChangedOnSkill(StatType statType, float updatedValue)
        {
            if (statType == StatType.CooldownReduction)
            {
                var cooldownReduction = updatedValue;
                if (ownerSkillModel.IsSkillTakeBreakByCooldown)
                {
                    if (1 - cooldownReduction > 0)
                        ownerSkillModel.Cooldown = ownerSkillModel.OriginalCooldown * (1 - cooldownReduction);
                }
            }
        }

        #endregion Class Methods
    }
}