using UnityEngine;
using Runtime.Definition;

namespace Runtime.Gameplay.EntitySystem
{
    public abstract class EntityModel
    {
        #region Members

        protected uint entityUId;
        protected string entityId;
        protected string entityVisualId;
        protected Vector2 position;
        protected int detectedPriority;
        protected Vector2 originalPosition;
        protected bool isActive;
        protected EntityBodyBound bodyBound;

        #endregion Members

        #region Properties

        public virtual bool IsDead
        {
            get => false;
        }

        public virtual bool IsActive
        {
            get => isActive;
        }

        public EntityBodyBound BodyBound
        {
            get => bodyBound;
            set => bodyBound = value;
        }

        public Vector2 Position
        {
            get => position;
            set => position = value;
        }

        public Vector2 CenterPosition
        {
            get => position + Vector2.up * bodyBound.verticalOffset;
        }

        public Vector2 TopPosition
        {
            get => position + Vector2.up * (bodyBound.verticalOffset + bodyBound.height * 0.5f);
        }

        public Vector2 OriginalPosition
        {
            get => originalPosition;
            set => originalPosition = value;
        }

        public virtual bool CanCounterDamage => false;
        public virtual bool CanGetMoreDamage => false;
        public virtual bool IsHeroBoss => false;
        public virtual int Level { get { return 1; } }
        public float BodyBoundRadius { get { return bodyBound.Width * 0.5f; } }
        public uint EntityUId { get { return entityUId; } }
        public string EntityId { get { return entityId; } }
        public string EntityVisualId { get { return entityVisualId; } }
        public int DetectedPriority { get { return detectedPriority; } }
        public abstract EntityType EntityType { get; }

        #endregion Properties

        #region Class Methods

        public EntityModel(uint entityUId, string entityId, string entityVisualId, int detectedPriority)
        {
            InitEvents();
            this.entityUId = entityUId;
            this.entityId = entityId;
            this.entityVisualId = entityVisualId;
            this.detectedPriority = detectedPriority;
        }

        public virtual float GetTotalStatValue(StatType statType)
            => 0;

        public void SetActive(bool isActive)
            => this.isActive = isActive;

        public static implicit operator bool(EntityModel entityModel)
            => entityModel != null;

        protected virtual void InitEvents() { }

        #endregion Class Methods
    }

    public static class EntityModelExtensions
    {
        #region Class Methods

        public static bool IsHero(this EntityType entityType)
            => entityType == EntityType.Hero;

        public static bool IsEnemyOrBoss(this EntityType entityType)
            => entityType == EntityType.Enemy || entityType == EntityType.Boss;

        public static bool IsCharacter(this EntityType entityType)
            => IsHero(entityType) || IsEnemyOrBoss(entityType);

        public static bool IsObjectTree(this EntityType entityType)
            => entityType == EntityType.ObjectTree;

        public static bool IsObjectCrystal(this EntityType entityType)
            => entityType == EntityType.ObjectCrystal;

        public static bool IsObject(this EntityType entityType)
            => entityType == EntityType.ObjectCrystal || entityType == EntityType.ObjectTree;

        #endregion Class Methods
    }
}