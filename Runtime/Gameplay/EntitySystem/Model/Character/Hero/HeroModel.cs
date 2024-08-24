using System.Collections.Generic;
using UnityEngine;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class HeroModel : CharacterModel
    {
        #region Members

        protected MovementStrategyType movementStrategyType;
        protected HeroModel followingModel;
        protected List<SkillModel> skillModels;

        #endregion Members

        #region Properties

        public override EntityType EntityType => EntityType.Hero;
        public HeroModel FollowingModel => followingModel;
        public List<SkillModel> SkillModels => skillModels;

        public MovementStrategyType MovementStrategyType
        {
            get => movementStrategyType;
            set
            {
                if (movementStrategyType != value)
                {
                    movementStrategyType = value;
                    MovementStrategyChangedEvent.Invoke(movementStrategyType);
                    UpdateState(movementStrategyType == MovementStrategyType.Spread ? CharacterState.Move : CharacterState.Idle);
                }
            }
        }

        public bool IsRangedAttack
        {
            get
            {
                return AttackType == AttackType.RangedAttack ||
                       AttackType == AttackType.RangedSpawnImpactAttack ||
                       AttackType == AttackType.RangedPenetrateProjectileAttack ||
                       AttackType == AttackType.RangedEmptyProjectileAttack ||
                       AttackType == AttackType.RangedLazerProjectileAttack;
            }
        }

        public bool IsMeleeAttack
        {
            get
            {
                return AttackType != AttackType.None &&
                       !IsRangedAttack;
            }
        }

        public Vector2 FormationPosition { get; set; }
        public bool HasCompletedFormation { get; set; }
        public bool HasEnemyTargetInDetectRange => currentTargetedTarget != null && currentTargetedTarget.IsEnemyOrBoss;

        #endregion Properties

        #region Class Methods

        public HeroModel(uint heroUId, string heroId, Vector2 formationPosition, HeroModelData heroModelData,
                         HeroModel followingModel, MovementStrategyType movementStrategyType)
            : base(heroUId, heroId, heroModelData)
        {
            this.followingModel = followingModel;
            this.movementStrategyType = movementStrategyType;
            this.skillModels = heroModelData.SkillModels;
            HasCompletedFormation = false;
            FormationPosition = formationPosition;
            UpdateState(movementStrategyType == MovementStrategyType.Spread ? CharacterState.Move : CharacterState.Idle);
        }

        public void SetFollowingModel(HeroModel followingModel)
            => this.followingModel = followingModel;

        private void SetForceMovementStrategyType(MovementStrategyType movementStrategyType)
        {
            this.movementStrategyType = movementStrategyType;
            MovementStrategyChangedEvent.Invoke(movementStrategyType);
            UpdateState(movementStrategyType == MovementStrategyType.Spread ? CharacterState.Move : CharacterState.Idle);
        }

        #endregion Class Methods
    }

    public class HeroModelData : CharacterModelData
    {
        #region Members

        protected List<SkillModel> skillModels;

        #endregion Members

        #region Properties

        public List<SkillModel> SkillModels => skillModels;

        #endregion Properties

        #region Class Methods

        public HeroModelData(string heroId, string heroVisualId, int detectedPriority, AttackType attackType,
                             float respawnDelay, List<SkillModel> skillModels, HeroLevelModel heroLevelModel)
            : base(heroId, heroVisualId, detectedPriority, attackType, respawnDelay, heroLevelModel)
            => this.skillModels = skillModels;

        #endregion Class Methods
    }

    public class HeroLevelModel : CharacterLevelModel
    {
        #region Class Methods

        public HeroLevelModel(int level, CharacterStats characterStats)
            : base(level, characterStats) { }

        #endregion Class Methods
    }
}