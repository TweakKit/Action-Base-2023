using Runtime.Definition;
using Runtime.Common.Resource;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class ObjectModel : EntityModel
    {
        #region Members

        protected float maxHp;
        protected float currentHp;
        protected float respawnDelay;
        protected string destroyedVfxPrefabName;
        protected bool markRespawnable;
        protected ObjectType objectType;
        protected ResourceData destroyedResourceData;

        #endregion Members

        #region Properties

        public override EntityType EntityType
        {
            get
            {
                switch (objectType)
                {
                    case ObjectType.Tree:
                        return EntityType.ObjectTree;

                    default:
                    case ObjectType.Crystal:
                        return EntityType.ObjectCrystal;
                }
            }
        }

        public ObjectType ObjectType => objectType;
        public override bool IsDead => currentHp <= 0;
        public float RespawnDelay => respawnDelay;
        public bool MarkRespawnable => markRespawnable;
        public string DestroyedVfxPrefabName => destroyedVfxPrefabName;
        public ResourceData DestroyedResourceData => destroyedResourceData;

        #endregion Properties

        #region Class Methods

        public ObjectModel(uint objectUId, string objectId, bool markRespawnable, ObjectType objectType, ObjectModelData objectModelData)
            : base(objectUId, objectId, objectModelData.ObjectVisualId, objectModelData.DetectedPriority)
        {
            this.objectType = objectType;
            this.maxHp = objectModelData.MaxHp;
            this.currentHp = maxHp;
            this.respawnDelay = objectModelData.RespawnDelay;
            this.markRespawnable = markRespawnable;
            this.destroyedResourceData = objectModelData.DestroyedResourceData;
            this.destroyedVfxPrefabName = "";
        }

        public void DebuffHp(DamageInfo damageInfo)
        {
            if (!IsDead)
            {
                var damageTaken = damageInfo.damage;
                damageTaken = damageTaken > 0 ? damageTaken : 0;

                var debuffValue = damageTaken;
                if (currentHp + damageTaken <= 0)
                    debuffValue = currentHp;

                currentHp -= debuffValue;
                HealthChangedEvent.Invoke(-debuffValue);

                if (IsDead)
                    DestroyEvent.Invoke();
            }
        }

        #endregion Class Methods
    }

    public class ObjectModelData
    {
        #region Members

        protected string objectId;
        protected string objectVisualId;
        protected int detectedPriority;
        protected float maxHp;
        protected float respawnDelay;
        protected ResourceData destroyedResourceData;

        #endregion Members

        #region Properties

        public int DetectedPriority => detectedPriority;
        public float MaxHp => maxHp;
        public float RespawnDelay => respawnDelay;
        public ResourceData DestroyedResourceData => destroyedResourceData;

        public string ObjectVisualId
        {
            get
            {
                if (string.IsNullOrEmpty(objectVisualId))
                    return objectId;
                else
                    return objectId;
            }
        }

        #endregion Properties

        #region Class Methods

        public ObjectModelData(string objectId,
                               string objectVisualId,
                               int detectedPriority,
                               float maxHp,
                               float respawnDelay,
                               ResourceData destroyedResourceData)
        {
            this.objectId = objectId;
            this.objectVisualId = objectVisualId;
            this.detectedPriority = detectedPriority;
            this.maxHp = maxHp;
            this.respawnDelay = respawnDelay;
            this.destroyedResourceData = destroyedResourceData;
        }

        #endregion Class Methods
    }
}