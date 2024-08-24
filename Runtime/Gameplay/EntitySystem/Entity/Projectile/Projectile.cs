using System;
using UnityEngine;
using Runtime.Manager.Pool;
using Runtime.Gameplay.Manager;
using Runtime.Extensions;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    /// <summary>
    /// This entity simulates the behavior of a projectile.<br/>
    /// </summary>
    [DisallowMultipleComponent]
    public class Projectile : Disposable, IProjectile
    {
        #region Members

        [SerializeField] protected bool isSingleTarget;
        [SerializeField] protected float moveSpeed;
        [SerializeField] protected bool isRotatable;
        [SerializeField] protected bool causeAOEDamage;
        [SerializeField] protected bool isAlwaysGetHitTarget = false;
        [SerializeField] protected string impactPrefabName;

        [ShowIf(nameof(causeAOEDamage), true)] [SerializeField] [Min(0.01f)]
        protected float rangeAffectRadius;

        [ShowIf(nameof(causeAOEDamage), true)] [SerializeField]
        protected string explosionAOEPrefabName;

        protected bool hasHitTarget;
        protected float moveDistance;
        protected float damageFactorValue;
        protected DamageSource damageSource;
        protected StatusEffectModel[] damageStatusEffectModels;
        protected Vector3 moveDirection;
        protected Vector3 previousPosition;
        protected Vector3 originalPosition;
        protected IInteractable interactableTarget;
        protected ProjectileFeatureData projectileFeatureData;

        #endregion Members

        #region Properties

        public CharacterModel CreatorModel
        {
            get;
            private set;
        }

        public Vector3 Position => transform.position;

        protected Action CompletedMovingAction { get; set; }

        #endregion Properties

        #region API Methods

        protected virtual void Update()
        {
            transform.position = transform.position + moveSpeed * moveDirection.normalized * Time.unscaledDeltaTime;
            CheckRotateProjectile();
            if (Vector2.SqrMagnitude(originalPosition - Position) > moveDistance * moveDistance)
                CompleteMovement();
        }

        protected virtual void OnTriggerEnter2D(Collider2D collider)
        {
            var interactable = collider.GetComponent<IInteractable>();
            if (interactable != null && interactable.CanGetAffected(CreatorModel, damageSource))
            {
                if (isSingleTarget)
                {
                    if (!hasHitTarget)
                    {
                        hasHitTarget = true;
                        DamageTarget(interactable);
                        CompleteMovement();
                    }
                }
                else DamageTarget(interactable);
            }
        }

        #endregion API Methods

        #region Class Methods

        public virtual void Build(CharacterModel creatorModel, Vector3 position)
        {
            transform.position = position;
            originalPosition = position;
            previousPosition = originalPosition;
            CreatorModel = creatorModel;
            HasDisposed = false;
        }

        public virtual void Init(float moveDistance, float damageFactorValue, DamageSource damageSource, StatusEffectModel[] damageStatusEffectModels,
                                 Vector2 moveDirection, IInteractable interactableTarget, Action completedMovingAction, ProjectileFeatureData projectileFeatureData = null)
        {
            this.moveDistance = moveDistance;
            this.damageFactorValue = damageFactorValue;
            this.damageSource = damageSource;
            this.damageStatusEffectModels = damageStatusEffectModels;
            this.moveDirection = moveDirection;
            this.projectileFeatureData = projectileFeatureData;
            hasHitTarget = false;
            CompletedMovingAction = completedMovingAction;
            if (isAlwaysGetHitTarget)
                SetTargetCache(interactableTarget);
        }

        public virtual void Init(float moveDistance, float damageFactorValue, DamageSource damageSource, StatusEffectModel[] damageStatusEffectModels,
                                 Vector2 moveDirection, Vector2 positionTarget, Action completedMovingAction, ProjectileFeatureData projectileFeatureData = null)
        {
            this.moveDistance = moveDistance;
            this.damageFactorValue = damageFactorValue;
            this.damageSource = damageSource;
            this.damageStatusEffectModels = damageStatusEffectModels;
            this.moveDirection = moveDirection;
            this.projectileFeatureData = projectileFeatureData;
            hasHitTarget = false;
            CompletedMovingAction = completedMovingAction;
        }

        public override void Dispose()
        {
            if (!HasDisposed)
                HasDisposed = true;
        }

        protected virtual void CompleteMovement()
        {
            if (causeAOEDamage)
            {
                var colliders = Physics2D.OverlapCircleAll(transform.position, rangeAffectRadius);
                foreach (var collider in colliders)
                {
                    var interactable = collider.GetComponent<IInteractable>();
                    if (interactable != null && interactable.CanGetAffected(CreatorModel, damageSource))
                        DamageTarget(interactable);
                }

                SpawnAOEVFXAsync().Forget();
            }

            if (isAlwaysGetHitTarget && interactableTarget != null)
            {
                DamageTarget(interactableTarget);
                interactableTarget = null;
            }

            if (CompletedMovingAction != null)
                CompletedMovingAction.Invoke();

            DestroySelf();
        }

        protected virtual void CheckRotateProjectile()
        {
            if (isRotatable)
            {
                var direction = (transform.position - previousPosition).normalized;
                if (direction == Vector3.zero) return;
                transform.rotation = direction.ToQuaternion();
                previousPosition = transform.position;
            }
        }

        protected virtual void DestroySelf()
        {
            GenerateImpact(transform.position).Forget();
            EntitiesManager.Instance.RemoveGameObject(gameObject);
        }

        protected virtual async UniTaskVoid GenerateImpact(Vector3 position)
        {
            if (!string.IsNullOrEmpty(impactPrefabName))
            {
                var impact = await PoolManager.Instance.Get(impactPrefabName, this.GetCancellationTokenOnDestroy());
                impact.transform.position = position;
            }
        }

        protected virtual void SetTargetCache(IInteractable interactableTarget)
        {
            if (interactableTarget != null && !interactableTarget.Model.IsDead)
            {
                if (interactableTarget.Model.EntityType != CreatorModel.EntityType && interactableTarget.IsCharacter)
                    this.interactableTarget = interactableTarget;
            }
        }

        protected async UniTaskVoid SpawnAOEVFXAsync()
        {
            if (!string.IsNullOrEmpty(explosionAOEPrefabName))
            {
                var explosionAOEEffectGameObject = await PoolManager.Instance.Get(explosionAOEPrefabName, this.GetCancellationTokenOnDestroy());
                explosionAOEEffectGameObject.transform.position = transform.position;
            }
        }

        public void SetMoveSpeed(float newMoveSpeed)
        {
            if (newMoveSpeed > 0)
                moveSpeed = newMoveSpeed;
        }

        protected virtual void DamageTarget(IInteractable interactable)
        {
            DamageInfo damageInfo = null;
            var damageDirection = (interactable.Model.Position - (Vector2)transform.position).normalized;
            if (projectileFeatureData != null)
            {
                if (projectileFeatureData.projectileFeatureType == ProjectileFeatureType.CauseCritOnBurned)
                    damageInfo = GetDamageInfoWithCauseCritOnBurned(interactable);
                else if (projectileFeatureData.projectileFeatureType == ProjectileFeatureType.CauseMoreDamageOnBleed)
                    damageInfo = GetDamageInfoWithCauseMoreDamageOnBleed(interactable);
                else if (projectileFeatureData.projectileFeatureType == ProjectileFeatureType.CauseFreezeOnChill)
                    damageInfo = GetDamageInfoWithFreezeOnChill(interactable);
                else if (projectileFeatureData.projectileFeatureType == ProjectileFeatureType.CauseMoreDamageOnPoison)
                    damageInfo = GetDamageInfoWithCauseMoreDamageOnPoison(interactable);
                else if (projectileFeatureData.projectileFeatureType == ProjectileFeatureType.CauseCastOnCrit)
                {
                    var projectileCauseCastOnCritFeature = projectileFeatureData.projectileFeature as ProjectileCauseCastOnCritFeature;
                    var decreaseDamePercentStep = projectileCauseCastOnCritFeature.decreaseDamePercentStep;
                    var ratioDamage = 1 - decreaseDamePercentStep * projectileCauseCastOnCritFeature.countTargetGetHit;
                    if (ratioDamage < 0) ratioDamage = 0;
                    var finalDamageFactorValue = damageFactorValue * ratioDamage;
                    damageInfo = CreatorModel.GetDamageInfo(damageSource, finalDamageFactorValue, damageStatusEffectModels, interactable.Model);
                    if (projectileCauseCastOnCritFeature.canRefreshCooldown)
                    {
                        if (damageInfo.damageSource == DamageSource.FromCritAttack || damageInfo.damageSource == DamageSource.FromCritSkill)
                            projectileCauseCastOnCritFeature.projectileFeatureCallBack?.Invoke();
                    }
                    projectileCauseCastOnCritFeature.countTargetGetHit++;
                }
            }
            else damageInfo = CreatorModel.GetDamageInfo(damageSource, damageFactorValue, damageStatusEffectModels, interactable.Model);
            interactable.GetHit(damageInfo, damageDirection);
        }

        protected DamageInfo GetDamageInfoWithCauseCritOnBurned(IInteractable interactable)
        {
            if (interactable.IsCharacter)
            {
                var characterModel = interactable.Model as CharacterModel;
                if (characterModel.CheckContainStatusEffectInStack(StatusEffectType.BurnAttack))
                    return CreatorModel.GetDamageInfo(damageSource, damageFactorValue, damageStatusEffectModels, interactable.Model, true);
                else
                    return CreatorModel.GetDamageInfo(damageSource, damageFactorValue, damageStatusEffectModels, interactable.Model);
            }
            else return CreatorModel.GetDamageInfo(damageSource, damageFactorValue, damageStatusEffectModels, interactable.Model);
        }

        protected DamageInfo GetDamageInfoWithCauseMoreDamageOnBleed(IInteractable interactable)
        {
            if (interactable.IsCharacter)
            {
                var characterModel = interactable.Model as CharacterModel;
                var damageFactorValueLocalValue = damageFactorValue;
                if (characterModel.CheckContainStatusEffectInStack(StatusEffectType.BleedAttack))
                {
                    var projectileCauseMoreDamageOnBleedFeature = projectileFeatureData.projectileFeature as ProjectileCauseMoreDamageOnBleedFeature;
                    damageFactorValueLocalValue += projectileCauseMoreDamageOnBleedFeature.increaseDamage;
                    var damageInfo = CreatorModel.GetDamageInfo(damageSource, damageFactorValueLocalValue, damageStatusEffectModels, interactable.Model);
                    return damageInfo;
                }
                else return CreatorModel.GetDamageInfo(damageSource, damageFactorValueLocalValue, damageStatusEffectModels, interactable.Model);
            }
            else return CreatorModel.GetDamageInfo(damageSource, damageFactorValue, damageStatusEffectModels, interactable.Model);
        }
        
        protected DamageInfo GetDamageInfoWithCauseMoreDamageOnPoison(IInteractable interactable)
        {
            if (interactable.IsCharacter)
            {
                var characterModel = interactable.Model as CharacterModel;
                var damageFactorValueLocalValue = damageFactorValue;
                if (characterModel.CheckContainStatusEffectInStack(StatusEffectType.PoisonAttack))
                {
                    var projectileCauseMoreDamageOnPoisonFeature = projectileFeatureData.projectileFeature as ProjectileCauseMoreDamageOnPoisonFeature;
                    damageFactorValueLocalValue += projectileCauseMoreDamageOnPoisonFeature.increaseDamage;
                    var damageInfo = CreatorModel.GetDamageInfo(damageSource, damageFactorValueLocalValue, damageStatusEffectModels, interactable.Model);
                    return damageInfo;
                }
                else return CreatorModel.GetDamageInfo(damageSource, damageFactorValueLocalValue, damageStatusEffectModels, interactable.Model);
            }
            else return CreatorModel.GetDamageInfo(damageSource, damageFactorValue, damageStatusEffectModels, interactable.Model);
        }

        protected DamageInfo GetDamageInfoWithFreezeOnChill(IInteractable interactable)
        {
            if (interactable.IsCharacter)
            {
                var characterModel = interactable.Model as CharacterModel;
                var canReceiveFreezeStatusEffect = Constant.CanCharacterReceiveFreezeStatusEffect(characterModel.EntityId);
                if (canReceiveFreezeStatusEffect && characterModel.GetStatusEffectStackCount(StatusEffectType.Chill) + 1 >= Constant.COUNT_STACK_CHILL_TO_FREEZE)
                {
                    characterModel.ClearStatusEffectStack(StatusEffectType.Chill);
                    var damageInfo = CreatorModel.GetDamageInfo(damageSource, damageFactorValue, damageStatusEffectModels, interactable.Model);
                    var projectileCauseMoreFreezeOnChill = projectileFeatureData.projectileFeature as ProjectileCauseFreezeOnChillFeature;
                    var statusEffectModels = new List<StatusEffectModel>();
                    FreezeStatusEffectModel freezeStatusEffectModel = new FreezeStatusEffectModel(projectileCauseMoreFreezeOnChill.freezeDuration);
                    statusEffectModels.Add(freezeStatusEffectModel);
                    damageInfo.damageStatusEffectModels = statusEffectModels.ToArray();
                    return damageInfo;
                }
                else return CreatorModel.GetDamageInfo(damageSource, damageFactorValue, damageStatusEffectModels, interactable.Model);
            }
            else return CreatorModel.GetDamageInfo(damageSource, damageFactorValue, damageStatusEffectModels, interactable.Model);
        }

        #endregion Class Methods
    }

    public class ProjectileFeatureData
    {
        #region Members

        public ProjectileFeatureType projectileFeatureType;
        public IProjectileFeature projectileFeature;

        #endregion Members

        #region Struct Methods

        public ProjectileFeatureData(ProjectileFeatureType projectileFeatureType, IProjectileFeature projectileFeature)
        {
            this.projectileFeatureType = projectileFeatureType;
            this.projectileFeature = projectileFeature;
        }

        #endregion Struct Methods
    }

    public enum ProjectileFeatureType
    {
        None = 0,
        CauseCritOnBurned = 1,
        CauseMoreDamageOnBleed = 2,
        CauseFreezeOnChill = 3,
        CauseCastOnCrit = 4,
        CauseMoreDamageOnPoison = 5,
    }

    public interface IProjectileFeature
    {
        #region Properties

        ProjectileFeatureType ProjectileFeatureType { get; }

        #endregion Properties
    }

    public class ProjectileCauseCritOnBurnedFeature : IProjectileFeature
    {
        #region Properties

        public ProjectileFeatureType ProjectileFeatureType
            => ProjectileFeatureType.CauseCritOnBurned;

        #endregion Properties
    }

    public class ProjectileCauseMoreDamageOnBleedFeature : IProjectileFeature
    {
        #region Members

        public float increaseDamage;

        #endregion Members

        #region Properties

        public ProjectileFeatureType ProjectileFeatureType
            => ProjectileFeatureType.CauseMoreDamageOnBleed;

        #endregion Properties

        #region Struct Methods

        public ProjectileCauseMoreDamageOnBleedFeature(float increaseDamage)
            => this.increaseDamage = increaseDamage;

        #endregion Struct Methods
    }

    public class ProjectileCauseFreezeOnChillFeature : IProjectileFeature
    {
        #region Members

        public float freezeDuration;

        #endregion Members

        #region Properties

        public ProjectileFeatureType ProjectileFeatureType
            => ProjectileFeatureType.CauseFreezeOnChill;

        #endregion Properties

        #region Struct Methods

        public ProjectileCauseFreezeOnChillFeature(float freezeDuration)
            => this.freezeDuration = freezeDuration;

        #endregion Struct Methods
    }

    public class ProjectileCauseCastOnCritFeature : IProjectileFeature
    {
        #region Members

        public float decreaseDamePercentStep;
        public Action projectileFeatureCallBack;
        public int countTargetGetHit;
        public bool canRefreshCooldown;

        #endregion Members
        
        #region Properties

        public ProjectileFeatureType ProjectileFeatureType
            => ProjectileFeatureType.CauseCastOnCrit;

        public ProjectileCauseCastOnCritFeature(bool canRefreshCooldown, float decreaseDamePercentStep, Action projectileFeatureCallBack = null)
        {
            this.decreaseDamePercentStep = decreaseDamePercentStep;
            this.projectileFeatureCallBack = projectileFeatureCallBack;
            this.countTargetGetHit = 0;
            this.canRefreshCooldown = canRefreshCooldown;
        }

        #endregion Properties
    }
    
    public class ProjectileCauseMoreDamageOnPoisonFeature : IProjectileFeature
    {
        #region Members

        public float increaseDamage;

        #endregion Members

        #region Properties

        public ProjectileFeatureType ProjectileFeatureType
            => ProjectileFeatureType.CauseMoreDamageOnPoison;

        #endregion Properties

        #region Struct Methods

        public ProjectileCauseMoreDamageOnPoisonFeature(float increaseDamage)
            => this.increaseDamage = increaseDamage;

        #endregion Struct Methods
    }
}