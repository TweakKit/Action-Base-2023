using Cysharp.Threading.Tasks;
using UnityEngine;
using Runtime.Definition;
using Runtime.Gameplay.Visual;

namespace Runtime.Gameplay.EntitySystem
{
    public enum CharacterState
    {
        Idle,
        Move,
        Attack,
        HardCC,
        StandStill,
        RefindTarget,
    }

    public abstract partial class CharacterModel : EntityModel
    {
        #region Members

        protected AttackType attackType;
        protected float maxHp;
        protected float currentHp;
        protected float baseMultiplyHp;
        protected float baseBonusHp;
        protected int level;
        protected bool hasMoveInput;
        protected bool isAttacking;
        protected bool isUsingSkill;
        protected Vector2 movePosition;
        protected Vector2 moveDirection;
        protected Vector2 faceDirection;
        protected bool faceRight;
        protected float respawnDelay;
        protected bool isInDamageReductionAreaAffect;
        protected bool isInGoddessAuraAreaAffect;
        protected IInteractable currentTargetedTarget;
        protected IInteractable currentAttackedTarget;
        protected CharacterState characterState;
        
        //For Shield.
        protected float maxDefense;
        protected float currentDefense;
        protected float baseMultiplyDefense;
        protected float baseBonusDefense;

        #endregion Members

        #region Properties

        public override int Level => level;

        public bool IsInDamageReductionAreaAffect
        {
            get => isInDamageReductionAreaAffect;
            set
            {
                if (isInDamageReductionAreaAffect != value)
                {
                    isInDamageReductionAreaAffect = value;
                }
            }
        }

        public bool IsInGoddessAuraAreaAffect
        {
            get => isInGoddessAuraAreaAffect;
            set
            {
                if (isInGoddessAuraAreaAffect != value)
                {
                    isInGoddessAuraAreaAffect = value;
                }
            }
        }

        public bool CanDealDamageBehindTarget { get; set; }

        public bool FaceRight
        {
            get => faceRight;
            set
            {
                if (faceRight != value)
                {
                    faceRight = value;
                    DirectionChangedEvent.Invoke();
                }
            }
        }

        public bool IsAttacking
        {
            get => isAttacking;
            set
            {
                if (isAttacking != value)
                    isAttacking = value;
            }
        }

        public bool IsUsingSkill
        {
            get => isUsingSkill;
            set
            {
                if (isUsingSkill != value)
                    isUsingSkill = value;
            }
        }

        public Vector2 FaceDirection
        {
            get
            {
                return faceDirection;
            }
        }

        public Vector2 MoveDirection
        {
            get => moveDirection;
            set
            {
                if (value != Vector2.zero && faceDirection != value)
                {
                    faceDirection = value;
                    FaceRight = value.x > 0.0f;
                }

                if (moveDirection != value)
                {
                    moveDirection = value;
                    if (hasMoveInput && moveDirection == Vector2.zero)
                    {
                        hasMoveInput = false;
                        MovementChangedEvent.Invoke();
                    }
                    else if (!hasMoveInput && moveDirection != Vector2.zero)
                    {
                        hasMoveInput = true;
                        MovementChangedEvent.Invoke();
                    }
                }
            }
        }

        public Vector2 MovePosition
        {
            get => movePosition;
            set => movePosition = value;
        }

        public override bool CanCounterDamage
        {
            get
            {
                return (characterStatus & CharacterStatus.Gargoyled) != 0;
            }
        }

        public override bool CanGetMoreDamage
        {
            get
            {
                return (characterStatus & CharacterStatus.GetMoreDamaged) != 0;
            }
        }
        public bool CheckCanAttack => !(IsDead || isAttacking || IsUsingSkill);
        public float RespawnDelay => respawnDelay;
        public float MaxHp => (maxHp + baseBonusHp) * baseMultiplyHp;
        public float MaxDefense => (maxDefense + baseBonusDefense) * baseMultiplyDefense;
        public float CurrentDefense => currentDefense;
        public float CurrentHp => currentHp;
        public override bool IsDead => currentHp <= 0;
        public bool HasMoveInput => hasMoveInput;
        public AttackType AttackType => attackType;
        public CharacterState CharacterState => characterState;
        public IInteractable CurrentTargetedTarget => currentTargetedTarget;
        public IInteractable CurrentAttackedTarget => currentAttackedTarget;

        public float DecreaseTakeHpPercent { get; set; } = 0.0f;
        public bool IsBlockControl { get; set; } = false;


        #endregion Properties

        #region Class Methods

