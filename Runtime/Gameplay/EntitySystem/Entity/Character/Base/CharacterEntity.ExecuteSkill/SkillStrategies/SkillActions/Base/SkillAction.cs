using System;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using Runtime.Definition;
using Runtime.Audio;
using Runtime.Extensions;
using Cysharp.Threading.Tasks;

namespace Runtime.Gameplay.EntitySystem
{
    public class SkillActionTransitionedData
    {
        #region Members

        public Vector2 castPosition;

        #endregion Members
    }

    public abstract class SkillAction : ISkillAction
    {
        #region Members

        protected CharacterModel creatorModel;
        protected Transform creatorTransform;
        protected SkillType skillType;
        protected SkillTargetType targetType;
        protected ICharacterSkillActionPlayer characterSkillActionPlayer;
        protected SkillActionPhase skillActionPhase;
        protected bool hasOperatedCastPoint;
        private float _castRange;

        #endregion Members

        #region Properties

        public SkillActionPhase SkillActionPhase => skillActionPhase;
        public bool HasOperatedCastPoint => hasOperatedCastPoint;
        public int TargetLayerMask { get; private set; }

        protected float CastRange
        {
            get
            {
                if (_castRange > 0)
                    return _castRange;
                else
                    return creatorModel.GetTotalStatValue(StatType.AttackRange);
            }
        }

        protected float AffectTargetRange
        {
            get
            {
                return CastRange + creatorModel.CurrentAttackedTarget.Model.BodyBoundRadius;
            }
        }

        #endregion Properties

        #region Class Methods

        public virtual void Init(CharacterModel creatorModel, Transform creatorTranform, SkillType skillType,
                                 SkillTargetType targetType, SkillActionPhase skillActionPhase, float castRange)
        {
            this.skillActionPhase = skillActionPhase;
            this.creatorModel = creatorModel;
            this.creatorTransform = creatorTranform;
            this.skillType = skillType;
            this.targetType = targetType;
            _castRange = castRange;
            hasOperatedCastPoint = false;
            TargetLayerMask = GetTargetLayerMask();
            characterSkillActionPlayer = creatorTransform.GetComponentInChildren<ICharacterSkillActionPlayer>(true);
            characterSkillActionPlayer?.Init(creatorModel);
        }

        public void PreExecuteAction()
            => hasOperatedCastPoint = false;

        public abstract UniTask ExecuteActionAsync(SkillActionTransitionedData skillActionTransitionedData, CancellationToken cancellationToken);

        public virtual void Cancel() { }

        protected virtual void TriggerCastPointOperation(CancellationToken cancellationToken)
            => hasOperatedCastPoint = true;

        private int GetTargetLayerMask()
        {
            if ((targetType & SkillTargetType.None) > 0)
                return 0;

            var targetLayerMask = 0;
            if ((targetType & SkillTargetType.Self) > 0)
            {
                switch (creatorModel.EntityType)
                {
                    case EntityType.Hero:
                        targetLayerMask |= Layer.HERO_LAYER_MASK;
                        break;

                    case EntityType.Enemy:
                    case EntityType.Boss:
                        targetLayerMask |= Layer.ENEMY_LAYER_MASK;
                        break;

                    case EntityType.ObjectTree:
                    case EntityType.ObjectCrystal:
                        targetLayerMask |= Layer.OBJECT_LAYER_MASK;
                        break;
                }
            }

            if (creatorModel.IsHeroBoss)
            {
                if ((targetType & SkillTargetType.Enemy) > 0 || (targetType & SkillTargetType.Boss) > 0)
                    targetLayerMask |= Layer.HERO_LAYER_MASK;
                if((targetType & SkillTargetType.Hero) > 0)
                    targetLayerMask |= Layer.ENEMY_LAYER_MASK;
            }
            else
            {
                if ((targetType & SkillTargetType.Enemy) > 0 || (targetType & SkillTargetType.Boss) > 0)
                    targetLayerMask |= Layer.ENEMY_LAYER_MASK;
                if((targetType & SkillTargetType.Hero) > 0)
                    targetLayerMask |= Layer.HERO_LAYER_MASK;
            }

            return targetLayerMask;
        }

        protected Vector2 GetProjectileDirection(Vector2 targetPosition, Vector2 sourcePosition)
        {
            var projectileDirection = (targetPosition - sourcePosition).normalized;
            if (projectileDirection == Vector2.zero)
                return creatorModel.FaceDirection;
            else
                return projectileDirection;
        }

        protected bool IsTargetInSkillRange()
        {
            if (!creatorModel.CurrentAttackedTarget.IsDead)
            {
                var sqrDistanceBetweenTargetAndOwner = Vector2.SqrMagnitude(creatorModel.CurrentAttackedTarget.Position - creatorModel.Position);
                return sqrDistanceBetweenTargetAndOwner <= AffectTargetRange * AffectTargetRange;
            }
            else return false;
        }

        protected Collider2D[] OverlapConeAll(CharacterModel creatorModel, Vector2 triggerDirection, Collider2D[] orinalTargets, float angleCone)
        {
            List<Collider2D> listColliderResult = new List<Collider2D>();
            for (int i = 0; i < orinalTargets.Length; i++)
            {
                Vector2 targetPosition = orinalTargets[i].transform.position;
                var directionTarget = (targetPosition - creatorModel.Position).normalized;
                var angleValue = Math.Abs(Vector2.Angle(triggerDirection, directionTarget));
                if (angleValue <= angleCone / 2)
                    listColliderResult.Add(orinalTargets[i]);
            }
            return listColliderResult.ToArray();
        }

        protected void PlaySoundSkill(CancellationToken cancellationToken)
        {
            var nameSound = string.Format(AudioConstants.CHARACTER_SKILL, skillType.ToString().ToSnakeCase());
            AudioController.Instance.PlaySoundEffectAsync(nameSound, cancellationToken).Forget();
        }

        #endregion Class Methods
    }
}