using UnityEngine;

namespace Runtime.Gameplay.EntitySystem
{
    public partial class ObjectEntity : Entity<ObjectModel>, IInteractable
    {
        #region Properties

        public EntityModel Model
            => ownerModel;

        public bool IsMainHero
            => false;

        #endregion Properties

        #region Class Methods

        private partial void PartialValidateInteractable() { }
        private partial void PartialInitializeInteractable() { }
        private partial void PartialDisposeInteractable() { }

        public bool CanGetAffected(EntityModel interactingModel, DamageSource damageSource)
        {
            if (!ownerModel.IsDead)
                return interactingModel.EntityType.IsHero() && damageSource == DamageSource.FromInteraction;
            else
                return false;
        }

        public void GetHit(DamageInfo damageInfo, Vector2 damageDirection)
            => ownerModel.DebuffHp(damageInfo);

        public virtual void GetAffected(EntityModel senderModel, StatusEffectModel[] statusEffectModels, Vector2 affectDirection) { }

        #endregion Class Methods
    }
}