        public CharacterModel(uint characterUId, string characterId, CharacterModelData characterModelData)
            : base(characterUId, characterId, characterModelData.CharacterVisualId, characterModelData.DetectedPriority)
        {
            level = characterModelData.CharacterLevelModel.Level;
            attackType = characterModelData.AttackType;
            faceRight = true;
            faceDirection = new Vector2(1, 0);
            respawnDelay = characterModelData.RespawnDelay;
            maxHp = characterModelData.CharacterLevelModel.CharacterStats.GetStatTotalValue(StatType.HealthPoint);
            currentHp = maxHp;
            baseMultiplyHp = 1f;
            baseBonusHp = 0f;
            DecreaseTakeHpPercent = 0.0f;
            
            maxDefense = 0;
            baseMultiplyDefense = 1f;
            baseBonusDefense = 0f;
            currentDefense = 0;
            
            this.IsBlockControl = false;
            UpdateState(CharacterState.Idle);
            InitStats(characterModelData.CharacterLevelModel.CharacterStats);
        }

        public virtual float DebuffHp(DamageInfo damageInfo)
        {
            if (!IsDead)
            {
                var debuffValue = damageInfo.damage;
                if (Constant.IsAlwaysTakeOneDamageValueCharacter(EntityId))
                    debuffValue = 1;
                
                if (currentDefense > 0)
                {
                    if (CanChangeHealthPoint())
                    {
                        if (debuffValue >= currentDefense)
                        {
                            debuffValue -= currentDefense;
                            currentDefense = 0;
                            ShieldChangedEvent.Invoke(currentDefense, damageInfo.damageSource);
                        }
                        else
                        {
                            currentDefense -= debuffValue;
                            ShieldChangedEvent.Invoke(-debuffValue, damageInfo.damageSource);
                            debuffValue = 0;
                            return debuffValue;
                        }
                    }
                }
                
                if (currentHp + debuffValue <= 0)
                    debuffValue = currentHp;

                if (!CanChangeHealthPoint())
                    debuffValue = 0;

                currentHp -= debuffValue;
                HealthChangedEvent.Invoke(-debuffValue, damageInfo.damageSource);

                if (IsDead)
                    DeathEvent.Invoke(damageInfo.damageSource);

                return debuffValue;
            }

            return 0;
        }

        public void MissedDamage()
        {
            if (!IsDead)
                ReactionChangedEvent.Invoke(CharacterReactionType.JustMissDamage);
        }

        public void UpdateState(CharacterState characterState)
        {
            if (this.characterState != characterState)
            {
                this.characterState = characterState;
                switch (characterState)
                {
                    case CharacterState.Idle:
                        ReactionChangedEvent.Invoke(CharacterReactionType.JustIdle);
                        break;

                    case CharacterState.Move:
                        ReactionChangedEvent.Invoke(CharacterReactionType.JustMove);
                        break;

                    case CharacterState.StandStill:
                        ReactionChangedEvent.Invoke(CharacterReactionType.JustStandStill);
                        break;

                    case CharacterState.RefindTarget:
                        ReactionChangedEvent.Invoke(CharacterReactionType.JustRefindTarget);
                        break;
                }
            }
        }

        public bool CanUpdateTargetedTarget()
            => (characterStatus & CharacterStatus.Taunted) == 0;

        public bool CanChangeHealthPoint()
            => (characterStatus & CharacterStatus.Invincibilited) == 0;

        public bool IsOnDecreaseTakeHpStatus()
            => (characterStatus & CharacterStatus.DecreasedTakeHp) != 0;

        public void UpdateTargetedTarget(IInteractable targetedTarget)
            => currentTargetedTarget = targetedTarget;

        public void UpdateAttackedTarget(IInteractable attackedTarget)
        {
            currentAttackedTarget = attackedTarget;
            FaceRight = (attackedTarget.Position - Position).x > 0.0f;
        }

