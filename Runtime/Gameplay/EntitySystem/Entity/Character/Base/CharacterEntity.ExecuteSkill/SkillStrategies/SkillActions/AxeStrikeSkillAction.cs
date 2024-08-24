using System.Threading;
using UnityEngine;
using Runtime.Definition;
using Cysharp.Threading.Tasks;

namespace Runtime.Gameplay.EntitySystem
{
    public class AxeStrikeSkillAction : SkillAction
    {
        #region Members

        private float _damageFactorValue;

        #endregion Members

        #region Class Methods

        public void Init(CharacterModel creatorModel,
                         Transform creatorTranform,
                         SkillType skillType,
                         SkillTargetType targetType,
                         SkillActionPhase skillActionPhase,
                         float castRange,
                         float damageFactorValue)
        {
            base.Init(creatorModel, creatorTranform, skillType, targetType, skillActionPhase, castRange);
            _damageFactorValue = damageFactorValue;
        }

        public async override UniTask ExecuteActionAsync(SkillActionTransitionedData skillActionTransitionedData, CancellationToken cancellationToken)
        {
            var hasFinishedAnimation = false;
            var characterPlayedSkillAction = new CharacterPlayedSkillAction
            (
                skillType: skillType,
                skillActionPhase: SkillActionPhase,
                eventTriggeredCallbackAction: () => TriggerCastPointOperation(cancellationToken),
                endActionCallbackAction: () => hasFinishedAnimation = true
            );
            PlaySoundSkill(cancellationToken);
            characterSkillActionPlayer.Play(characterPlayedSkillAction);
            await UniTask.WaitUntil(() => hasFinishedAnimation, cancellationToken: cancellationToken);
        }

        protected override void TriggerCastPointOperation(CancellationToken cancellationToken)
        {
            base.TriggerCastPointOperation(cancellationToken);
            if (IsTargetInSkillRange())
            {
                var damageInfo = creatorModel.GetDamageInfo(DamageSource.FromSkill, _damageFactorValue, null, creatorModel.CurrentAttackedTarget.Model);
                var damageDirection = (creatorModel.CurrentAttackedTarget.Position - creatorModel.Position).normalized;
                creatorModel.CurrentAttackedTarget.GetHit(damageInfo, damageDirection);
            }
        }

        #endregion Class Methods
    }
}