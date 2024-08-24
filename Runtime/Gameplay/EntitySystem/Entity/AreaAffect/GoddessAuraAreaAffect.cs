using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Runtime.Definition;
using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    [RequireComponent(typeof(Collider2D))]
    public class GoddessAuraAreaAffect : AreaAffect
    {
        #region Members

        protected float timeStepOneWave;
        private float _damageOneWaveByAttackPercent;
        private float _healingOneWaveByAttackByPercent;
        private float _radiusAoe;

        #endregion Members

        #region Properties

        #endregion Properties

        #region Class Methods

        public virtual void InitGoddessAuraAreaAffect(EntityModel senderModel,
                                                        GoddessAuraAreaAffectModel triggerOnWaveAreaAffectModel,
                                                        float damageOneWaveByAttackPercent,
                                                        float healingOneWaveByAttackByPercent,
                                                        float radiusAoe,
                                                        CancellationToken cancellationToken,
                                                        StatusEffectModel[] statusEffectModel = null,
                                                        Transform parent = null,
                                                        Action onDestroyCallback = null)
        {
            base.InitAreaAffect(senderModel, triggerOnWaveAreaAffectModel, statusEffectModel, parent, onDestroyCallback);
            timeStepOneWave = triggerOnWaveAreaAffectModel.TimeStepOneWave;
            _damageOneWaveByAttackPercent = damageOneWaveByAttackPercent;
            _healingOneWaveByAttackByPercent = healingOneWaveByAttackByPercent;
            _radiusAoe = radiusAoe;
            StartTriggerByTimeStep(cancellationToken).Forget();
        }

        private async UniTask StartTriggerByTimeStep(CancellationToken cancellationToken)
        {
            var timeStepOneWaveInMilisecond = (int)(timeStepOneWave * 1000);
            var creatorModel = senderModel as CharacterModel;
            var statusEffectModels = new StatusEffectModel[1];
            HealAttackStatusEffectModel healAttackStatusEffectModel = new HealAttackStatusEffectModel(_healingOneWaveByAttackByPercent);
            statusEffectModels[0] = healAttackStatusEffectModel;

            while (colliderSelf.enabled && !HasFinished)
            {
                if (senderModel.IsDead)
                {
                    ForceFinish();
                    break;
                }
                var colliders = Physics2D.OverlapCircleAll(transform.position, _radiusAoe);
                foreach (var collider in colliders)
                {
                    var interactable = collider.GetComponent<IInteractable>();
                    if (interactable != null && !interactable.IsDead)
                    {
                        if (interactable.IsHero)
                        {
                            if (teammateTargetType.IsHero())
                            {
                                var affectDirection = (interactable.Model.Position - creatorModel.Position).normalized;
                                interactable.GetAffected(creatorModel, statusEffectModels, affectDirection);
                            }
                            else if (rivalsTargetType.IsHero())
                            {
                                var affectDirection = (interactable.Model.Position - creatorModel.Position).normalized;
                                var damageInfo = creatorModel.GetDamageInfo(DamageSource.FromSkill, _damageOneWaveByAttackPercent, null, interactable.Model);
                                interactable.GetHit(damageInfo, affectDirection);
                            }
                        }
                        else if (interactable.IsEnemyOrBoss)
                        {
                            if (teammateTargetType.IsEnemyOrBoss())
                            {
                                var affectDirection = (interactable.Model.Position - creatorModel.Position).normalized;
                                interactable.GetAffected(creatorModel, statusEffectModels, affectDirection);
                            }
                            else if (rivalsTargetType.IsEnemyOrBoss())
                            {
                                var affectDirection = (interactable.Model.Position - creatorModel.Position).normalized;
                                var damageInfo = creatorModel.GetDamageInfo(DamageSource.FromSkill, _damageOneWaveByAttackPercent, null, interactable.Model);
                                interactable.GetHit(damageInfo, affectDirection);
                            }
                        }
                    }
                }
                if(HasFinished)
                    break;
                await UniTask.Delay(timeStepOneWaveInMilisecond, cancellationToken: cancellationToken);
            }
        }

        protected override void OnTriggerAffect(IInteractable interactable)
        {
            var affectDirection = senderModel.Position - interactable.Position;
            interactable.GetAffected(senderModel, statusEffectModel, affectDirection);
            var characterModel = interactable.Model as CharacterModel;
            if (!characterModel.IsInGoddessAuraAreaAffect)
            {
                characterModel.IsInGoddessAuraAreaAffect = true;
            }
        }

        protected override void OutTriggerAffect(IInteractable interactable)
        {
            var characterModel = interactable.Model as CharacterModel;
            if (characterModel.IsInGoddessAuraAreaAffect)
            {
                characterModel.IsInGoddessAuraAreaAffect = false;
            }
        }

        #endregion Class Methods

        #region API Methods

        #endregion API Methods
    }
}