        public virtual DamageInfo GetDamageInfo(DamageSource damageSource,
                                                float damageFactorValue,
                                                StatusEffectModel[] damageModifierModels,
                                                EntityModel targetModel,
                                                bool isAutoCrit = false,
                                                bool causeNoDamage = false)
        {
            if (causeNoDamage)
            {
                var damageInfo = new DamageInfo(damageSource, 0.0f, damageModifierModels, this, targetModel);
                return damageInfo;
            }
            else
            {
                var attackDamage = GetTotalStatValue(StatType.AttackDamage);
                attackDamage *= damageFactorValue;
                var affectedEntityDamageReduction = targetModel.GetTotalStatValue(StatType.DamageReduction);
                var affectedEntityFixedDamageReduction = targetModel.GetTotalStatValue(StatType.FixedDamageReduction);
                var critChance = GetTotalStatValue(StatType.CritChance);
                float extraDamage = 0;
                if (isAutoCrit || critChance >= Random.Range(0.0f, 1.0f))
                {
                    var critDamage = GetTotalStatValue(StatType.CritDamage);
                    extraDamage = attackDamage * critDamage;
                    if (damageSource == DamageSource.FromAttack)
                        damageSource = DamageSource.FromCritAttack;
                    else if (damageSource == DamageSource.FromSkill)
                        damageSource = DamageSource.FromCritSkill;
                }

                var finalDamage = (attackDamage + extraDamage) * (1 - affectedEntityDamageReduction) - affectedEntityFixedDamageReduction;
                if (finalDamage <= 0)
                    finalDamage = 1;
                var lifeStealStatValue = GetTotalStatValue(StatType.LifeSteal);
                var addedHp = lifeStealStatValue * finalDamage;
                if (addedHp > 0.0f)
                    BuffHp(addedHp);

                if (targetModel.CanCounterDamage)
                {
                    var counterDamageStatValue = targetModel.GetTotalStatValue(StatType.CounterDamage);
                    if (counterDamageStatValue < 0) counterDamageStatValue = 0;
                    var damageCounterValue = finalDamage * counterDamageStatValue;
                    var damageInfo = new DamageInfo(DamageSource.FromSkill, damageCounterValue, null, targetModel, this);
                    DebuffHp(damageInfo);
                    GameplayVisualController.Instance.GenerateCounterDamageVFX(targetModel.CenterPosition).Forget();
                }

                if (targetModel.CanGetMoreDamage)
                {
                    var getMoreDamageStatValue = targetModel.GetTotalStatValue(StatType.GetMoreDamage);
                    if (getMoreDamageStatValue < 0) getMoreDamageStatValue = 0;
                    var damageGetMoreValue = finalDamage * getMoreDamageStatValue;
                    finalDamage += damageGetMoreValue;
                }

                return new DamageInfo(damageSource, finalDamage, damageModifierModels, this, targetModel);
            }
        }

        public void SetTransformMovePosition(Vector2 transformMovePosition)
        {
            MovePosition = transformMovePosition;
            MovePositionUpdatedEvent.Invoke();
        }

        protected partial void InitStats(CharacterStats characterStats);

        #endregion Class Methods
    }

    public class CharacterModelData
    {
        #region Members

        protected string characterId;
        protected string characterVisualId;
        protected int detectedPriority;
        protected AttackType attackType;
        protected float respawnDelay;
        protected CharacterLevelModel characterLevelModel;

        #endregion Members

        #region Properties

        public int DetectedPriority => detectedPriority;
        public AttackType AttackType => attackType;
        public float RespawnDelay => respawnDelay;
        public CharacterLevelModel CharacterLevelModel => characterLevelModel;

        public string CharacterVisualId
        {
            get
            {
                if (string.IsNullOrEmpty(characterVisualId))
                    return characterId;
                else
                    return characterVisualId;
            }
        }

        #endregion Properties

        #region Class Methods

        public CharacterModelData(string characterId,
                                  string characterVisualId,
                                  int detectedPriority,
                                  AttackType attackType,
                                  float respawnDelay,
                                  CharacterLevelModel characterLevelModel)
        {
            this.characterId = characterId;
            this.characterVisualId = characterVisualId;
            this.detectedPriority = detectedPriority;
            this.attackType = attackType;
            this.respawnDelay = respawnDelay;
            this.characterLevelModel = characterLevelModel;
        }

        public void SetCharacterStatsScaleFactor(float statsScaleFactor)
            => characterLevelModel.SetCharacterStatsScaleFactor(statsScaleFactor);

        public void SetStatsScaleFactorByStatType(StatType statType, float statsScaleFactor)
            => characterLevelModel.SetStatsScaleFactorByStatType(statType, statsScaleFactor);

        #endregion Class Methods
    }

    public class CharacterLevelModel
    {
        #region Members

        protected int level;
        protected CharacterStats characterStats;

        #endregion Members

        #region Properties

        public int Level => level;
        public CharacterStats CharacterStats => characterStats;

        #endregion Properties

        #region Class Methods

        public CharacterLevelModel(int level, CharacterStats characterStats)
        {
            this.level = level;
            this.characterStats = characterStats;
        }

        public void SetCharacterStatsScaleFactor(float statScaleFactor)
            => characterStats.SetStatsScaleFactor(statScaleFactor);

        public void SetStatsScaleFactorByStatType(StatType statType, float statScaleFactor)
            => characterStats.SetStatsScaleFactorByStatType(statType, statScaleFactor);

        #endregion Class Methods
    }
}