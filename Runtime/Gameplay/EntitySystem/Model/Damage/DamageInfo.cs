namespace Runtime.Gameplay.EntitySystem
{
    public class DamageInfo
    {
        #region Members

        public DamageSource damageSource;
        public float damage;
        public StatusEffectModel[] damageStatusEffectModels;
        public EntityModel creatorModel;
        public EntityModel targetModel;
        public float critDamage;

        #endregion Members

        #region Class Methods

        public DamageInfo(DamageSource damageSource, float damage, StatusEffectModel[] damageStatusEffectModels, EntityModel creatorModel, EntityModel targetModel)
        {
            this.damageSource = damageSource;
            this.damage = damage;
            this.damageStatusEffectModels = damageStatusEffectModels;
            this.creatorModel = creatorModel;
            this.targetModel = targetModel;
        }

        #endregion Class Methods
    }

    public enum DamageSource
    {
        None,
        FromAttack,
        FromSkill,
        FromCritAttack,
        FromCritSkill,
        FromInteraction,
    }
}